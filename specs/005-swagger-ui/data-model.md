# Modelo de Datos: Interfaz Swagger UI del Backend

## Entidades de dominio

La iniciativa no introduce entidades nuevas ni modifica entidades existentes.

## Elementos de configuración involucrados

### Publicación OpenAPI existente

| Elemento | Ubicación | Rol |
|---|---|---|
| Documento OpenAPI | `Program.cs` mediante `AddOpenApi()` y `MapOpenApi()` | Fuente del contrato consumido por la UI |
| Entorno de ejecución | `app.Environment.IsDevelopment()` | Gate de exposición para OpenAPI y Swagger UI |

### Configuración de Swagger UI

| Elemento | Tipo conceptual | Regla |
|---|---|---|
| Ruta UI | cadena | debe ser `/swagger` |
| Documento OpenAPI asociado | URL relativa | debe apuntar al documento realmente publicado por la aplicación |
| Dependencia de UI | paquete NuGet | debe ser compatible con .NET 10 y Minimal APIs |

## Persistencia

- No hay tablas, migraciones ni cambios en la base de datos.
- No hay cambios en `DbContext` ni en configuraciones EF Core.
