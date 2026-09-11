# Investigación: Interfaz Swagger UI del Backend

## Decisión 1: Mantener OpenAPI nativo y añadir solo la UI

- **Decisión**: Conservar `AddOpenApi()` y `MapOpenApi()` como fuente del documento OpenAPI, y agregar únicamente la pieza necesaria para servir Swagger UI.
- **Razonamiento**: El proyecto ya publica correctamente el documento OpenAPI; añadir la UI es el cambio mínimo y reduce riesgo sobre la infraestructura existente.
- **Alternativas consideradas**: Sustituir toda la configuración por un enfoque distinto de Swagger, descartado porque ampliaría el alcance sin necesidad funcional.

## Decisión 2: Limitar Swagger UI a `Development`

- **Decisión**: Exponer Swagger UI solo dentro del bloque `if (app.Environment.IsDevelopment())`.
- **Razonamiento**: Mantiene la misma restricción de entorno que ya tiene OpenAPI y evita ampliar la superficie pública fuera del uso local previsto.
- **Alternativas consideradas**: Exponer la UI en todos los entornos, descartado por no haber sido solicitado explícitamente y por ser menos conservador.

## Decisión 3: Ruta `/swagger`

- **Decisión**: Publicar la interfaz en la ruta convencional `/swagger`.
- **Razonamiento**: Es la ruta que el usuario intentó abrir y la convención más reconocible para herramientas y validación manual.
- **Alternativas consideradas**: Usar otra ruta personalizada, descartado por no aportar valor adicional.

## Decisión 4: Validación mediante pruebas unitarias de composición

- **Decisión**: Añadir cobertura unitaria sobre `Program.cs` y la configuración visible de Swagger UI sin incorporar pruebas de integración.
- **Razonamiento**: La constitución autoriza unit tests y el estilo actual del proyecto ya usa inspección de `Program.cs` para validar composición.
- **Alternativas consideradas**: Probar la UI con `WebApplicationFactory`, descartado por quedar fuera del tipo de prueba aprobado por la constitución vigente.
