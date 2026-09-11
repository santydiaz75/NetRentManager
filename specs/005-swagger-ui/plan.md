# Plan de Implementación: Interfaz Swagger UI del Backend

**Rama**: `005-swagger-ui` | **Fecha**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/005-swagger-ui/spec.md`

## Resumen

Agregar Swagger UI al proyecto `NetRentManagerApi` para que la interfaz interactiva quede disponible en `/swagger` cuando la aplicación se ejecute en `Development`, reutilizando el documento OpenAPI ya publicado por el backend y manteniendo `Program.cs` como punto de composición.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y SDK `10.0.400` fijado por `global.json`.

**Dependencias principales**: ASP.NET Core Minimal APIs, `Microsoft.AspNetCore.OpenApi`, y una dependencia de Swagger UI compatible con el stack actual.

**Persistencia**: N/A. La iniciativa no modifica `AppDbContext`, entidades ni migraciones.

**Pruebas**: xUnit. Se añadirán pruebas unitarias de composición para verificar el registro de Swagger UI y la conservación del documento OpenAPI.

**Plataforma objetivo**: ASP.NET Core ejecutado localmente desde `app/backend/src/NetRentManagerApi`.

**Tipo de proyecto**: Web API Minimal API con arquitectura Vertical Slice.

**Objetivos de rendimiento**: sin impacto apreciable en runtime fuera del arranque y solo en `Development`.

**Restricciones**: no controllers, no cambios de dominio, no cambios persistentes, no exposición de Swagger UI fuera de `Development`, y sin sustituir el mecanismo actual de `AddOpenApi()` si no es necesario.

**Escala/Alcance**: un cambio de infraestructura localizado en composición del backend, dependencias del proyecto y pruebas del backend.

## Investigación y decisiones

Las decisiones técnicas se consolidan en [research.md](research.md).

## Comprobación de Constitución

- **Aprobado**: la iniciativa vive en `specs/005-swagger-ui/` y respeta el flujo Spec-Driven.
- **Aprobado**: el backend sigue usando Minimal APIs y `Program.cs` solo compone infraestructura.
- **Aprobado**: no se agregan controllers, capas paralelas, persistencia ni cambios de dominio.
- **Aprobado**: las pruebas siguen siendo unitarias, como exige la constitución.
- **Aprobado**: cada cambio de código y validación quedará trazado en `tasks.md`.

No existen violaciones constitucionales que justificar.

## Estructura del proyecto

### Documentación de la funcionalidad

```text
specs/005-swagger-ui/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── http-contracts.md
└── tasks.md
```

### Código fuente

```text
app/backend/src/NetRentManagerApi/
├── NetRentManagerApi.csproj
└── Program.cs

app/backend/tests/NetRentManagerApiTests/
└── Infrastructure/
	├── ProgramCompositionTests.cs
	└── SwaggerConfigurationTests.cs
```

**Decisión estructural**: la composición seguirá centralizada en `Program.cs`. La iniciativa no crea slices ni endpoints nuevos; solo publica middleware/UI sobre el documento OpenAPI existente.

## Diseño técnico

### Flujo de composición

1. `NetRentManagerApi.csproj` incorpora la dependencia necesaria para servir Swagger UI.
2. `Program.cs` mantiene `AddOpenApi()` y, en `Development`, registra el documento OpenAPI y la interfaz Swagger UI.
3. Swagger UI se configura para consumir el documento OpenAPI servido por la propia aplicación.
4. El resto de la canalización (`UseExceptionHandler`, `UseStaticFiles`, `MapSliceEndpoints`, `MigrateAsync`) permanece sin cambios funcionales ajenos al alcance.

## Estrategia de pruebas

Las pruebas verificarán por inspección de código que `Program.cs` sigue componiendo infraestructura y que contiene la configuración esperada para Swagger UI dentro de la condición de entorno de desarrollo. Si la API usada lo permite sin host real, se añadirá una prueba en memoria de descubrimiento de la ruta configurada.

## Seguimiento de complejidad

| Violación | Necesidad | Alternativa simple descartada |
|-----------|-----------|-------------------------------|
| N/A | N/A | N/A |
