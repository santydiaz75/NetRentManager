# Guía de Validación: Consulta de Propiedad por Id

## Prerrequisitos

- SDK definido por `global.json` (`10.0.400`).
- PostgreSQL disponible mediante la configuración existente.
- Backend `app/backend/src/NetRentManagerApi` compilado.
- Una propiedad existente con imagen y otra con `imageUrl: null`.
- La ruta pública `/assets/properties/` servida por la API.

## Preparación

```powershell
dotnet restore .\app\NetRentManager.sln
dotnet build .\app\NetRentManager.sln --no-restore
```

## Pruebas automatizadas

```powershell
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
```

La suite debe cubrir:

1. Consulta de una propiedad existente con HTTP 200 y los nueve campos públicos.
2. `imageUrl` absoluta con `http`, `https`, host y puerto variables.
3. Propiedad sin imagen con HTTP 200 e `imageUrl: null`.
4. GUID válido inexistente con HTTP 404 y segmento no GUID con HTTP 404.
5. Imagen persistida inválida con HTTP 500 sin ruta interna en la respuesta.
6. Ausencia de `items` y metadatos de paginación.
7. Cancelación propagada y auto-descubrimiento de slice/handler.

## Ejemplos manuales

Consultar una propiedad con imagen:

```powershell
curl.exe -i http://localhost:5065/api/properties/{id-con-imagen}
```

Esperado: HTTP 200 y `imageUrl` con formato
`http://localhost:5065/assets/properties/{fileName}`.

Consultar una propiedad sin imagen:

```powershell
curl.exe -i http://localhost:5065/api/properties/{id-sin-imagen}
```

Esperado: HTTP 200, todos los campos públicos e `imageUrl: null`.

Consultar un GUID inexistente:

```powershell
curl.exe -i http://localhost:5065/api/properties/00000000-0000-0000-0000-000000000099
```

Esperado: HTTP 404 con `ProblemDetails`.

Consultar un id inválido:

```powershell
curl.exe -i http://localhost:5065/api/properties/no-es-guid
```

Esperado: HTTP 404 por no coincidencia de la restricción `Guid`.

## Medición p95

Ejecutar al menos 100 consultas válidas alternando una propiedad con imagen y otra
sin imagen. Registrar el tiempo total de cada respuesta, ordenar las muestras y
tomar la posición `ceil(0.95 * N)`. El criterio pasa cuando el p95 es menor de
250 ms y todas las respuestas son HTTP 200.

Conservar fecha, SDK, configuración, número de solicitudes, ids usados, p95,
latencia máxima y cualquier error observado.

## Evidencia de validación

Durante la implementación registrar:

- build y cantidad de pruebas correctas;
- respuestas 200 con imagen y sin imagen;
- URL absoluta con esquema, host y puerto actuales;
- respuestas 404 para GUID inexistente e id inválido;
- respuesta 500 para imagen persistida inconsistente;
- ausencia de metadatos de paginación;
- cancelación y auto-descubrimiento;
- resultado de la muestra p95;
- confirmación de que no se modificó persistencia ni se expusieron entidades EF.

## Evidencia de validación

Fecha: 2026-09-11

Comandos ejecutados:

```powershell
dotnet build .\app\NetRentManager.sln --no-restore
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
dotnet run --project .\app\backend\src\NetRentManagerApi\NetRentManagerApi.csproj --no-build --urls http://localhost:5078
```

Resultados:

- Build de la solución: correcto; backend y frontend compilados con las advertencias existentes de compatibilidad de paquetes con `net10.0`.
- Suite backend: 107 pruebas correctas, 0 errores y 0 omitidas.
- Suite focalizada `GetPropertyById`: 15 pruebas correctas, 0 errores y 0 omitidas.
- Auto-descubrimiento: la API mapeó 5 slices, incluyendo `GetPropertyByIdSlice`.
- Consulta con imagen: HTTP 200 y `imageUrl` absoluta bajo `http://localhost:5078/assets/properties/`.
- Consulta sin imagen: HTTP 200 y `imageUrl: null`.
- GUID inexistente: HTTP 404 con `properties.not_found`.
- Segmento `no-es-guid`: HTTP 404 por restricción `{id:guid}`.
- URL con `https` y host/puerto variables: cubierta por pruebas de mapping con `https://api.example.test:8443`.
- Imágenes inseguras (`support`, ruta física, traversal y URI absoluta): HTTP lógico 500 cubierto por pruebas de mapping.
- Muestra p95: 100 consultas válidas, 100 respuestas HTTP 200, p95 de 176.37 ms y máximo de 306.13 ms.
- La API se detuvo después de la validación manual; no queda proceso local ejecutándose.
- No se agregaron migraciones, paginación, filtros, entidades EF en respuestas ni cambios de frontend.
