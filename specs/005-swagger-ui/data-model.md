# Modelo de Datos: Swagger UI para el Backend

Esta iniciativa no introduce entidades persistentes ni cambios en PostgreSQL. Sus modelos son artefactos de documentación y runtime.

## OpenAPI v1

- **Representa**: contrato HTTP consumido por Swagger UI.
- **Origen**: `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json`, generado por la iniciativa `009-open-api`.
- **Identificador**: versión `v1` del documento.
- **Contenido relevante**: paths, métodos, parámetros, cuerpos, respuestas y schemas de los endpoints existentes.

## Swagger UI

- **Representa**: interfaz web de exploración y ejecución del contrato.
- **Ruta de entrada**: `/swagger`.
- **Documento consumido**: `/openapi/v1.json`.
- **Disponibilidad**: únicamente cuando `ASPNETCORE_ENVIRONMENT=Development`.
- **Estado fuera de Development**: ruta inexistente; HTTP `404 Not Found`.

## Relación

```text
OpenAPI v1 (documento estático) 1 ---- 1 Swagger UI (cliente visual)
```

Swagger UI no persiste datos, no modifica el contrato y no crea entidades de dominio.
