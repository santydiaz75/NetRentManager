# Modelo de Datos: Alta de Propiedades

## Entidad persistente existente

### `Property`

La entidad existente de `Domain/Properties` se mantiene como raíz de persistencia.

| Campo | Entrada | Regla |
|---|---|---|
| `Id` | generado por servidor | `Guid` único por creación |
| `Title` | formulario | obligatorio, máximo definido por configuración existente; no único |
| `Description` | formulario | obligatorio, máximo definido por configuración existente |
| `Address` | formulario | obligatorio, máximo definido por configuración existente |
| `Price` | formulario | obligatorio, no negativo |
| `Status` | formulario | obligatorio; `Available`, `Rented` o `Maintenance` |
| `BedroomCount` | formulario | obligatorio, entero no negativo |
| `BathroomCount` | formulario | obligatorio, entero no negativo |
| `AreaSquareMeters` | formulario | obligatorio, positivo |
| `ImageUrl` | derivado del upload | nullable; `null` sin imagen, `/assets/properties/{guid}.{ext}` con imagen |
| `CreatedAt` | generado por servidor | UTC/default existente |
| `UpdatedAt` | no se recibe | `null` en la creación inicial |

## Request de formulario

### `CreatePropertyRequest`

Formato `multipart/form-data`:

| Campo | Tipo | Obligatorio | Regla |
|---|---|---:|---|
| `title` | texto | sí | no vacío y longitud existente |
| `description` | texto | sí | no vacío y longitud existente |
| `address` | texto | sí | no vacío y longitud existente |
| `price` | decimal | sí | mayor o igual que cero |
| `status` | texto/enum | sí | solo tres estados permitidos |
| `bedroomCount` | entero | sí | mayor o igual que cero |
| `bathroomCount` | entero | sí | mayor o igual que cero |
| `areaSquareMeters` | decimal | sí | mayor que cero |
| `image` | `IFormFile?` | no | PNG/JPG/JPEG real, 1 byte a 5 MiB |

## Upload validado

- `ImageUpload` se considera ausente cuando `image` es `null`.
- El tamaño máximo es `5 * 1024 * 1024` bytes; el tamaño exacto se acepta.
- El contenido se identifica por magic bytes; la extensión final se normaliza a `.png` o `.jpg`.
- El nombre físico nunca usa directamente `FileName` del cliente.
- La escritura usa el `CancellationToken` de la petición.

## Response

### `CreatePropertyResponse`

Incluye los campos persistidos del recurso creado, `id`, `status` textual e
`imageUrl` nullable. La respuesta debe serializar camelCase y no incluir contenido
binario ni rutas físicas.

## Persistencia y migración

- `Property.ImageUrl` cambia de `string` requerido a `string?` opcional.
- `PropertyConfiguration` cambia `image_url` a nullable.
- La iniciativa genera una migración única para esta alteración, sin resembrar ni
  modificar otras columnas.
- Las propiedades existentes con rutas de imagen siguen siendo válidas.

## Efectos compensatorios

- Antes de `SaveChangesAsync`, el archivo puede existir en el destino público.
- Si falla `SaveChangesAsync`, falla la copia, se cancela la petición o falla una
  operación posterior, se elimina el archivo generado si pertenece a la operación.
- Si no hay imagen, no se crea archivo y `ImageUrl` permanece `null`.
