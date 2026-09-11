---

description: "Tareas para habilitar Swagger UI en NetRentManagerApi"
---

# Tareas: Interfaz Swagger UI del Backend

**Entrada**: Documentos de diseño de `specs/005-swagger-ui/`

**Prerequisitos**: `spec.md`, `plan.md`, `research.md`, `data-model.md`, `contracts/http-contracts.md`, `quickstart.md`

**Pruebas**: Se incluyen unit tests porque la constitución solo autoriza ese tipo de pruebas y la spec exige validar la composición de infraestructura.

**Regla de trazabilidad**: Todo cambio de código y toda validación de esta iniciativa debe corresponder a una tarea de este archivo antes de marcarse como completado.

## Fases y dependencias

- **Fase 1 - Preparación**: confirma el punto de composición y prepara la dependencia necesaria.
- **Fase 2 - Implementación**: habilita Swagger UI y ajusta la cobertura automatizada.
- **Fase 3 - Validación y cierre**: compila, ejecuta pruebas y registra evidencia.

---

## Fase 1: Preparación compartida

**Propósito**: preparar la dependencia y confirmar el alcance exacto del cambio.

- [X] T001 Actualizar `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` para agregar la dependencia necesaria para Swagger UI sin eliminar `Microsoft.AspNetCore.OpenApi`.
- [X] T002 Revisar y actualizar `specs/005-swagger-ui/spec.md` para reflejar el inicio de implementación con trazabilidad de estado cuando empiece la ejecución de código.

**Checkpoint**: el proyecto declara la dependencia necesaria y la spec queda lista para pasar a implementación.

---

## Fase 2: Implementación

**Propósito**: habilitar Swagger UI en la composición existente y validar la intención con pruebas unitarias.

### Pruebas de la historia

- [X] T003 [P] Crear o actualizar pruebas en `app/backend/tests/NetRentManagerApiTests/Infrastructure/` para comprobar que `Program.cs` mantiene OpenAPI y registra Swagger UI en `Development` sin introducir controllers.

### Implementación de la historia

- [X] T004 Actualizar `app/backend/src/NetRentManagerApi/Program.cs` para exponer Swagger UI en `/swagger` dentro del bloque de `Development`, manteniendo el documento OpenAPI existente.

**Checkpoint**: la UI queda configurada en la ruta esperada y el cambio sigue alineado con Minimal APIs y la composición centralizada.

---

## Fase 3: Validación y cierre

**Propósito**: verificar el resultado y dejar evidencia completa de la iniciativa.

- [X] T005 Compilar `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` para verificar que Swagger UI y sus dependencias se resuelven correctamente.
- [X] T006 Ejecutar las pruebas relevantes de `app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` relacionadas con composición de `Program.cs`.
- [X] T007 Actualizar `specs/005-swagger-ui/quickstart.md` con la evidencia de validación ejecutada y marcar como completadas las tareas terminadas.
- [X] T008 Actualizar `specs/005-swagger-ui/spec.md` para registrar la transición `En implementación` -> `Implementada` solo si no quedan tareas pendientes y la validación quedó documentada.
