# Contratos HTTP: Actualización de Estado de Propiedad

## PATCH /api/properties/{id}/status

### Solicitud

- **Parámetro de ruta**: `id`, GUID de una propiedad existente.
- **Content-Type**: `application/json`.
- **Cuerpo permitido**:

```json
{
  "status": "Available"
}
```

- `status` acepta `Available`, `Rented` y `Maintenance` sin distinguir mayúsculas/minúsculas; el backend persiste el valor canónico.
- Las propiedades JSON adicionales producen `400 Bad Request`.
- No se admite `multipart/form-data`, `image`, upload ni `imageUrl` como entrada.

### Respuesta 200 OK

Devuelve la propiedad completa mediante el contrato explícito existente. Como mínimo contiene:

```json
{
  "id": "00000000-0000-0000-0000-000000000001",
  "title": "Apartamento",
  "description": "Descripción",
  "address": "Calle Principal 10",
  "price": 1200,
  "status": "Available",
  "bedroomCount": 2,
  "bathroomCount": 1,
  "areaSquareMeters": 70,
  "imageUrl": null
}
```

La única diferencia permitida respecto del registro anterior es `status`; `imageUrl` y los demás campos deben conservarse.

### Respuestas de error

- **400 Bad Request**: GUID inválido, JSON mal formado, `status` ausente, nulo, vacío, inválido o propiedades adicionales.
- **404 Not Found**: no existe una propiedad con el `id` solicitado.
- **500 Internal Server Error**: fallo inesperado de persistencia, devuelto como `ProblemDetails` sin detalles internos.

## Compatibilidad OpenAPI

El documento OpenAPI generado debe describir exactamente la operación PATCH, su parámetro GUID, el request JSON solo con `status` y las respuestas 200, 400, 404 y 500. No debe describir campos de actualización general ni upload de archivos.
