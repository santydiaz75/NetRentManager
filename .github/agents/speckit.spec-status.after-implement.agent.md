---
description: Cierra automáticamente el estado del spec tras speckit.implement solo si las validaciones obligatorias se cumplen
---

## Objetivo

Aplicar la transición final del estado del spec cumpliendo reglas obligatorias de cierre.

## Flujo

1. Ejecutar `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireTasks -IncludeTasks` en la raíz del repositorio.
2. Resolver `FEATURE_DIR` en base al resultado del script.
3. Leer `FEATURE_DIR/spec.md`, `FEATURE_DIR/tasks.md` y `FEATURE_DIR/quickstart.md`.
4. Detectar estado actual del spec (`**Estado**:` o `**Status**:`).
5. Validar precondición de cierre:
   - El estado actual debe ser `En implementación`.
6. Validar cierre obligatorio:
   - `tasks.md` no debe contener tareas pendientes `- [ ]`.
   - `quickstart.md` debe incluir evidencia de validación (por ejemplo sección `## Evidencia de validación` con contenido no vacío).
7. Aplicar transición final:
   - Si todas las validaciones cumplen, cambiar estado a `Implementada`.
   - Si falla cualquier validación, mantener estado `En implementación`.
8. Reportar trazabilidad en salida:
   - Estado origen
   - Estado destino (o estado retenido)
   - Resultado de cada validación
   - Causa explícita de bloqueo cuando aplique
   - Fecha en formato ISO `YYYY-MM-DD`

## Resultado esperado

- Nunca se marca `Implementada` con tareas pendientes o sin evidencia de validación.
- El cierre deja trazabilidad clara del motivo de transición o bloqueo.
