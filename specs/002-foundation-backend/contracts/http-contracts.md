# Contratos HTTP: Fundación Interna del Backend

## `GET /health`

- **Propósito**: comprobar que el descubrimiento y el mapeo de slices de
  infraestructura están disponibles.
- **Entrada**: ninguna.
- **Respuesta exitosa**: HTTP `200` con un payload estable que indique estado
  saludable.
- **Persistencia**: no consulta base de datos.
- **Registro**: la ruta se declara dentro de `HealthSlice`; `Program.cs` no la
  registra individualmente.

## Validación de requests futuros

- **Sin validator**: el delegate del endpoint continúa y conserva su respuesta.
- **Validator válido**: el delegate continúa.
- **Validator inválido**: HTTP `400` con `ValidationProblemDetails` y todos los
  errores agrupados por propiedad.
- **Content type**: formato estándar de Problem Details para respuestas JSON.

## Errores esperados

El mapper centralizado de `Result` usa `ProblemDetails` para errores no asociados a
validación:

| Tipo de error | HTTP |
|---|---:|
| `NotFound` | 404 |
| `Conflict` | 409 |
| `Validation` | 400 |
| `Forbidden` | 403 |

El payload conserva el código y los detalles seguros definidos por el resultado. No
se exponen stack traces ni mensajes de excepciones internas.
