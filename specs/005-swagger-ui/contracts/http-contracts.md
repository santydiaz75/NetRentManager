# Contrato HTTP: Publicación de Swagger UI

## Interfaz de documentación

```http
GET /swagger
Accept: text/html
```

**Comportamiento esperado**:

- En `Development`, la ruta devuelve la interfaz Swagger UI.
- Fuera de `Development`, la ruta no debe quedar expuesta por esta iniciativa.

## Documento OpenAPI consumido por la UI

La interfaz Swagger UI debe consumir el documento OpenAPI ya publicado por la aplicación. La ruta exacta debe permanecer alineada con la configuración real de `Program.cs`.

## Criterio de aceptación observable

- Abrir `/swagger` en desarrollo deja de producir HTTP 404.
- La interfaz muestra los endpoints publicados por el backend.
