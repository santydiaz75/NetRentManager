# Contrato HTTP: Alta de Propiedades

## Solicitud

```http
POST /api/properties
Content-Type: multipart/form-data
```

Partes requeridas:

- `title`
- `description`
- `address`
- `price`
- `status`
- `bedroomCount`
- `bathroomCount`
- `areaSquareMeters`

Parte opcional:

- `image`: archivo PNG o JPG/JPEG de entre 1 byte y 5 MiB inclusive.

`status` debe enviarse explícitamente como `Available`, `Rented` o `Maintenance`.

## Respuesta de creación

**HTTP 201 Created**

```json
{
  "id": "00000000-0000-0000-0000-000000000001",
  "title": "Apartamento central",
  "description": "Descripción",
  "address": "Calle Principal 1",
  "price": 1250.00,
  "status": "Available",
  "bedroomCount": 2,
  "bathroomCount": 1,
  "areaSquareMeters": 75.50,
  "imageUrl": "/assets/properties/00000000-0000-0000-0000-000000000001.jpg",
  "createdAt": "2026-09-11T12:00:00Z"
}
```

Cuando no se envía imagen:

```json
{
  "id": "00000000-0000-0000-0000-000000000002",
  "status": "Available",
  "imageUrl": null
}
```

La respuesta incluye `Location: /api/properties/{id}` cuando la infraestructura lo permita.

## Errores de validación

**HTTP 400 Bad Request**

Se devuelve `ValidationProblemDetails` agrupado por campo para:

- campos obligatorios ausentes o inválidos;
- `status` ausente o fuera del catálogo;
- imagen vacía;
- extensión o `Content-Type` no permitido;
- contenido que no coincide con PNG/JPEG;
- tamaño superior a 5 MiB;
- formulario multipart mal formado.

## Error interno

**HTTP 500 Internal Server Error**

Se devuelve `ProblemDetails` con código estable para fallos de almacenamiento o
persistencia. No se exponen stack traces, rutas físicas, secretos ni contenido del
archivo. El archivo generado se elimina y la propiedad no queda persistida de forma
parcial.

## Cancelación

La cancelación del request interrumpe lectura, escritura y persistencia, y ejecuta
la limpieza compensatoria del archivo generado.
