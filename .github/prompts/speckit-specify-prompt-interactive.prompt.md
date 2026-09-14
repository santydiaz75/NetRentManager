---
name: speckit-specify-prompt-interactive
description: Inicia el descubrimiento funcional interactivo para generar un spec.md
argument-hint: Describe la funcionalidad que deseas crear, modificar, corregir o documentar
agent: agent
------------

Aplica íntegramente las instrucciones definidas en el siguiente template:

[Template interactivo para speckit.specify](../../support/templates/speckit-specify-prompt-interactive.md)

## Entrada de la iniciativa

Considera todo el texto proporcionado por el usuario después del comando `/speckit-specify-prompt-interactive` como la descripción inicial de la iniciativa.

La descripción puede corresponder a:

* Una funcionalidad nueva.
* Una modificación de comportamiento existente.
* Una corrección.
* Una iniciativa de ingeniería inversa o documentación retrospectiva.

Analiza primero esa descripción utilizando el template referenciado.

No generes inmediatamente el archivo `spec.md`.

Debes comenzar por:

1. Extraer la información funcional ya proporcionada.
2. Mostrar la cobertura inicial de la especificación.
3. Identificar las decisiones materiales pendientes.
4. Realizar exactamente una pregunta interactiva por turno.
5. Mostrar el enunciado completo antes de las opciones.
6. Presentar cuatro alternativas concretas y una opción personalizada.
7. Marcar una alternativa como recomendada cuando exista información suficiente.
8. Incorporar cada respuesta al borrador acumulativo.
9. Mostrar el progreso durante el descubrimiento.
10. Solicitar autorización antes de generar el `spec.md`.

No solicites nuevamente información que ya esté presente en la descripción inicial.

No preguntes por decisiones técnicas que correspondan a `speckit.plan`.

Respeta completamente:

* El `constitution.md` vigente.
* El template interactivo referenciado.
* Las specs aprobadas aplicables.
* La estructura canónica del repositorio.
