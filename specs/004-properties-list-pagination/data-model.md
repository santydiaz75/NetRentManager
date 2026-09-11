# Modelo de Datos: Listado Paginado de Propiedades

## Entidades existentes utilizadas

### `Property`

Entidad persistente existente de `Domain/Properties`.

| Campo | Uso en el listado | Regla |
|---|---|---|
| `Id` | Identidad y desempate | Se proyecta como `id`; `ThenBy(Id)` garantiza orden estable |
| `Title` | Orden primario | Se proyecta como `title` y se ordena ascendentemente |
| `Description` | Contrato de item | Se proyecta como `description` |
| `Address` | Contrato de item | Se proyecta como `address` |
| `Price` | Contrato de item | Se proyecta como `price` |
| `Status` | Contrato de item | Se proyecta como `status` |
| `BedroomCount` | Contrato de item | Se proyecta como `bedroomCount` |
| `BathroomCount` | Contrato de item | Se proyecta como `bathroomCount` |
| `AreaSquareMeters` | Contrato de item | Se proyecta como `areaSquareMeters` |
| `ImageUrl` | Fuente de la URL pública | Debe ser ruta pública relativa válida, nunca ruta física |

No se modifican las entidades ni el esquema persistente de la spec 003.

## Contratos del caso de uso

### `ListPropertiesRequest`

| Campo | Tipo conceptual | Predeterminado | Validación |
|---|---|---:|---|
| `page` | entero | 1 | mayor o igual que 1 |
| `pageSize` | entero | 6 | entre 1 y 100, inclusive |

Los errores de conversión de query string y las reglas de FluentValidation producen HTTP 400 mediante el filtro global.

### `PropertyListItem`

Contrato de salida por propiedad:

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

`imageUrl` se construye durante el mapping como URL absoluta con el esquema y host del request y la ruta pública `/assets/properties/{fileName}`.

### `PagedPropertiesResponse`

Contrato de salida del endpoint:

| Campo | Tipo conceptual | Regla |
|---|---|---|
| `items` | colección de `PropertyListItem` | hasta `pageSize` elementos |
| `page` | entero | valor validado de la solicitud |
| `pageSize` | entero | valor validado de la solicitud |
| `totalItems` | entero | total antes de `Skip` y `Take` |
| `totalPages` | entero | cero si no hay elementos; si no, techo de `totalItems / pageSize` |
| `hasNext` | booleano | `page < totalPages` |
| `hasPrevious` | booleano | `page > 1` y `totalItems > 0` |

Una página posterior a la última es válida y devuelve `items` vacío con metadatos coherentes.

## Error de integridad de imagen

Si `ImageUrl` está vacía, contiene una ruta física, contiene `support`, no tiene un nombre de archivo seguro o no puede formar una URL absoluta pública, el handler devuelve un error interno de código estable. El mapper común lo representa como `ProblemDetails` HTTP 500 sin exponer rutas ni excepciones.

## Relaciones y persistencia

- El caso de uso lee `AppDbContext.Properties`.
- No agrega tablas, migraciones, columnas ni relaciones.
- La consulta usa `AsNoTracking` y proyección directa.
- La ordenación se ejecuta en la base de datos antes de paginar.
