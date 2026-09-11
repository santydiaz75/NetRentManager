# Contratos HTTP: Swagger UI para el Backend

## GET /swagger

### Entorno Development

- **Respuesta esperada**: `200 OK` o redirección equivalente a `/swagger/index.html`.
- **Content-Type**: `text/html` para la página final.
- **Contenido mínimo**: HTML de Swagger UI con una referencia al documento `/openapi/v1.json`.

### Entornos distintos de Development

- **Respuesta esperada**: `404 Not Found`.
- **Cuerpo**: no debe contener HTML ni configuración de Swagger UI.

## GET /swagger/index.html

### Entorno Development

- **Respuesta esperada**: `200 OK`.
- **Content-Type**: `text/html`.
- **Contenido**: página principal de Swagger UI.

### Entornos distintos de Development

- **Respuesta esperada**: `404 Not Found`.

## Recursos de Swagger UI

Los recursos JavaScript, CSS y auxiliares necesarios para la interfaz deben responder `200` únicamente en Development. El nombre concreto del recurso debe verificarse contra la versión instalada de `Swashbuckle.AspNetCore` durante la implementación.

Fuera de Development, cualquier recurso exclusivo de Swagger UI debe responder `404`.

## Documento referenciado

- **Ruta**: `GET /openapi/v1.json`.
- **Fuente**: documento estático de `009-open-api`.
- **Regla**: Swagger UI puede consumirlo, pero no debe generar ni publicar una copia alternativa del contrato.
