# Contrato HTTP: Actualización de Propiedades

## Solicitud

```http
PUT /api/properties/{id}
Content-Type: multipart/form-data
```

`{id}` debe ser un `Guid` válido. El formulario reemplaza todos los campos de
negocio y debe incluir:

- `title`
- `description`
- `address`
- `price`
- `status`
- `bedroomCount`
- `bathroomCount`
- `areaSquareMeters`

`status` debe ser `Available`, `Rented` o `Maintenance`. La parte `image` es
opcional. Si no aparece, `imageUrl` conserva exactamente su valor anterior.

Cuando aparece, `image` debe ser PNG o JPG/JPEG real, no vacío y de hasta
`5 * 1024 * 1024` bytes inclusive.

## Respuesta exitosa

**HTTP 200 OK**

```json
{
  "id": "00000000-0000-0000-0000-000000000001",
  "title": "Apartamento actualizado",
  "description": "Descripción actualizada",
  "address": "Calle Principal 1",
  "price": 1350.00,
  "status": "Available",
  "bedroomCount": 2,
  "bathroomCount": 1,
  "areaSquareMeters": 75.50,
  "imageUrl": "/assets/properties/7c1a8d8e4e9b4fd2a8a2d6c9d6d7a111.jpg",
  "createdAt": "2026-09-11T12:00:00Z",
  "updatedAt": "2026-09-11T13:00:00Z"
}
```

Si no existía imagen y no se envía `image`, `imageUrl` permanece `null`.

## Errores

### HTTP 400 Bad Request

`ValidationProblemDetails` para campos faltantes o inválidos, `status` fuera del
catálogo y formularios multipart mal formados. La propiedad permanece sin cambios.

### HTTP 404 Not Found

`ProblemDetails` cuando no existe la propiedad indicada. No se modifica la base de
datos ni se escribe ningún archivo.

### HTTP 413 Payload Too Large

`ProblemDetails` para una imagen mayor de 5 MiB. No se modifica la propiedad ni se
publica el archivo.

### HTTP 415 Unsupported Media Type

`ProblemDetails` para archivo vacío, extensión/MIME no permitido o contenido real
que no coincide con PNG/JPEG. No se modifica la propiedad ni se conserva un archivo
nuevo parcial.

### HTTP 500 Internal Server Error

`ProblemDetails` para fallos inesperados de almacenamiento, persistencia o
compensación previa al commit. No expone stack traces, rutas físicas, secretos ni
bytes. El archivo nuevo se elimina y se restauran los valores anteriores cuando
la operación no fue confirmada.

Un fallo al eliminar la imagen anterior después de un commit correcto se registra
con logging estructurado y no cambia la respuesta HTTP 200 ni la nueva `imageUrl`.

## Concurrencia

No se expone token de versión ni se devuelve HTTP 409. Si dos actualizaciones son
válidas, prevalece la última actualización confirmada por la base de datos.
