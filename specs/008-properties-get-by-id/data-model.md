# Modelo de Datos: Consulta de Propiedad por Id

## Persistencia utilizada

### `Property`

La entidad persistente existente se consulta sin seguimiento. No se modifican
columnas, relaciones, migraciones ni valores almacenados.

| Campo | Proyección pública | Regla |
|---|---|---|
| `Id` | `id` | GUID solicitado |
| `Title` | `title` | texto público |
| `Description` | `description` | texto público |
| `Address` | `address` | texto público |
| `Price` | `price` | decimal público |
| `Status` | `status` | texto (`Available`, `Rented`, `Maintenance`) |
| `BedroomCount` | `bedroomCount` | entero público |
| `BathroomCount` | `bathroomCount` | entero público |
| `AreaSquareMeters` | `areaSquareMeters` | decimal público |
| `ImageUrl` | `imageUrl` | `null` o URL absoluta calculada |

`CreatedAt` y `UpdatedAt` no forman parte del elemento público de 004 y no se
incluyen en esta respuesta.

## Request

### `GetPropertyByIdRequest`

El identificador se recibe como segmento `Guid` de la ruta. La restricción
`{id:guid}` produce 404 para valores no convertibles antes de ejecutar el handler.
El mapping recibe además el `HttpRequest` actual para calcular la URL pública.

## Proyección interna

### `PropertyByIdProjection`

Es una proyección interna con los nueve campos necesarios para el response. Se
obtiene mediante `AsNoTracking` y no se serializa directamente. La consulta usa
`SingleOrDefaultAsync` con el token de cancelación.

## Response

### `GetPropertyByIdResponse`

Contrato público JSON camelCase:

- `id`
- `title`
- `description`
- `address`
- `price`
- `status`
- `bedroomCount`
- `bathroomCount`
- `areaSquareMeters`
- `imageUrl`

No contiene `items`, metadatos de paginación, entidad EF, bytes ni rutas físicas.

## Reglas de `imageUrl`

1. `ImageUrl == null` produce `imageUrl: null`.
2. Una ruta no nula debe comenzar por `/assets/properties/`.
3. No puede contener `..`, `support`, una URI absoluta, una ruta física ni un
   nombre vacío, `.` o `..`.
4. Se extrae únicamente el nombre de archivo seguro.
5. Se construye una URL absoluta con `Request.Scheme`, `Request.Host` y el nombre
   escapado; el host puede incluir puerto.
6. Si alguna regla falla, el mapping devuelve `Error.Internal` y la respuesta es
   HTTP 500 sin elemento parcial.

## Estados del caso de uso

- **Solicitado**: se recibió un `Guid` válido.
- **No encontrado**: no existe una fila; respuesta HTTP 404.
- **Encontrado**: existe la proyección y se valida/mapea la imagen.
- **Respondido**: respuesta HTTP 200 con contrato completo.
- **Inconsistente**: la imagen no puede exponerse de forma segura; respuesta HTTP 500.
- **Cancelado**: se propaga la cancelación y no se crea respuesta de éxito.
