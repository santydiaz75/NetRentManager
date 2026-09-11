# Guía de Validación: Listado Paginado de Propiedades

## Prerrequisitos

- SDK definido por `global.json` (`10.0.400`).
- PostgreSQL y la configuración de conexión existente para arrancar la API con datos semilla.
- PowerShell desde la raíz del repositorio.
- Solución `app/NetRentManager.sln`.
- Persistencia y assets de `003-properties-persistence-seeding` ya implementados.

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

1. Valores predeterminados `page = 1` y `pageSize = 6`.
2. Rechazo de `page <= 0`, `pageSize <= 0`, `pageSize > 100` y valores no convertibles.
3. Orden `title` ascendente con desempate por `id` ascendente.
4. Cálculo de `totalItems`, `totalPages`, `hasNext` y `hasPrevious`.
5. Propiedad `items` y todos los campos obligatorios del item.
6. Consulta proyectada sin tracking y propagación de cancelación.
7. Construcción de URL absoluta bajo `/assets/properties`.
8. HTTP 500 con `ProblemDetails` ante `ImageUrl` inválida.
9. Auto-descubrimiento del slice y del handler sin cambios manuales en `Program.cs`.

## Prueba manual del endpoint

Iniciar la API con la configuración local disponible y ejecutar desde `app/backend/src/NetRentManagerApi/NetRentManagerApi.http`:

```http
GET {{NetRentManagerApi_HostAddress}}/api/properties
Accept: application/json

###

GET {{NetRentManagerApi_HostAddress}}/api/properties?page=1&pageSize=6
Accept: application/json
```

La respuesta esperada es HTTP 200 con `items`, como máximo seis elementos en la primera solicitud y URLs absolutas de imagen. La segunda solicitud debe conservar `page = 1` y `pageSize = 6` en los metadatos.

También deben comprobarse manualmente:

```http
GET {{NetRentManagerApi_HostAddress}}/api/properties?page=2&pageSize=6
Accept: application/json

###

GET {{NetRentManagerApi_HostAddress}}/api/properties?pageSize=101
Accept: application/json
```

La primera devuelve la segunda página o una colección vacía con metadatos coherentes; la segunda devuelve HTTP 400.

## Validación de imagen pública

Para una API ejecutándose en `http://localhost:5023`, cada elemento debe exponer una URL como:

```text
http://localhost:5023/assets/properties/1.png
```

La URL debe responder desde el mismo proceso y no debe contener `support`, una ruta física ni ser relativa.

## Evidencia de validación

Durante la implementación, registrar en esta sección la fecha, comandos ejecutados, cantidad de pruebas, respuestas HTTP observadas y comprobación de que los assets públicos se sirven desde `wwwroot/assets/properties`.

## Evidencia de validación

Fecha: 2026-09-11

Comandos ejecutados:

```powershell
dotnet build .\app\backend\src\NetRentManagerApi\NetRentManagerApi.csproj --no-restore
dotnet build .\app\NetRentManager.sln --no-restore
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
```

Resultados:

- Build del proyecto API: correcto, con tres advertencias preexistentes de compatibilidad de paquetes con `net10.0`.
- Build de la solución: correcto.
- Tests backend: 56 correctos, 0 errores y 0 omitidos.
- Auto-descubrimiento: se mapearon `HealthSlice` y `ListPropertiesSlice` sin registro manual adicional.
- `GET http://localhost:5023/api/properties`: HTTP 200, `items` con 6 elementos y URL `http://localhost:5023/assets/properties/1.png`.
- `GET http://localhost:5023/api/properties?page=1&pageSize=6`: HTTP 200.
- `GET http://localhost:5023/api/properties?pageSize=101`: HTTP 400 con error de validación para `PageSize`.
- `GET http://localhost:5023/assets/properties/1.png`: HTTP 200, archivo servido con 2,513,225 bytes.
- La consulta observada usa `COUNT`, proyección de columnas, `ORDER BY title, id`, `LIMIT` y `OFFSET`.
- La API fue detenida después de la prueba manual; no queda un proceso local ejecutándose.
