Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot | Split-Path -Parent
$projectPath = Join-Path $repoRoot 'app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj'
$outputPath = Join-Path $repoRoot 'app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json'
$redoclyConfigPath = Join-Path $repoRoot '.redocly.yaml'
$nswagConfigPath = Join-Path $repoRoot 'support/scripts/openapi-v1.nswag.json'

function Assert-CommandAvailable {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Command,
        [Parameter(Mandatory = $true)]
        [string]$Message
    )

    if (-not (Get-Command $Command -ErrorAction SilentlyContinue)) {
        throw $Message
    }
}

function ConvertTo-CanonicalJson {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path
    )

    $canonicalizer = @'
const fs = require('fs');
const path = process.argv[1];
const sort = (value) => Array.isArray(value)
  ? value.map(sort)
  : value !== null && typeof value === 'object'
    ? Object.fromEntries(Object.keys(value).sort().map(key => [key, sort(value[key])]))
    : value;
fs.writeFileSync(path, JSON.stringify(sort(JSON.parse(fs.readFileSync(path, 'utf8'))), null, 2) + '\n', 'utf8');
'@

    & node -e $canonicalizer $Path
    if ($LASTEXITCODE -ne 0) {
        throw "❌ No se pudo canonizar el documento OpenAPI generado."
    }
}

Push-Location $repoRoot
try {
    Assert-CommandAvailable -Command 'dotnet' -Message '❌ Prerrequisito faltante: dotnet no está disponible en PATH.'
    Assert-CommandAvailable -Command 'node' -Message '❌ Prerrequisito faltante: node no está disponible en PATH.'
    Assert-CommandAvailable -Command 'npx' -Message '❌ Prerrequisito faltante: npx no está disponible en PATH.'

    if (-not (Test-Path $redoclyConfigPath)) {
        throw "❌ Prerrequisito faltante: no existe .redocly.yaml en $redoclyConfigPath"
    }

    if (-not (Test-Path $nswagConfigPath)) {
        throw "❌ Prerrequisito faltante: no existe configuración NSwag en $nswagConfigPath"
    }

    Write-Host "[1/4] Compilando NetRentManagerApi (Release)..."
    dotnet build $projectPath -c Release --tl:off | Out-Host

    if (-not (Test-Path $outputPath)) {
        throw "❌ No se generó OpenAPI en: $outputPath"
    }
    ConvertTo-CanonicalJson -Path $outputPath
    Write-Host "Documento OpenAPI generado"

    Write-Host "`n[2/4] Validando con Redocly..."
    & npx --yes @redocly/cli lint --config $redoclyConfigPath $outputPath
    if ($LASTEXITCODE -ne 0) {
        throw "❌ Redocly lint reportó errores. Revise la salida previa."
    }
    Write-Host "Validacion Redocly exitosa"

    Write-Host "`n[3/4] Generando cliente tipado con NSwag..."
    $clientOutputDir = Join-Path $repoRoot 'artifacts/openapi-client-smoke'
    $clientOutputFile = Join-Path $clientOutputDir 'NetRentManagerApiClient.cs'
    
    New-Item -ItemType Directory -Force -Path $clientOutputDir | Out-Null
    if (Test-Path $clientOutputFile) { Remove-Item $clientOutputFile }

    dotnet tool restore | Out-Host
    if ($LASTEXITCODE -ne 0) {
        throw "❌ dotnet tool restore falló. Revise la salida previa."
    }
    
    & dotnet tool run nswag run $nswagConfigPath | Out-Host
    if ($LASTEXITCODE -ne 0) {
        throw "❌ NSwag falló al generar el cliente smoke. Revise la salida previa."
    }
    
    if (-not (Test-Path $clientOutputFile)) {
        throw "❌ NSwag no generó el cliente tipado esperado en: $clientOutputFile"
    }
    Write-Host "Cliente tipado generado: $clientOutputFile"

    Write-Host "`n[4/4] OpenAPI v1 completamente regenerado y validado"
    Write-Host "Documento: $outputPath"
    Write-Host "Validacion: Redocly + NSwag pasadas"
}
finally {
    Pop-Location
}