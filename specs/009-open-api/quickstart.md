# Guía de Validación: OpenAPI v1 Público

## Prerrequisitos obligatorios

Verificar antes de implementar o validar:

```powershell
node --version
npm --version
npx @redocly/cli --version
dotnet tool restore
dotnet tool list
```

Resultados esperados:

- Node.js `>= 20`.
- npm y npx disponibles.
- `@redocly/cli` responde su versión.
- `NSwag.ConsoleCore` aparece en `dotnet tool list`.
- `.redocly.yaml` existe en la raíz.

Si falta una herramienta, detener la validación y corregir el entorno; no tratar la
salida como evidencia válida.

## Preparación

```powershell
npm install
dotnet tool restore
dotnet build .\app\backend\src\NetRentManagerApi\NetRentManagerApi.csproj -c Release --no-restore
```

**Importante**: la generación de `openapi/v1.json` solo se activa en configuración
`Release` (`OpenApiGenerateDocuments`/`OpenApiGenerateDocumentsOnBuild` en
`NetRentManagerApi.csproj`). Un build en `Debug` (el habitual al depurar en el IDE)
NO regenera ni modifica el archivo versionado.

El build en `Release` debe generar:

```text
app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json
```

## Script reproducible

```powershell
.\support\scripts\generate-openapi-v1.ps1
```

El script compila el proyecto con `dotnet build -c Release` (la única configuración
con la generación OpenAPI activa), comprueba el JSON, ejecuta Redocly lint y genera
el cliente C# mediante NSwag en:

```text
artifacts/openapi-client-smoke/
```

## Validación Redocly

```powershell
npx @redocly/cli lint --config .redocly.yaml .\app\backend\src\NetRentManagerApi\wwwroot\openapi\v1.json
```

Esperado: validación sin errores. `security-defined` permanece deshabilitada y
`operation-4xx-response` solo se reporta como warning.

## Pruebas automatizadas

```powershell
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
```

La suite debe cubrir:

1. Inventario exacto de los cinco endpoints y normalización `{id:guid}`.
2. Presencia de `components/schemas` compartidos y referencias `$ref`.
3. Drift inducido por path/método faltante o sobrante.
4. HTTP 200 y `application/json` para `/openapi/v1.json`.
5. Ausencia de Swagger UI, ReDoc UI y endpoints interactivos.
6. Lint Redocly y generación del cliente NSwag; si falta una herramienta requerida, la validación debe fallar y no omitirse silenciosamente.
7. Matriz de responses por operación: health y consultas `200`; listado inválido `400`; detalle inexistente `404`; creación `201/400/500`; actualización `200/400/404/413/415/500`.
8. Coincidencia del JSON regenerado con el archivo versionado en Git.

## Validación manual runtime

Levantar la API desde el proyecto existente y consultar:

```powershell
curl.exe -i http://localhost:5065/openapi/v1.json
```

Esperado: HTTP 200, `Content-Type: application/json`, JSON OpenAPI v1 y paths de
`/health`, `/api/properties` y `/api/properties/{id}` con sus métodos.

No deben responder como UI estas rutas:

```text
/swagger
/redoc
/scalar
```

## Evidencia de validación

Durante la implementación registrar:

- versiones de Node.js, npm, Redocly, .NET y NSwag;
- build que generó `wwwroot/openapi/v1.json`;
- lint Redocly correcto;
- cliente NSwag generado;
- pruebas de drift correctas y drift inducido fallido;
- respuesta HTTP 200 y Content-Type del documento;
- inventario exacto de cinco endpoints;
- ausencia de UI runtime;
- dos ejecuciones consecutivas reproducibles del script;
- resultado de la comparación byte a byte contra el JSON versionado;
- status codes verificados por operación y nombres exactos de schemas compartidos.

## Evidencia de validación

Fecha: 2026-09-11

Resultados:

- Node.js `v24.15.0`, npm `10.6.0`, Redocly `2.52.0`, .NET SDK `10.0.400` y NSwag `14.6.2` verificados.
- Build backend y solución: correctos; `wwwroot/openapi/v1.json` generado durante MSBuild.
- Redocly lint: correcto, con warnings esperables de licencia local, servidor localhost, health sin 4xx y multipart nullable.
- NSwag: cliente generado en `artifacts/openapi-client-smoke/NetRentManagerApiClient.cs`.
- Pruebas backend: 118 correctas, 0 errores y 0 omitidas.
- Pruebas OpenAPI focalizadas: 11 correctas, 0 errores y 0 omitidas.
- Drift runtime: inventario de 5 slices/operaciones alineado; drift inducido detectado.
- Regeneración reproducible: dos ejecuciones con SHA-256 `6201A31F4DAE615B211C45659F3C4F2A92D89F6B082E238A86C902318DD33002` idéntico.
- Runtime: `GET /openapi/v1.json` HTTP 200 con `Content-Type: application/json`; `/swagger`, `/redoc` y `/scalar` HTTP 404.
- No se agregaron migraciones, UI runtime, endpoints de negocio ni cambios de frontend.

## Evidencia de validación (T043 - generación solo en Release)

Fecha: 2026-09-12

- `dotnet build ...NetRentManagerApi.csproj -c Debug`: build correcto, `wwwroot/openapi/v1.json` NO se modifica (`git status --porcelain` sin salida para ese archivo).
- `support/scripts/generate-openapi-v1.ps1` (compila con `-c Release`): build, Redocly lint y cliente NSwag correctos; `git diff` contra el JSON versionado sin diferencias (sin drift).
