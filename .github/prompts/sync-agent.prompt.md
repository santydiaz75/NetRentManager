---
name: sync-agent
description: Sincroniza AGENTS.md con los ultimos cambios del proyecto. Si no existe, lo crea desde cero.
---

Objetivo
- Mantener `AGENTS.md` siempre actualizado con los ultimos cambios del proyecto.
- Si `AGENTS.md` no existe, crearlo desde cero con una guia completa y accionable del agente.

Entradas
- BASE_DIR: ./
- AGENT_FILE: AGENTS.md

Flujo obligatorio

1. Leer contexto de trabajo y reglas:
  - .github/copilot-instructions.md
  - .specify/memory/constitution.md
  - spec activa: spec.md, plan.md, tasks.md

2. Detectar cambios relevantes del proyecto:
  - Inspeccionar estructura actual de carpetas y archivos.
  - Priorizar cambios en `/app`, `.specify` y `.github`.
  - Identificar altas, bajas y modificaciones que afecten el comportamiento del sistema o el flujo de trabajo.

3. Sincronizar `AGENTS.md`:
  - Si existe: actualizar solo lo necesario (enfoque incremental, sin reescribir todo).
  - Si no existe: crearlo con estructura completa inicial.
  - Preservar contenido manual util cuando no contradiga el estado real del repo.
  - Eliminar informacion obsoleta o inconsistente.

4. Estructura requerida de `AGENTS.md` (minimo):
  - Resumen del proyecto.
  - Stack y restricciones tecnicas.
  - Estructura de carpetas vigente.
  - Modulos clave y responsabilidades.
  - Flujos operativos del agente (como contribuir, validar cambios y mantener consistencia).
  - Reglas de Spec-Driven Development aplicables al repo.
  - Lista de archivos criticos y su proposito.
  - Seccion "Ultima sincronizacion" con fecha actual y resumen corto de cambios detectados.

5. Criterios de calidad:
  - Contenido claro, accionable y sin ambiguedades.
  - Sin inventar archivos, comandos o capacidades no presentes.
  - Mantener coherencia con la constitucion y la spec activa.
  - Usar ASCII.

Salida obligatoria al finalizar

1. Indicar si `AGENTS.md` fue creado o actualizado.
2. Resumir los cambios aplicados en `AGENTS.md`.
3. Listar archivos del proyecto considerados para la sincronizacion.
4. Confirmar que la informacion quedo alineada con `.specify` y estado actual del repo.
