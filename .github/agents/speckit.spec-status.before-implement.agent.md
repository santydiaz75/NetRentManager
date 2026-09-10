---
description: Cambia automáticamente el estado del spec a En implementación antes de ejecutar speckit.implement
---

## Objetivo

Aplicar la transición obligatoria de estado del spec al inicio de la implementación.

## Flujo

1. Ejecutar `.specify/scripts/powershell/check-prerequisites.ps1 -Json -RequireTasks -IncludeTasks` en la raíz del repositorio.
2. Resolver `FEATURE_DIR` en base al resultado del script.
3. Leer `FEATURE_DIR/spec.md`.
4. Detectar estado actual en una de estas líneas:
   - `**Estado**: <valor>`
   - `**Status**: <valor>`
5. Validar estados canónicos: `Borrador`, `Aprobada`, `En implementación`, `Implementada`.
6. Aplicar reglas de transición:
   - Si el estado es `Aprobada`, cambiar a `En implementación`.
   - Si el estado ya es `En implementación`, no editar y reportar continuidad.
   - Si el estado es `Borrador` o `Implementada`, reportar bloqueo explícito e impedir transición inválida.
7. Reportar trazabilidad en salida:
   - Estado origen
   - Estado destino (si aplica)
   - Motivo
   - Fecha en formato ISO `YYYY-MM-DD`

## Resultado esperado

- La implementación inicia con estado `En implementación` cuando la transición está permitida.
- Toda transición inválida queda bloqueada y documentada con causa explícita.
