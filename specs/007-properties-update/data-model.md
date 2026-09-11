# Modelo de Datos: Actualización de Propiedades

## Entidad persistente existente

### `Property`

La entidad de `Domain/Properties` se actualiza en memoria y se persiste mediante
`AppDbContext`; no se crea una entidad paralela.

| Campo | Fuente | Regla de actualización |
|---|---|---|
| `Id` | segmento de ruta | `Guid` existente; no se modifica |
| `Title` | formulario | obligatorio, no vacío, máximo 200 |
| `Description` | formulario | obligatorio, no vacío, máximo 2000 |
| `Address` | formulario | obligatorio, no vacío, máximo 300 |
| `Price` | formulario | obligatorio, mayor o igual que cero |
| `Status` | formulario | obligatorio; `Available`, `Rented` o `Maintenance` |
| `BedroomCount` | formulario | obligatorio, entero no negativo |
| `BathroomCount` | formulario | obligatorio, entero no negativo |
| `AreaSquareMeters` | formulario | obligatorio, mayor que cero |
| `ImageUrl` | upload opcional | conserva el valor anterior sin `image`; nueva URL con imagen válida |
| `CreatedAt` | persistencia | no se modifica |
| `UpdatedAt` | servidor | UTC de la actualización confirmada |

## Request de formulario

### `UpdatePropertyRequest`

Formato `multipart/form-data`. Todos los campos siguientes son obligatorios por
la semántica de reemplazo completo de `PUT`:

| Campo | Tipo | Obligatorio | Regla |
|---|---|---:|---|
| `title` | texto | sí | no vacío, máximo 200 |
| `description` | texto | sí | no vacío, máximo 2000 |
| `address` | texto | sí | no vacío, máximo 300 |
| `price` | decimal | sí | mayor o igual que cero |
| `status` | texto/enum | sí | solo tres estados |
| `bedroomCount` | entero | sí | mayor o igual que cero |
| `bathroomCount` | entero | sí | mayor o igual que cero |
| `areaSquareMeters` | decimal | sí | mayor que cero |
| `image` | `IFormFile?` | no | PNG/JPG real de 1 byte a 5 MiB |

## Resultado de imagen

### `PropertyImageUpdate`

Representa el estado temporal de un reemplazo:

- `PreviousImageUrl`: URL anterior, posiblemente `null`.
- `CreatedFilePath`: ruta física de la nueva imagen escrita por la operación.
- `NewImageUrl`: ruta relativa `/assets/properties/{guid}.{ext}`.
- `PreviousPropertyValues`: valores necesarios para restaurar la entidad tracked
  si falla la persistencia o la operación se cancela antes del commit.

La ruta física solo vive durante la coordinación del handler; nunca se serializa.

## Response

### `UpdatePropertyResponse`

Incluye `id`, todos los campos persistidos, `status` como texto, `imageUrl` nullable,
`createdAt` y `updatedAt`. No incluye bytes, rutas físicas, nombres temporales ni
información del servidor.

## Transiciones

1. `Encontrada`: el id existe y todavía no se ha mutado la entidad.
2. `Validada`: campos y, si existe, upload superan sus reglas.
3. `Staged`: el archivo nuevo existe en el destino runtime y la entidad conserva
   los valores anteriores disponibles para compensación.
4. `Confirmada`: `SaveChangesAsync` terminó correctamente y la nueva URL quedó
   persistida.
5. `Limpieza pendiente`: se intenta eliminar la imagen anterior después del commit.
   Un fallo aquí se registra y no revierte `Confirmada`.

Cualquier fallo entre `Validada` y `Confirmada` elimina `CreatedFilePath`, restaura
la entidad tracked y devuelve el error correspondiente.

## Error tipado

Se agregan dos tipos a `ErrorType`:

- `UnsupportedMediaType`: HTTP 415 para archivo vacío, formato declarado no
  permitido o contenido real incompatible.
- `PayloadTooLarge`: HTTP 413 para tamaño mayor a `5 * 1024 * 1024` bytes.

Los errores de campos y formularios mal formados continúan usando `Validation`
HTTP 400; inexistencia usa `NotFound` HTTP 404 e I/O/persistencia usa `Internal`
HTTP 500.
