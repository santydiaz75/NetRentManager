# Contrato HTTP: Listado Paginado de Propiedades

## Solicitud

```http
GET /api/properties?page={page}&pageSize={pageSize}
Accept: application/json
```

Los parámetros son opcionales:

- `page`: entero, predeterminado `1`, mínimo `1`.
- `pageSize`: entero, predeterminado `6`, rango `1..100`.

La ausencia de un parámetro aplica su valor predeterminado. Un valor no convertible, menor o igual que cero o `pageSize` mayor que 100 produce HTTP 400.

## Respuesta exitosa

**HTTP 200 OK**

```json
{
  "items": [
    {
      "id": "00000000-0000-0000-0000-000000000001",
      "title": "Apartamento central",
      "description": "Descripción de ejemplo",
      "address": "Calle Principal 1",
      "price": 1250.00,
      "status": "Available",
      "bedroomCount": 2,
      "bathroomCount": 1,
      "areaSquareMeters": 75.50,
      "imageUrl": "http://localhost:5023/assets/properties/1.png"
    }
  ],
  "page": 1,
  "pageSize": 6,
  "totalItems": 10,
  "totalPages": 2,
  "hasNext": true,
  "hasPrevious": false
}
```

Los nombres JSON se muestran en camelCase conforme a la configuración estándar de Minimal APIs.

## Respuesta de validación

**HTTP 400 Bad Request**

Se devuelve `ValidationProblemDetails` con errores por parámetro. Ejemplo conceptual:

```json
{
  "type": "https://httpstatuses.com/400",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "pageSize": ["pageSize debe estar entre 1 y 100."]
  }
}
```

## Respuesta por inconsistencia de datos

**HTTP 500 Internal Server Error**

Se devuelve `ProblemDetails` con un código estable para una propiedad cuya `ImageUrl` no permite generar una URL pública. No se exponen rutas físicas, valores sensibles ni stack trace.

## Orden y paginación

El servidor aplica:

1. `OrderBy(title ascendente)`.
2. `ThenBy(id ascendente)`.
3. `Count` sobre la consulta completa.
4. `Skip((page - 1) * pageSize)` y `Take(pageSize)`.

Una página fuera del rango existente no es un error y devuelve `items: []`.

## Disponibilidad de imágenes

Cada `imageUrl` exitosa debe ser absoluta, comenzar con `http://` o `https://`, usar el host de la solicitud y terminar bajo `/assets/properties/{fileName}`. El archivo debe ser servible desde `wwwroot/assets/properties`.
