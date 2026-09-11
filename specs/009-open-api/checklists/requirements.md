# Lista de Calidad de la Especificación: OpenAPI v1 Público

**Propósito**: Validar que la especificación del contrato OpenAPI v1 sea completa, verificable, generada desde la implementación real y explícita sobre sus herramientas externas.
**Creado**: 2026-09-11
**Funcionalidad**: [spec.md](../spec.md)

## Calidad del contenido

- [x] No contiene placeholders de plantilla.
- [x] Está enfocada en el contrato HTTP público, su validación y la detección de drift.
- [x] Distingue claramente runtime de herramientas CLI offline.
- [x] Todas las secciones obligatorias están completas.

## Completitud funcional

- [x] Los cinco endpoints obligatorios están enumerados explícitamente.
- [x] Se define la normalización de constraints `{id:guid}` a `{id}`.
- [x] Cada operación debe describir método, ruta, parámetros, body y respuestas.
- [x] ProblemDetails, HttpValidationProblemDetails y PropertyStatus están definidos como schemas reutilizables.
- [x] Se exige `openapi/v1.json` versionado y `GET /openapi/v1.json` con JSON.
- [x] La generación desde implementación real y la prohibición de edición manual están explícitas.
- [x] Se define que `openapi/v1.json` se commitea como salida generada auditable y que el build debe detectar drift contra el archivo versionado.
- [x] Se distingue entre los tres artefactos canónicos de la iniciativa y los artefactos auxiliares trazables requeridos por esta feature.
- [x] Swagger UI, ReDoc UI y cualquier UI interactiva están excluidos explícitamente.

## Herramientas y reproducibilidad

- [x] Se exige Node.js 20+ y se documentan `node --version` y `npm --version`.
- [x] Se exige npx y `npx @redocly/cli --version`.
- [x] Se exige `@redocly/cli` como herramienta NPM offline.
- [x] Se exige NSwag.ConsoleCore en `dotnet-tools.json`.
- [x] Se documentan `dotnet tool restore`, `dotnet tool list` y `dotnet tool run nswag`.
- [x] Se exige `.redocly.yaml` con `security-defined: off` y `operation-4xx-response: warn`.
- [x] Se exige `support/scripts/generate-openapi-v1.ps1` con build, lint, NSwag y fallos explícitos.
- [x] Se exige salida reproducible en `artifacts/openapi-client-smoke/`.

## Drift y validación

- [x] Se exige una prueba xUnit basada en `EndpointDataSource`.
- [x] Se exige comparación de métodos y paths con normalización de constraints.
- [x] Se exige drift inducido que falle.
- [x] Se exige Redocly lint automatizado.
- [x] Se exige generación de cliente C# con NSwag como smoke test.
- [x] Se exige ausencia de UI en el inventario runtime.
- [x] La ausencia de herramientas obligatorias produce fallo y no un skip silencioso.
- [x] Existe una matriz explícita de status codes esperados por operación.
- [x] Los nombres de schemas compartidos tienen una estrategia estable y verificable.
- [x] Los criterios de éxito son medibles y verificables.

## Gobernanza y alcance

- [x] Se documentan dependencias con las specs 001 a 008.
- [x] Se mantiene el stack y la arquitectura del repositorio.
- [x] Todo artefacto nuevo queda sujeto a trazabilidad en `tasks.md`.
- [x] El alcance excluye cambios de negocio, frontend, persistencia y múltiples versiones.

## Notas

- La instalación actual de `@redocly/cli` y el script existente se tratarán como punto de partida y deberán corregirse o completarse durante la planificación/implementación.
- Al momento de especificar, `dotnet-tools.json` y `.redocly.yaml` aún no existen; ambos son artefactos obligatorios de esta iniciativa.
- La spec refina el significado operativo de gobernanza y versionado sin crear una iniciativa paralela; cualquier modificación de la constitución global requeriría una enmienda independiente.
- Checklist validado el 2026-09-11.
- La spec queda lista para `/speckit.clarify` o `/speckit.plan`.
