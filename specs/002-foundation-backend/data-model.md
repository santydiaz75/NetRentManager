# Modelo de Datos: Fundación Interna del Backend

Esta iniciativa no introduce entidades de dominio, tablas, persistencia ni
migraciones. El modelo siguiente describe únicamente contratos técnicos en memoria.

## `ISlice`

- **Propósito**: representar un endpoint Minimal API descubrible.
- **Operación**: `AddEndpoint(IEndpointRouteBuilder app)`.
- **Ciclo de vida**: singleton; debe ser público, concreto, no abstracto y stateless.
- **Relaciones**: `RegisterSlices` registra implementaciones; `MapSliceEndpoints`
  las resuelve y ejecuta.

## `IHandler`

- **Propósito**: marker interface para handlers de casos de uso futuros.
- **Operaciones**: ninguna requerida por esta spec.
- **Ciclo de vida**: scoped para permitir dependencias de request.
- **Relaciones**: `RegisterHandlers` registra implementaciones del assembly indicado.
- **Restricción**: no se crea ningún handler de negocio en esta iniciativa.

## `Result<T>` y `Error`

- **Propósito**: representar éxito o error esperado antes de convertirlo a HTTP.
- **Error mínimo**: código estable, tipo, mensaje seguro y status code asociado por
  el mapper; los detalles adicionales son opcionales.
- **Relaciones**: el mapper centralizado produce `ProblemDetails` o
  `ValidationProblemDetails` según el tipo de error.
- **Transiciones**: `Success` permanece resultado exitoso; `Error` se transforma en
  una respuesta HTTP sin lanzar excepción.

## `ValidationProblemDetails`

- **Propósito**: contrato HTTP para errores de validación de entrada.
- **Estado HTTP**: `400`.
- **Contenido**: diccionario `Errors` agrupado por nombre de propiedad, conservando
  todos los mensajes producidos por FluentValidation.
- **Relaciones**: lo produce `ValidationFilterFactory` cuando existe validator y la
  validación es inválida.

## `HealthResponse`

- **Propósito**: resultado estable de la única sonda de infraestructura.
- **Persistencia**: ninguna.
- **Endpoint**: `GET /health`.
- **Restricción**: no representa una entidad ni una feature de negocio.
