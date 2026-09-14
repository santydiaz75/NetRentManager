# Template interactivo para crear `spec.md`

## Propósito

Este documento define el flujo obligatorio de descubrimiento funcional que debe seguirse para crear un archivo `spec.md`.

Este archivo es un template reutilizable e inmutable. No contiene una funcionalidad específica y no debe modificarse durante la ejecución.

La descripción de la iniciativa será proporcionada por el prompt invocador. Debe considerarse como descripción inicial todo el texto que el usuario escriba después del comando personalizado que cargó este template.

El objetivo del flujo es transformar esa descripción libre en una especificación funcional:

* Clara.
* Completa.
* Verificable.
* Limitada al alcance solicitado.
* Separada de las decisiones técnicas.
* Alineada con el `constitution.md`.
* Compatible con el flujo de Spec Kit.

No se debe generar inmediatamente el archivo `spec.md`.

Primero se debe analizar la entrada, detectar información faltante y realizar un descubrimiento funcional interactivo.

---

# 1. Fuentes de información y precedencia

Utiliza las siguientes fuentes, en este orden:

1. Las respuestas más recientes proporcionadas explícitamente por el usuario.
2. La descripción inicial recibida mediante el prompt invocador.
3. Las decisiones confirmadas durante esta interacción.
4. El `constitution.md` vigente.
5. Las specs aprobadas que resulten aplicables.
6. Los artefactos existentes de la iniciativa.
7. La evidencia verificable del repositorio, únicamente cuando se trate de ingeniería inversa o documentación retrospectiva.

No inventes requisitos, actores, reglas de negocio, validaciones, datos, escenarios ni criterios de éxito.

Cuando dos fuentes se contradigan, no elijas silenciosamente una interpretación. Detén esa parte del análisis y solicita una decisión explícita al usuario.

La información proporcionada directamente por el usuario prevalece sobre una inferencia anterior, salvo que contradiga la constitución o una regla normativa vigente.

---

# 2. Entrada proporcionada por el prompt invocador

Considera como descripción inicial de la iniciativa todo el texto proporcionado por el usuario después del comando personalizado.

La entrada puede describir:

* Una funcionalidad nueva.
* Una modificación de comportamiento existente.
* Una corrección.
* Una iniciativa de ingeniería inversa.
* Una documentación retrospectiva de un cambio ya implementado.

No solicites al usuario que vuelva a pegar o repetir la descripción inicial.

Si el mensaje incluye texto entre comillas, interpreta el contenido de las comillas como parte de la descripción, pero no conserves las comillas como parte del requerimiento.

Si la descripción está distribuida en varias líneas, analiza todas las líneas como una única entrada funcional.

---

# 3. Áreas funcionales que deben analizarse

Clasifica la información encontrada dentro de las siguientes áreas:

1. Naturaleza de la iniciativa.
2. Contexto actual.
3. Problema.
4. Objetivo.
5. Usuarios, actores o sistemas involucrados.
6. Flujo principal esperado.
7. Requisitos funcionales.
8. Reglas de negocio.
9. Datos funcionales necesarios.
10. Validaciones funcionales.
11. Escenarios de aceptación.
12. Casos límite.
13. Alcance incluido.
14. Fuera de alcance.
15. Dependencias y supuestos.
16. Criterios de éxito.
17. Evidencia disponible.
18. Aclaraciones pendientes.

No todas las áreas deben completarse artificialmente.

Cuando una sección no corresponda a la iniciativa, márcala como `No aplica`.

---

# 4. Análisis inicial obligatorio

Antes de formular la primera pregunta:

1. Lee completamente la descripción inicial.
2. Identifica la naturaleza de la iniciativa.
3. Extrae todos los hechos funcionales ya proporcionados.
4. Diferencia hechos confirmados de inferencias.
5. Detecta ambigüedades materiales.
6. Identifica posibles contradicciones.
7. Determina qué áreas están completas, parciales, pendientes o no aplican.
8. Estima qué decisiones deben resolverse antes de generar el `spec.md`.

No generes todavía el archivo.

No realices preguntas sobre información que ya haya sido proporcionada claramente.

---

# 5. Cobertura inicial de la especificación

Antes de comenzar las preguntas, muestra obligatoriamente un resumen visible con los siguientes estados:

* `[x] Confirmado`: existe información suficiente.
* `[~] Parcial`: existe información, pero falta una decisión material.
* `[ ] Pendiente`: no existe información suficiente.
* `[-] No aplica`: la sección no corresponde a la iniciativa.

Utiliza un formato equivalente a:

## Cobertura inicial de la especificación

* `[x]` Naturaleza de la iniciativa
* `[x]` Contexto actual
* `[~]` Problema
* `[x]` Objetivo
* `[~]` Flujo principal
* `[ ]` Reglas de negocio
* `[ ]` Casos límite
* `[-]` Evidencia retrospectiva

## Información comprendida

Resume en un máximo de siete puntos los hechos funcionales identificados.

## Decisiones pendientes

Indica brevemente qué decisiones materiales necesitan confirmación.

## Próximo paso

Informa que comenzarás el descubrimiento interactivo realizando una pregunta a la vez.

No muestres todas las preguntas futuras.

---

# 6. Selección de preguntas

Formula una pregunta únicamente cuando su respuesta pueda modificar de forma material:

* El problema.
* El objetivo.
* Los actores.
* El flujo principal.
* Los requisitos funcionales.
* Las reglas de negocio.
* Los datos necesarios.
* Las validaciones.
* Los escenarios de aceptación.
* Los casos límite.
* El alcance.
* Los elementos fuera de alcance.
* Las dependencias.
* Los supuestos.
* Los criterios de éxito.
* La evidencia retrospectiva.

No preguntes por información que ya pueda deducirse inequívocamente de:

1. La descripción inicial.
2. Una respuesta anterior del usuario.
3. La constitución.
4. Una spec aprobada aplicable.
5. Evidencia verificable del repositorio para una iniciativa retrospectiva.

No realices preguntas de bajo impacto únicamente para completar una sección o alcanzar una cantidad determinada de preguntas.

---

# 7. Separación entre `spec.md` y `plan.md`

Durante este flujo está prohibido preguntar por decisiones técnicas como:

* Frameworks.
* Librerías.
* Paquetes.
* Patrones de arquitectura.
* Clases.
* Interfaces.
* Métodos.
* Endpoints.
* Rutas HTTP.
* Métodos HTTP.
* DTOs.
* Entidades.
* Tablas.
* Columnas.
* Migraciones.
* Estructuras de persistencia.
* Componentes internos.
* Nombres de archivos.
* Estructuras de carpetas.
* Estrategias de implementación.
* Configuración de infraestructura.

Estas decisiones corresponden a `speckit.plan`.

Si la descripción inicial menciona detalles técnicos, utilízalos únicamente como contexto cuando ya representen una restricción confirmada. No conviertas automáticamente esos detalles en requisitos funcionales.

La especificación debe concentrarse en:

* Qué necesita hacer el sistema.
* Por qué se necesita.
* Quién utiliza la funcionalidad.
* Qué comportamiento debe observarse.
* Qué reglas deben cumplirse.
* Cómo se verificará el resultado.

---

# 8. Orden recomendado del descubrimiento

Prioriza las preguntas siguiendo este orden.

## Nivel 1: propósito

1. Naturaleza de la iniciativa.
2. Contexto actual.
3. Problema.
4. Objetivo.

## Nivel 2: comportamiento

5. Actores.
6. Flujo principal.
7. Requisitos funcionales.
8. Reglas de negocio.

## Nivel 3: condiciones

9. Datos funcionales.
10. Validaciones.
11. Escenarios de aceptación.
12. Casos límite.

## Nivel 4: límites y cierre

13. Alcance incluido.
14. Fuera de alcance.
15. Dependencias y supuestos.
16. Criterios de éxito.

## Nivel 5: iniciativas retrospectivas

17. Evidencia disponible.
18. Aclaraciones pendientes.

Puedes cambiar el orden cuando una decisión sea necesaria para comprender correctamente una pregunta posterior.

---

# 9. Una pregunta por interacción

Realiza exactamente una pregunta por turno.

No presentes varias preguntas en una misma respuesta.

No agrupes decisiones independientes dentro de una sola pregunta.

Cada pregunta debe resolver una dimensión funcional concreta.

Antes de esperar la respuesta del usuario, imprime obligatoriamente:

1. El número y el tema de la pregunta.
2. El enunciado completo.
3. La información ya comprendida relacionada con la pregunta.
4. La explicación de por qué importa.
5. Las opciones disponibles.
6. La recomendación contextual.
7. Las instrucciones para responder.

La pregunta debe ser completamente visible antes de las opciones.

Está prohibido mostrar solamente:

* Las opciones.
* La alternativa recomendada.
* Un selector.
* Un formulario.
* Un widget.
* Una tabla de respuestas.
* Una lista de recomendaciones.

El usuario nunca debe tener que deducir la pregunta a partir de las opciones.

---

# 10. Formato obligatorio de cada pregunta

Utiliza el siguiente formato:

## Pregunta [N de TOTAL] — [tema corto]

**Pregunta:**

[Enunciado completo, visible, claro y concreto]

**Lo que entendí hasta ahora:**

[Resumen breve de la información relacionada que ya fue proporcionada]

**Por qué importa:**

[Explicación breve del impacto funcional de esta decisión]

**Opciones:**

A) [Alternativa concreta y ejecutable]
B) [Alternativa concreta y ejecutable] — **Recomendada**
C) [Alternativa concreta y ejecutable]
D) [Alternativa concreta y ejecutable]
E) Otro — escribe tu respuesta personalizada

**Recomendación:**

Recomiendo la opción **B** porque [explicación específica basada en el contexto de la iniciativa].

**Cómo responder:**

Responde con `A`, `B`, `C`, `D` o `E`.

También puedes:

* Escribir directamente una respuesta personalizada.
* Seleccionar varias opciones cuando la pregunta lo permita.
* Agregar una condición, por ejemplo: `B, pero solamente cuando...`.
* Escribir `No aplica`.
* Escribir `Volver` para revisar la decisión anterior.
* Escribir `Mostrar progreso` para consultar el estado actual.

No asumas que la opción recomendada siempre será `B`. Coloca la etiqueta `Recomendada` en la alternativa que corresponda y actualiza también la sección **Recomendación**.

---

# 11. Construcción de las opciones

Cada pregunta debe presentar cinco opciones:

* Cuatro alternativas concretas: `A`, `B`, `C` y `D`.
* Una alternativa personalizada: `E) Otro`.

Las opciones concretas deben:

* Responder directamente a la pregunta.
* Representar comportamientos funcionales distinguibles.
* Ser comprensibles sin contexto adicional.
* Ser ejecutables desde una perspectiva funcional.
* Evitar expresiones genéricas.
* Evitar decisiones técnicas.
* Estar relacionadas con la funcionalidad descrita.
* Reflejar alternativas razonables.

No utilices opciones como:

* Usar el comportamiento estándar.
* Aplicar una buena práctica.
* Hacerlo de la mejor forma.
* Usar una opción flexible.
* Dejar que el sistema decida.
* Lo que sea más conveniente.

Cuando solo existan dos o tres alternativas razonables, utiliza las opciones restantes para respuestas como:

* Mantener el comportamiento actual.
* No incluirlo en esta iniciativa.
* No aplica.
* Dejarlo fuera de alcance.

No inventes alternativas absurdas únicamente para completar las cuatro opciones concretas.

---

# 12. Recomendación contextual

Marca exactamente una alternativa como **Recomendada** cuando exista información suficiente para hacerlo responsablemente.

La recomendación debe basarse en:

1. La descripción inicial.
2. Las respuestas anteriores.
3. El objetivo funcional.
4. Las reglas de negocio confirmadas.
5. El alcance.
6. La constitución.
7. Las specs aprobadas relacionadas.
8. La consistencia con el resto del flujo.

No recomiendes una alternativa únicamente porque sea común en otros proyectos.

Explica la recomendación en una o dos frases.

Cuando no exista información suficiente, indica:

`No existe una recomendación responsable con la información disponible.`

En ese caso:

* No marques ninguna opción como recomendada.
* Explica qué información falta.
* Permite que el usuario decida libremente.

---

# 13. Selección múltiple

Permite seleccionar varias opciones únicamente cuando las alternativas puedan aplicarse simultáneamente.

Cuando la selección múltiple esté permitida, indícalo explícitamente antes de las opciones:

`Puedes seleccionar varias opciones.`

Acepta respuestas como:

* `A, C`
* `B y D`
* `A, pero también...`

No permitas selección múltiple cuando las alternativas sean mutuamente excluyentes.

---

# 14. Respuesta personalizada

Si el usuario selecciona `E` o escribe directamente una respuesta personalizada:

1. Acepta la respuesta sin obligarlo a elegir una de las alternativas.
2. Interpreta la intención funcional.
3. Reformula la respuesta como una decisión clara.
4. Conserva exactamente su significado.
5. Solicita confirmación solo cuando resulte ambigua o contradictoria.
6. Registra la decisión en todas las secciones afectadas.
7. Continúa con la siguiente pregunta.

Ejemplo de respuesta:

`E. La imagen debe poder agregarse después de registrar la propiedad.`

Registro esperado:

`El usuario puede registrar la propiedad sin imagen y agregarla posteriormente mediante el flujo correspondiente.`

No agregues condiciones que el usuario no haya indicado.

---

# 15. Registro de cada respuesta

Después de recibir una respuesta:

1. Interpreta la opción seleccionada o el texto libre.
2. Confirma la decisión registrada.
3. Indica las áreas actualizadas.
4. Actualiza el borrador acumulativo.
5. Actualiza el estado de cobertura.
6. Continúa con la siguiente pregunta pendiente.

Utiliza un formato equivalente a:

### Decisión registrada

**Decisión:**

[Descripción funcional completa de la respuesta]

**Áreas actualizadas:**

* Reglas de negocio.
* Casos límite.
* Escenarios de aceptación.

**Progreso:**

`11 de 18 áreas revisadas`

Después presenta la siguiente pregunta en la misma respuesta, salvo que exista una contradicción que deba resolverse primero.

No repitas una pregunta ya respondida.

---

# 16. Borrador acumulativo

Mantén internamente un borrador acumulativo de la especificación durante todo el proceso.

Cada respuesta puede actualizar una o varias áreas.

Por ejemplo, una decisión sobre registros duplicados puede actualizar:

* Reglas de negocio.
* Validaciones.
* Casos límite.
* Escenarios de aceptación.
* Criterios de éxito.

No obligues al usuario a responder varias veces sobre la misma decisión solo porque afecta secciones diferentes.

Conserva durante toda la interacción:

* Hechos confirmados.
* Decisiones confirmadas.
* Requisitos identificados.
* Reglas de negocio.
* Datos funcionales.
* Validaciones.
* Escenarios.
* Casos límite.
* Alcance.
* Fuera de alcance.
* Dependencias.
* Supuestos.
* Evidencia.
* Aclaraciones pendientes.
* Contradicciones detectadas.

---

# 17. Progreso visible

Después de cada tres respuestas, al completar un nivel de descubrimiento o cuando el usuario escriba `Mostrar progreso`, presenta un resumen compacto.

Utiliza:

## Progreso de la especificación

* `[x]` Contexto
* `[x]` Problema
* `[x]` Objetivo
* `[x]` Actores
* `[~]` Reglas de negocio
* `[ ]` Casos límite
* `[ ]` Fuera de alcance

**Decisiones confirmadas:** [cantidad]

**Decisiones pendientes:** [cantidad]

**Aclaraciones abiertas:** [cantidad]

**Cobertura aproximada:** [porcentaje]

No muestres el contenido completo del futuro `spec.md` después de cada respuesta.

---

# 18. Comando `Volver`

Cuando el usuario escriba `Volver`:

1. Identifica la decisión inmediatamente anterior.
2. Muestra la decisión registrada.
3. Vuelve a presentar la pregunta original completa.
4. Permite seleccionar una nueva opción o escribir una respuesta personalizada.
5. Reemplaza la decisión anterior.
6. Actualiza todas las áreas afectadas.
7. Revisa si la modificación genera contradicciones nuevas.

No conserves simultáneamente la decisión anterior y la nueva.

---

# 19. Contradicciones

Si una respuesta contradice:

* La descripción inicial.
* Una respuesta anterior.
* Una regla de negocio confirmada.
* Una spec aprobada.
* La constitución.
* Evidencia verificable de una iniciativa retrospectiva.

No elijas silenciosamente una interpretación.

Muestra:

## Contradicción detectada

**Información anterior:**

[Contenido anterior]

**Nueva respuesta:**

[Contenido nuevo]

**Impacto:**

[Áreas y comportamientos afectados]

Después realiza exactamente una pregunta para resolver la contradicción.

No continúes con preguntas independientes hasta que la contradicción material quede resuelta.

---

# 20. Aclaraciones pendientes

Cuando una decisión material no pueda resolverse:

1. Regístrala como:

`[NEEDS CLARIFICATION: pregunta concreta]`

2. Indica qué áreas afecta.
3. Mantén la especificación en estado Borrador.
4. Continúa con otras preguntas independientes.
5. Inclúyela en el resumen final.

No bloquees todo el descubrimiento por una sola aclaración pendiente.

No permitas que una spec con aclaraciones materiales sea presentada como aprobada.

---

# 21. Ingeniería inversa y documentación retrospectiva

Cuando la descripción inicial indique que el cambio ya fue implementado:

* Clasifica la iniciativa como retrospectiva.
* Reconstruye la especificación a partir del comportamiento verificado.
* No propongas código nuevo.
* No introduzcas mejoras futuras dentro del alcance retrospectivo.
* Distingue entre hechos observados y comportamientos supuestos.
* Solicita evidencia únicamente cuando no esté disponible.
* Registra compilación, pruebas y validación manual cuando correspondan.
* Conserva la trazabilidad del cambio.

Las preguntas deben cubrir, cuando aplique:

* Qué problema existía.
* Qué comportamiento ocurría antes.
* Qué comportamiento existe ahora.
* Qué archivos o cambios sirven como evidencia.
* Qué pruebas se ejecutaron.
* Qué validación manual se realizó.
* Qué resultado se observó.
* Qué gaps documentales deben considerarse resueltos.

La evidencia sirve para documentar el comportamiento existente. No debe utilizarse para inventar requisitos nuevos.

---

# 22. Condición para finalizar las preguntas

Finaliza el descubrimiento cuando:

* Las decisiones funcionales materiales estén confirmadas.
* Las áreas restantes estén marcadas como `No aplica`.
* Las decisiones no resueltas estén registradas como aclaraciones.
* No existan contradicciones abiertas.
* El objetivo esté claramente definido.
* El flujo principal esté claro.
* Las reglas de negocio relevantes estén confirmadas.
* El alcance y fuera de alcance estén diferenciados.
* Los criterios de éxito sean verificables.

No continúes realizando preguntas de bajo impacto únicamente para completar todas las áreas.

No finalices prematuramente mientras falten decisiones que puedan modificar significativamente la funcionalidad.

---

# 23. Resumen final de decisiones

Al terminar el descubrimiento, muestra un resumen estructurado.

Utiliza:

## Resumen final de decisiones

### Naturaleza de la iniciativa

[Contenido confirmado]

### Contexto actual

[Contenido confirmado]

### Problema

[Contenido confirmado]

### Objetivo

[Contenido confirmado]

### Actores

[Contenido confirmado]

### Flujo principal

[Contenido confirmado]

### Requisitos funcionales

[Contenido confirmado]

### Reglas de negocio

[Contenido confirmado]

### Datos funcionales

[Contenido confirmado]

### Validaciones

[Contenido confirmado]

### Escenarios de aceptación

[Contenido confirmado]

### Casos límite

[Contenido confirmado]

### Alcance incluido

[Contenido confirmado]

### Fuera de alcance

[Contenido confirmado]

### Dependencias y supuestos

[Contenido confirmado]

### Criterios de éxito

[Contenido confirmado]

### Evidencia

[Contenido confirmado o `No aplica`]

### Aclaraciones pendientes

[Contenido confirmado o `Ninguna`]

No generes todavía el archivo hasta presentar la decisión final.

---

# 24. Pregunta final

Después del resumen, presenta:

## Pregunta final — Generación de la especificación

**Pregunta:**

¿Qué deseas hacer con la información recopilada?

**Por qué importa:**

Esta decisión determina si el `spec.md` puede generarse ahora o si debemos revisar alguna sección.

**Opciones:**

A) Generar ahora el `spec.md` con las decisiones confirmadas — **Recomendada**
B) Revisar una sección específica antes de generar
C) Agregar información funcional adicional
D) Generar como Borrador conservando las aclaraciones pendientes
E) Otro — escribe la acción que deseas realizar

**Recomendación:**

Recomiendo la opción **A** cuando no existan aclaraciones materiales ni contradicciones abiertas.

Cuando existan aclaraciones materiales, recomienda la opción **D**.

**Cómo responder:**

Responde con `A`, `B`, `C`, `D` o `E`.

---

# 25. Generación del `spec.md`

Cuando el usuario autorice la generación:

1. Utiliza el template oficial de `spec.md` definido por el proyecto.
2. Respeta el `constitution.md`.
3. Utiliza todas las decisiones confirmadas.
4. Genera el documento completamente en español.
5. Expresa los requisitos como comportamientos observables.
6. Incluye reglas de negocio verificables.
7. Incluye escenarios de aceptación.
8. Separa claramente alcance y fuera de alcance.
9. Incluye casos límite relevantes.
10. Incluye criterios objetivos de éxito.
11. Registra las aclaraciones pendientes.
12. Mantén el estado Borrador cuando existan aclaraciones materiales.
13. No agregues decisiones técnicas.
14. No inventes requisitos.
15. No repitas innecesariamente el stack definido por la constitución.
16. No introduzcas funcionalidades futuras dentro del alcance.
17. Utiliza la estructura y ubicación canónica definidas por el proyecto.

Cuando el flujo oficial de Spec Kit requiera:

* Crear una rama.
* Determinar el siguiente número de spec.
* Crear el directorio de la iniciativa.
* Ejecutar scripts del proyecto.
* Utilizar `.specify/templates/spec-template.md`.

Respeta esos mecanismos existentes y no los reemplaces con convenciones inventadas.

Si un script de Spec Kit contradice la constitución, detente y reporta la divergencia de acuerdo con el protocolo constitucional.

---

# 26. Resultado final

Después de generar el archivo, informa:

* La ruta del `spec.md`.
* El nombre de la iniciativa.
* El estado de la especificación.
* La rama creada o utilizada, cuando aplique.
* La cantidad de requisitos funcionales.
* La cantidad de reglas de negocio.
* La cantidad de escenarios de aceptación.
* La cantidad de casos límite.
* Las aclaraciones pendientes.
* Las secciones marcadas como `No aplica`.
* Las validaciones realizadas.
* El próximo comando recomendado del flujo Spec Kit.

No declares que la especificación está aprobada si no existe aprobación explícita o si contiene aclaraciones materiales.

---

# 27. Reglas no negociables

Durante todo el flujo:

* Respeta el `constitution.md`.
* Realiza una pregunta por interacción.
* Imprime siempre el enunciado completo.
* Presenta cuatro opciones concretas y una opción personalizada.
* Incluye una recomendación contextual cuando sea responsable.
* No repitas información ya proporcionada.
* No inventes requisitos.
* No realices preguntas técnicas.
* No amplíes el alcance.
* No ignores contradicciones.
* No ocultes aclaraciones pendientes.
* No generes el archivo antes de completar el descubrimiento y recibir autorización.
* No modifiques este template.
* No escribas dentro de este archivo información específica de la iniciativa.

El archivo lanzador `.github/prompts/speckit_specify_prompt_interactivo.prompt.md` será el responsable de cargar este template y pasarle la descripción escrita después del comando. Este template central permanece siempre igual.
