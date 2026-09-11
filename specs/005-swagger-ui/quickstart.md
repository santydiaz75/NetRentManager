# Guía de Validación: Swagger UI para el Backend

## Prerrequisitos

- .NET SDK requerido por `global.json`.
- Backend compilable desde `app/backend/src/NetRentManagerApi`.
- Documento `wwwroot/openapi/v1.json` generado por la iniciativa `009-open-api`.

Para preparar el smoke de navegador después de restaurar el proyecto de tests:

```powershell
& "app/backend/tests/NetRentManagerApiTests/bin/Debug/net10.0/playwright.ps1" install chromium
```

## Ejecutar en Development

Desde la raíz del repositorio:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj
```

Con el backend iniciado, validar:

```powershell
curl.exe -i http://localhost:5065/swagger
curl.exe -i http://localhost:5065/swagger/index.html
curl.exe -i http://localhost:5065/openapi/v1.json
```

### Resultado esperado

- `/swagger` responde `200` o redirige a la página principal.
- `/swagger/index.html` responde `200` con `text/html`.
- La interfaz muestra las operaciones presentes en OpenAPI v1.
- La configuración de la interfaz referencia `/openapi/v1.json`.
- `/openapi/v1.json` responde `200` con JSON OpenAPI v1.

## Ejecutar fuera de Development

Iniciar el mismo proyecto con otro entorno, por ejemplo:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Production"
dotnet run --project app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj
```

Validar:

```powershell
curl.exe -i http://localhost:5065/swagger
curl.exe -i http://localhost:5065/swagger/index.html
```

### Resultado esperado

- Ambas rutas responden `404 Not Found`.
- No se sirve HTML, JavaScript, CSS ni configuración de Swagger UI.
- El documento OpenAPI estático conserva el comportamiento definido por `009-open-api`; esta spec solo restringe la UI.

## Pruebas automatizadas

Ejecutar desde la raíz:

```powershell
dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj
```

La evidencia de cierre debe registrar la respuesta de las rutas en Development y fuera de Development, la referencia al documento v1 y el resultado de las pruebas.

## Evidencia de validación

Validación ejecutada el 2026-09-11:

- `dotnet build app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj --no-restore`: correcto, con tres advertencias preexistentes de soporte de paquetes .NET 10.
- `dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj --no-restore`: `122/122` pruebas correctas.
- Development real con `--no-launch-profile`: `/swagger` respondió `301`, `/swagger/index.html` respondió `200 text/html`, los assets JS/CSS respondieron `200` y `/openapi/v1.json` respondió `200 application/json`.
- Smoke Playwright con `SWAGGER_UI_BASE_URL=http://127.0.0.1:5095`: `2/2` escenarios correctos; `Try it out` ejecutó `GET /health` y comprobó respuesta `200 OK`, y el escenario de documento OpenAPI inválido mostró el contenedor de error de carga.
- Production real con `--no-launch-profile`: `/swagger`, `/swagger/index.html`, `swagger-ui.css`, `swagger-ui-bundle.js` y `swagger-ui-standalone-preset.js` respondieron `404`.
