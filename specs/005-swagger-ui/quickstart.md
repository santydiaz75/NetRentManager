# Guía de Validación: Interfaz Swagger UI del Backend

## Prerrequisitos

- SDK definido por `global.json` (`10.0.400`).
- PowerShell desde la raíz del repositorio.
- Solución `app/NetRentManager.sln`.
- Configuración local válida para arrancar `NetRentManagerApi`.

## Preparación

Restaurar y compilar la solución:

```powershell
dotnet restore .\app\NetRentManager.sln
dotnet build .\app\NetRentManager.sln --no-restore
```

## Pruebas automatizadas

Ejecutar las pruebas del backend:

```powershell
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
```

La cobertura específica de esta iniciativa debe comprobar:

1. Que `Program.cs` mantiene `AddOpenApi()`.
2. Que `Program.cs` registra Swagger UI en `Development`.
3. Que la composición sigue sin controllers ni lógica de negocio.
4. Que la solución compila tras añadir la dependencia necesaria.

## Prueba manual

Iniciar la API en `Development` y abrir:

```text
https://localhost:7065/swagger
```

También debe seguir estando disponible el documento OpenAPI publicado por la aplicación.

## Evidencia de validación

Fecha: 2026-09-11

Comandos ejecutados:

```powershell
dotnet build .\app\backend\src\NetRentManagerApi\NetRentManagerApi.csproj --no-restore
```

Pruebas ejecutadas:

- `NetRentManagerApiTests.Infrastructure.ProgramCompositionTests.Program_ComposesInfrastructure_WithoutManualEndpointRegistration`
- `NetRentManagerApiTests.Infrastructure.SwaggerConfigurationTests.Program_Configures_SwaggerUi_To_Use_The_Published_OpenApi_Document`
- `NetRentManagerApiTests.Infrastructure.SwaggerConfigurationTests.Program_Registers_SwaggerUi_Only_In_Development_Block`

Resultados:

- Build del proyecto API: correcto.
- Tests relevantes del backend: 3 correctos, 0 errores y 0 omitidos.
- Ejecución manual en `Development`: `https://localhost:7065/swagger` respondió HTTP 200 y mostró el título `Swagger UI`.
- Documento OpenAPI: `https://localhost:7065/openapi/v1.json` respondió HTTP 200.
- La API se detuvo tras la validación manual.
