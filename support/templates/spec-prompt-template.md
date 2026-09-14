---
name: <nombre-prompt>
description: <descripcion-spec>
agent: speckit.specify
---

/speckit.specify

Crear la especificación funcional para:

**[NOMBRE CORTO DE LA FUNCIONALIDAD]**

## 1. Naturaleza de la iniciativa

Tipo de iniciativa:

* [ ] Funcionalidad nueva.
* [ ] Cambio de comportamiento existente.
* [ ] Corrección de un comportamiento defectuoso.
* [ ] Ingeniería inversa o documentación retrospectiva de un cambio ya implementado.

Si se trata de una iniciativa retrospectiva, el código ya fue modificado y esta especificación debe reconstruir el propósito, el alcance, los criterios de aceptación y la trazabilidad del cambio sin introducir comportamiento nuevo.

## 2. Contexto actual

Actualmente:

[DESCRIBIR CÓMO FUNCIONA EL SISTEMA EN ESTE MOMENTO]

Los usuarios o sistemas involucrados actualmente pueden:

* [COMPORTAMIENTO ACTUAL 1]
* [COMPORTAMIENTO ACTUAL 2]
* [COMPORTAMIENTO ACTUAL 3]

## 3. Problema

El problema que necesitamos resolver es:

[EXPLICAR EL PROBLEMA REAL, LA LIMITACIÓN O EL COMPORTAMIENTO INCORRECTO]

Este problema provoca que:

* [CONSECUENCIA PARA EL USUARIO]
* [CONSECUENCIA PARA EL NEGOCIO]
* [CONSECUENCIA PARA OTRO SISTEMA, SI APLICA]

## 4. Objetivo

El objetivo de esta iniciativa es permitir que:

**[ACTOR PRINCIPAL]** pueda **[ACCIÓN PRINCIPAL]** para **[RESULTADO O VALOR ESPERADO]**.

La funcionalidad se considerará útil cuando:

[DESCRIBIR EL RESULTADO FUNCIONAL GENERAL ESPERADO]

## 5. Usuarios, actores o sistemas involucrados

### Actor principal

**[NOMBRE DEL ACTOR]**

Necesita:

[NECESIDAD PRINCIPAL DEL ACTOR]

### Actores secundarios

* **[ACTOR O SISTEMA 2]:** [PARTICIPACIÓN]
* **[ACTOR O SISTEMA 3]:** [PARTICIPACIÓN]

Si no existen actores secundarios, indicarlo explícitamente.

## 6. Flujo principal esperado

1. El usuario o sistema [ACCIÓN INICIAL].
2. El sistema [RESPUESTA OBSERVABLE].
3. El usuario o sistema [SIGUIENTE ACCIÓN].
4. El sistema [PROCESAMIENTO FUNCIONAL OBSERVABLE].
5. El usuario recibe [RESULTADO FINAL].

## 7. Requisitos funcionales

* El sistema debe permitir [CAPACIDAD FUNCIONAL 1].
* El sistema debe permitir [CAPACIDAD FUNCIONAL 2].
* El sistema debe mostrar [INFORMACIÓN O ESTADO].
* El sistema debe conservar [DATO O ESTADO FUNCIONAL].
* El sistema debe impedir [COMPORTAMIENTO NO PERMITIDO].
* El sistema debe informar claramente cuando [ERROR O CONDICIÓN ESPECIAL].
* El sistema debe permitir que el usuario [ACCIÓN DE RECUPERACIÓN O CONTINUIDAD].

Cada requisito debe describir un comportamiento observable y verificable.

## 8. Reglas de negocio

* [REGLA DE NEGOCIO 1]
* [REGLA DE NEGOCIO 2]
* [REGLA DE NEGOCIO 3]
* [RESTRICCIÓN FUNCIONAL]
* [CONDICIÓN PARA PERMITIR O RECHAZAR UNA OPERACIÓN]

No inventar reglas de negocio que no estén descritas en este prompt, en la constitución o en una especificación aprobada.

## 9. Datos funcionales necesarios

La funcionalidad necesita trabajar con la siguiente información:

* **[DATO 1]:** [DESCRIPCIÓN FUNCIONAL]
* **[DATO 2]:** [DESCRIPCIÓN FUNCIONAL]
* **[DATO 3]:** [DESCRIPCIÓN FUNCIONAL]

Para cada dato, indicar cuando corresponda:

* Si es obligatorio u opcional.
* Qué representa para el usuario.
* Qué restricciones funcionales tiene.
* Qué debe ocurrir si no está disponible.

No definir tablas, columnas, tipos técnicos, entidades, DTOs ni estructuras de persistencia.

## 10. Validaciones funcionales

* [CAMPO O INFORMACIÓN] es obligatorio.
* [CAMPO O INFORMACIÓN] debe cumplir [REGLA].
* No debe permitirse [COMBINACIÓN O ESTADO INVÁLIDO].
* Cuando [VALIDACIÓN] falle, el usuario debe recibir [RESULTADO OBSERVABLE].
* Si existen varios errores, el sistema debe [COMPORTAMIENTO ESPERADO].

Las validaciones deben expresarse desde la perspectiva funcional, sin indicar cómo serán programadas.

## 11. Escenarios de aceptación

### Escenario 1: operación exitosa

Dado que [ESTADO INICIAL],
cuando [ACCIÓN DEL USUARIO O SISTEMA],
entonces [RESULTADO OBSERVABLE ESPERADO].

### Escenario 2: validación de información

Dado que [ESTADO INICIAL],
cuando [ACCIÓN CON INFORMACIÓN INVÁLIDA],
entonces [MENSAJE O COMPORTAMIENTO ESPERADO].

### Escenario 3: recurso inexistente

Dado que [EL RECURSO NO EXISTE],
cuando [ACCIÓN REALIZADA],
entonces [RESPUESTA FUNCIONAL ESPERADA].

### Escenario 4: estado no permitido

Dado que [ESTADO ACTUAL],
cuando [ACCIÓN NO PERMITIDA],
entonces [RESULTADO ESPERADO].

### Escenario 5: datos opcionales ausentes

Dado que [FALTA INFORMACIÓN OPCIONAL],
cuando [ACCIÓN DEL USUARIO],
entonces [COMPORTAMIENTO ESPERADO SIN INTERRUMPIR EL FLUJO].

## 12. Casos límite

Considerar explícitamente:

* Qué ocurre cuando [CASO LÍMITE 1].
* Qué ocurre cuando [CASO LÍMITE 2].
* Qué ocurre si la operación se ejecuta dos veces.
* Qué ocurre si el estado cambia durante la operación.
* Qué ocurre si parte de la información no está disponible.
* Qué ocurre si el usuario abandona o regresa al flujo.
* Qué ocurre cuando no existen resultados.

Agregar únicamente casos límite con impacto funcional real.

## 13. Alcance incluido

Esta iniciativa incluye:

* [ELEMENTO INCLUIDO 1]
* [ELEMENTO INCLUIDO 2]
* [ELEMENTO INCLUIDO 3]
* [VALIDACIÓN O ESCENARIO INCLUIDO]

## 14. Fuera de alcance

Esta iniciativa no incluye:

* [ELEMENTO EXCLUIDO 1]
* [ELEMENTO EXCLUIDO 2]
* [FUNCIONALIDAD QUE SE REALIZARÁ POSTERIORMENTE]
* [CAMBIO TÉCNICO O FUNCIONAL NO RELACIONADO]

No incorporar elementos fuera de alcance como requisitos de esta especificación.

## 15. Dependencias funcionales

Esta iniciativa depende de:

* [SPEC O FUNCIONALIDAD EXISTENTE]
* [DATOS QUE DEBEN EXISTIR]
* [SISTEMA O PROCESO RELACIONADO]

Supuestos conocidos:

* [SUPUESTO CONFIRMADO 1]
* [SUPUESTO CONFIRMADO 2]

Los supuestos no confirmados deben tratarse como aclaraciones pendientes.

## 16. Criterios de éxito

La iniciativa se considerará completada cuando:

* El actor principal pueda [RESULTADO VERIFICABLE].
* El sistema aplique correctamente [REGLA DE NEGOCIO].
* Los datos inválidos produzcan [RESULTADO ESPERADO].
* Los recursos inexistentes sean manejados mediante [COMPORTAMIENTO].
* Los escenarios de aceptación puedan verificarse objetivamente.
* No se implemente comportamiento fuera del alcance aprobado.

No utilizar únicamente criterios técnicos como “el código compila”, “el endpoint existe” o “el componente fue creado”.

## 17. Evidencia disponible

Completar esta sección principalmente para correcciones o iniciativas retrospectivas.

Evidencia existente:

* **Cambio realizado:** [RESUMEN DEL CAMBIO]
* **Diff o archivos relacionados:** [REFERENCIAS]
* **Compilación:** [RESULTADO O PENDIENTE]
* **Pruebas automatizadas:** [RESULTADO O PENDIENTE]
* **Validación manual:** [RESULTADO O PENDIENTE]
* **Resultado observado:** [DESCRIPCIÓN]

La evidencia debe utilizarse para documentar el comportamiento existente, no para agregar requisitos nuevos.

## 18. Información que todavía necesita aclaración

Decisiones funcionales conocidas como pendientes:

* [PREGUNTA O DECISIÓN PENDIENTE 1]
* [PREGUNTA O DECISIÓN PENDIENTE 2]

Si no existen decisiones pendientes, indicar:

`No existen aclaraciones funcionales conocidas al iniciar la especificación.`

## Instrucciones para speckit.specify

Genera el `spec.md` respetando completamente el `constitution.md` vigente.

Antes de generar el archivo:

1. Analiza el input y detecta únicamente las decisiones funcionales materiales que falten.
2. No preguntes por información que ya esté explícitamente definida en este prompt, en la constitución o en specs aprobadas.
3. No preguntes por frameworks, librerías, clases, endpoints, tablas, carpetas, arquitectura ni detalles de implementación.
4. Cuando necesites una aclaración, realiza las preguntas de una en una.
5. Imprime siempre el enunciado completo y visible antes de mostrar las opciones.
6. Muestra claramente:

   * Número y tema de la pregunta.
   * Enunciado completo.
   * Por qué importa.
   * Opciones disponibles.
   * Cómo responder.
7. No muestres únicamente opciones o recomendaciones.
8. No inventes requisitos ni reglas de negocio.
9. Si una decisión material no puede resolverse, regístrala como:

`[NEEDS CLARIFICATION: pregunta concreta]`

10. Mantén la especificación en estado Borrador mientras existan aclaraciones materiales pendientes.
11. Expresa los requisitos como comportamientos observables y verificables.
12. Mantén una separación estricta entre `spec.md` y `plan.md`.
13. No agregues código ni propongas la solución técnica.
14. No repitas innecesariamente el stack tecnológico establecido por la constitución.
15. Genera el documento completamente en español.
16. Mantén el alcance limitado a esta iniciativa.
17. Al terminar las preguntas, muestra un resumen de las decisiones confirmadas antes de generar el `spec.md`.
