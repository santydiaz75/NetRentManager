# Modelo de Artefactos: OpenAPI v1 Público

## Documento generado

### `openapi/v1.json`

- Ubicación fuente/versionada: `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json`.
- Versión contractual: OpenAPI v1.
- Fuente: endpoints registrados por la implementación real de Minimal APIs durante build.
- Salida runtime: archivo estático servido por `/openapi/v1.json`.
- No se genera durante startup ni bajo demanda.
- Se commitea en Git como salida generada auditable; nunca se edita manualmente.
- Una diferencia entre la salida regenerada y el archivo commiteado invalida la
  validación de la iniciativa.

## Inventario de endpoints

### `EndpointInventory`

Cada entrada contiene:

- método HTTP;
- patrón de ruta;
- ruta normalizada sin constraints de ASP.NET Core;
- exclusión explícita del documento estático para evitar auto-comparación;
- referencia al endpoint runtime usado para detectar drift.

Inventario esperado:

| Método | Ruta normalizada |
|---|---|
| `GET` | `/health` |
| `GET` | `/api/properties` |
| `POST` | `/api/properties` |
| `GET` | `/api/properties/{id}` |
| `PUT` | `/api/properties/{id}` |

## Schemas compartidos

### `PropertyStatus`

Enum público con `Available`, `Rented` y `Maintenance`.

### `ProblemDetails`

Error HTTP común con `type`, `title`, `status`, `detail`, `instance` y extensiones
compatibles con el mapper existente.

### `HttpValidationProblemDetails`

Error de validación con `status`, `title`, `detail`, `errors` por campo y extensiones
compatibles con `ValidationProblemDetails`.

Los tres schemas deben vivir en `components/schemas` y las operaciones deben usar
`$ref` cuando el transformador OpenAPI los exponga. Sus nombres deben ser
exactamente `PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails`;
las variantes duplicadas no están permitidas.

## Configuración y herramientas

- `.redocly.yaml`: reglas para API pública sin seguridad.
- `dotnet-tools.json`: manifest local con `NSwag.ConsoleCore`.
- `package.json`/`package-lock.json`: dependencia de desarrollo `@redocly/cli`.
- `support/scripts/openapi-v1.nswag.json`: entrada NSwag que consume el JSON y
  escribe el cliente smoke.
- `support/scripts/generate-openapi-v1.ps1`: orquestador build/lint/NSwag.

Las herramientas obligatorias se consideran prerequisito de validación. Si falta
Node.js, npx, Redocly, NSwag o `.redocly.yaml`, la validación falla y no se marca
como omitida.

## Salidas de validación

### `OpenApiClientSmoke`

Cliente C# generado en `artifacts/openapi-client-smoke/`. Es una salida de
validación, no una dependencia ni ruta runtime.

### `DriftReport`

Resultado de pruebas xUnit que enumera métodos/rutas faltantes, sobrantes o
incompatibles entre `EndpointDataSource` y el JSON generado.
