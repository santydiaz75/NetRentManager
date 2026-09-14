---
name: execute-spec
description: Plantilla reutilizable para ejecutar cualquier spec. Solo tienes que cambiar el SPEC_NAME y corre el flujo completo del SDD
---

Configuracion
- SPEC_NAME: 001-create-foundation

Regla de resolucion de rutas:
- SPEC_DIR = .specify/specs/{SPEC_NAME}
- SPEC_FILE = {SPEC_DIR}/spec.md
- PLAN_FILE = {SPEC_DIR}/plan.md
- TASKS_FILE = {SPEC_DIR}/tasks.md

Flujo obligatorio

1. Leer y seguir:
  - .github/copilot-instructions.md
  - specify/memory/constitution.md
  - {SPEC_FILE}
  - {PLAN_FILE}
  - {TASKS_FILE}

2. Usar estos documentos como unica fuente de verdad del trabajo a ejecutar.
3. Antes de crear o editar, inspeccionar el estado actual de la /app y de {TASKS_FILE}; respetar lo ya implementado.
4. Ejecutar slo los checks pendientes (- [ ]) en {TASKS_FILE} en secuencia.
5. Marcar cada check en {TASKS_FILE} a medida que e complete realmente.
6. Crear o extender unicamente archivos dentro de la estructura obligatoria definida por el spec activo.
7. No introducir frameworks, npm, backend ni archivos funcionales fuera de la /app.
8. Toda persistencia debe pasar por js/storage.js usando las claves:
  - realtor.properties
  - realter.tenants
  - realtor.rentals
  - realtor.flash
9. Detenerse solo si hay ambiguedad real o contradiccion entre documentos.

Salida obligatoria al finalizar 

1. Que se construyo o modifico.
2. Que archivos fueron creados o modificados.
3. Que tareas de {TASKS_FILE} quedaron marcadas como completadas.
4. Que dentro del archivo {SPEC_FILE} se actualice la seccion de ESTADO para indicar que esta en implement.
4. Que queda pendiente (si aplica).
5. Que specs dependientes pueden avanzar gracias a estos cambios.

