/speckit.constitution

Actualiza el archivo `constitution.md` existente en este repositorio.

Esta es una modificación normativa de la constitución. Debes editar la constitución actual directamente, no crear una constitución nueva ni generar un archivo alternativo.

## Objetivo de la actualización

Incorporar reglas obligatorias sobre:

1. La calidad, alcance y verificabilidad de los archivos `spec.md`.
2. La separación entre requisitos funcionales y decisiones técnicas.
3. El manejo de ambigüedades durante `speckit.specify`.
4. El control explícito del alcance de cada iniciativa.
5. La revisión requerida antes de aprobar una especificación.
6. La delimitación de las preguntas que pertenecen a `speckit.specify`.
7. La visibilidad obligatoria de las preguntas interactivas.
8. La obligación de imprimir claramente el enunciado antes de mostrar opciones, recomendaciones, selectores, formularios o widgets.

## Reglas generales de edición

* Conserva íntegramente todos los principios, secciones y reglas existentes, excepto las modificaciones explícitamente solicitadas en este prompt.
* No elimines contenido existente.
* No resumas contenido existente.
* No reformules los bloques que este prompt solicita agregar literalmente.
* Mantén el documento completamente en español.
* Conserva el estilo normativo basado en `DEBE`, `NO DEBE`, `PUEDE` y `Está prohibido`.
* No introduzcas reglas adicionales que no estén indicadas en este prompt.
* No generes una constitución paralela.
* No crees otro archivo.
* No modifiques archivos distintos de `constitution.md`.
* Evita duplicar títulos, secciones, principios o reglas que ya existan.
* Cuando una sección deba reemplazarse, elimina únicamente la versión anterior de esa sección y conserva la nueva versión.
* Revisa el documento completo después de editarlo para comprobar su consistencia.

## Gestión automática de metadata

Utiliza exclusivamente el comportamiento nativo de `speckit.constitution` para administrar la metadata constitucional.

No escribas de forma fija ni calcules manualmente dentro del contenido solicitado:

* La versión actual.
* La versión nueva.
* La fecha de modificación.
* La fecha de ratificación.
* El contenido exacto del `Sync Impact Report`.

No hardcodees números de versión ni fechas provenientes de este prompt.

Permite que `speckit.constitution`:

* Lea la versión actual del documento.
* Determine el impacto semántico de la enmienda.
* Calcule la siguiente versión.
* Conserve la fecha original de ratificación.
* Utilice la fecha real de ejecución como fecha de última modificación.
* Genere o actualice automáticamente el `Sync Impact Report`.
* Mantenga sincronizada la versión reportada en toda la constitución.

Esta actualización agrega un nuevo principio y expande materialmente reglas existentes. Debe considerarse una enmienda de impacto `MINOR` de acuerdo con las reglas de gobierno de la constitución, pero el número resultante debe ser calculado automáticamente por `speckit.constitution`.

No reemplaces manualmente la línea final de metadata. No escribas una fecha o versión específica en el resultado.

El `Sync Impact Report` generado automáticamente debe reflejar conceptualmente:

* La adición del Principio IX: Calidad, Alcance y Verificabilidad de las Especificaciones.
* La ampliación del Modo Interactivo de Preguntas.
* La delimitación funcional de las preguntas de `speckit.specify`.
* La visibilidad obligatoria del enunciado.
* La actualización de los formatos de preguntas con opciones y de preguntas Sí/No.

---

# Modificación 1: agregar el Principio IX

Inserta el siguiente principio inmediatamente después del Principio VIII, `Ingeniería Inversa y Documentación Retrospectiva`, y antes de la sección `Estructura del Repositorio`.

Debes insertar este contenido literalmente, sin resumirlo ni reformularlo:

## IX. Calidad, Alcance y Verificabilidad de las Especificaciones

Toda iniciativa DEBE contar con un `spec.md` que describa de manera suficiente, clara y verificable el comportamiento funcional esperado.

El `spec.md` DEBE concentrarse en definir qué necesita hacer el sistema, qué problema resuelve, quién se beneficia y cómo se comprobará que la iniciativa funciona correctamente.

### Contenido funcional obligatorio

Toda especificación DEBE incluir, cuando aplique:

* El contexto o problema actual.
* El objetivo de la iniciativa.
* Los usuarios, actores o sistemas involucrados.
* Los flujos principales de usuario.
* Los requisitos funcionales.
* Las reglas de negocio.
* Las validaciones funcionales.
* Los escenarios de aceptación.
* Los casos límite relevantes.
* El alcance incluido.
* Los elementos fuera de alcance.
* Los criterios objetivos de éxito.
* Las dependencias y supuestos funcionales conocidos.

Una sección PUEDE omitirse únicamente cuando no sea aplicable a la iniciativa. No se deben crear secciones con contenido artificial solo para completar una plantilla.

### Separación entre especificación e implementación

El `spec.md` DEBE describir comportamientos, resultados y restricciones observables, sin convertir la especificación en un documento de diseño técnico.

Salvo que formen parte explícita de un contrato funcional externo, el `spec.md` NO DEBE definir:

* Clases, interfaces, métodos o namespaces.
* Frameworks, librerías o paquetes.
* Patrones de arquitectura o diseño.
* Endpoints, rutas HTTP o métodos HTTP concretos.
* Tablas, columnas, migraciones o esquemas de base de datos.
* Componentes internos de frontend.
* Estructuras de carpetas o nombres de archivos de implementación.
* Estrategias de persistencia.
* Configuraciones específicas de infraestructura.
* Fragmentos de código como solución propuesta.

Las decisiones técnicas y de implementación DEBEN documentarse posteriormente en `plan.md`.

Las restricciones globales ya definidas por esta constitución, incluido el stack tecnológico obligatorio, se consideran heredadas y NO DEBEN repetirse innecesariamente en cada `spec.md`.

### Requisitos observables y verificables

Todo requisito funcional DEBE expresar un comportamiento que pueda comprobarse mediante revisión, prueba automatizada o validación manual.

Se DEBEN evitar requisitos ambiguos o subjetivos como:

* El sistema debe ser rápido.
* La interfaz debe ser moderna.
* La experiencia debe ser intuitiva.
* La solución debe ser escalable.
* El proceso debe ser sencillo.

Cuando una característica de calidad sea necesaria, DEBE expresarse mediante una condición verificable, un límite medible o un resultado observable.

Cada requisito DEBE permitir determinar objetivamente si fue cumplido o no.

### Escenarios de aceptación

Los escenarios de aceptación DEBEN describirse utilizando una estructura equivalente a:

* Dado un estado inicial.
* Cuando un usuario, actor o sistema realiza una acción.
* Entonces ocurre un resultado observable.

Cada iniciativa DEBE cubrir como mínimo, cuando sean aplicables:

1. El flujo principal exitoso.
2. Las validaciones funcionales relevantes.
3. Los recursos inexistentes o estados inválidos.
4. Los errores funcionales esperados.
5. Los casos límite con impacto en el usuario o el negocio.

No se deben agregar escenarios que describan detalles internos de implementación sin relevancia funcional.

### Manejo de ambigüedades

Está prohibido inventar requisitos, reglas de negocio, datos, actores, restricciones o comportamientos que no estén respaldados por:

1. El input proporcionado por el humano.
2. Una decisión respondida durante el modo interactivo.
3. Una especificación vigente y aprobada.
4. Esta constitución.
5. Evidencia verificable del repositorio, únicamente cuando se trate de una iniciativa retrospectiva regulada por el Principio VIII.

Cuando falte una decisión funcional material, `speckit.specify` DEBE solicitarla mediante el Modo Interactivo de Preguntas.

Si la decisión no puede resolverse durante la generación inicial, se DEBE registrar temporalmente utilizando el formato:

`[NEEDS CLARIFICATION: pregunta concreta que debe resolverse]`

La pregunta DEBE indicar exactamente qué decisión falta y por qué afecta el comportamiento esperado.

Una especificación NO PUEDE pasar al estado Aprobada mientras contenga una aclaración pendiente que afecte:

* El objetivo.
* El alcance.
* Las reglas de negocio.
* Los datos requeridos.
* El flujo principal.
* Los escenarios de aceptación.
* Los criterios de éxito.

Las aclaraciones sin impacto material PUEDEN documentarse como supuestos explícitos, siempre que no contradigan el input, otra spec aprobada o esta constitución.

### Control de alcance

Toda especificación DEBE identificar explícitamente:

* Qué comportamiento está incluido.
* Qué comportamiento está fuera de alcance.
* Qué capacidades se reservan para iniciativas futuras.

Está prohibido incorporar funcionalidades adicionales por conveniencia técnica, interpretación del agente o anticipación de necesidades futuras.

Un requisito adicional solo PUEDE incorporarse cuando:

* Sea indispensable para completar el flujo solicitado.
* Derive directamente de una regla de negocio confirmada.
* Sea requerido por una spec aprobada.
* Sea aceptado explícitamente por el humano durante el modo interactivo.

Las oportunidades de mejora que no pertenezcan al alcance actual DEBEN registrarse como recomendaciones futuras y no como requisitos obligatorios.

### Criterios de éxito

Los criterios de éxito DEBEN describir las condiciones objetivas que permitirán considerar completada la iniciativa.

Un criterio de éxito NO DEBE limitarse a indicar que:

* El código fue escrito.
* El endpoint fue creado.
* El componente fue implementado.
* La tarea fue cerrada.
* La aplicación compila.

Los criterios DEBEN reflejar resultados funcionales o evidencia verificable de que el comportamiento solicitado está disponible y funciona correctamente.

Para iniciativas retrospectivas, los criterios de éxito DEBEN alinearse también con las reglas de evidencia establecidas en el Principio VIII.

### Revisión previa a la aprobación

Antes de cambiar una especificación de Borrador a Aprobada, el agente DEBE verificar que:

* El problema y el objetivo estén claramente definidos.
* Los requisitos sean observables y verificables.
* Las reglas de negocio estén respaldadas por información confirmada.
* Los escenarios de aceptación cubran los comportamientos principales.
* El alcance y los elementos fuera de alcance estén identificados.
* No existan contradicciones con esta constitución.
* No existan contradicciones no resueltas con specs aprobadas.
* No existan aclaraciones materiales pendientes.
* No se hayan introducido decisiones técnicas que correspondan a `plan.md`.

Si alguna condición no se cumple, la especificación DEBE permanecer en estado Borrador y el agente DEBE reportar el motivo concreto.

---

# Modificación 2: delimitar las preguntas de speckit.specify

Dentro de `Modo Interactivo de Preguntas`, inmediatamente después de los párrafos que definen cuándo operan interactivamente `speckit.specify`, `speckit.clarify` y `speckit.plan`, inserta literalmente:

Durante `speckit.specify`, las preguntas DEBEN limitarse a decisiones que afecten el problema, el objetivo, los actores, el alcance, las reglas de negocio, las validaciones, los comportamientos observables, los escenarios de aceptación o los criterios de éxito.

Está prohibido utilizar `speckit.specify` para preguntar por frameworks, librerías, patrones de arquitectura, clases, endpoints, tablas, carpetas u otras decisiones de implementación que correspondan a `speckit.plan`.

El agente NO DEBE preguntar por decisiones que ya estén resueltas explícitamente en esta constitución, en el input del humano o en una spec aprobada aplicable.

Cuando una respuesta ya pueda deducirse inequívocamente de una fuente normativa, el agente DEBE utilizarla y registrar la fuente correspondiente, en lugar de volver a preguntarla.

---

# Modificación 3: reforzar la regla general de las preguntas

Dentro de la sección `Modo Interactivo de Preguntas`, localiza la regla existente que indica que toda pregunta debe redactarse de forma clara, directa y entendible.

Reemplaza esa regla literalmente por:

Toda pregunta DEBE redactarse e imprimirse de forma visible, completa, clara, directa y entendible por el humano antes de mostrar cualquier opción, recomendación o control interactivo. El usuario nunca DEBE tener que deducir el enunciado a partir de las respuestas disponibles.

---

# Modificación 4: agregar la visibilidad obligatoria del enunciado

Dentro de la sección `Modo Interactivo de Preguntas`, inmediatamente después de la oración:

`Cuando un comando opera en modo interactivo, DEBE seguir este protocolo sin excepción.`

Inserta literalmente la siguiente subsección:

### Visibilidad obligatoria del enunciado

Antes de presentar opciones o solicitar una respuesta, el agente DEBE imprimir de forma visible y completa el enunciado de la pregunta.

Cada interacción DEBE mostrar, en este orden:

1. El número y el tema de la pregunta.
2. El enunciado completo de la pregunta.
3. La explicación de por qué la decisión es importante.
4. Las opciones disponibles, cuando corresponda.
5. Las instrucciones para responder.

Está prohibido mostrar únicamente:

* Las opciones disponibles.
* La opción recomendada.
* Una tabla de alternativas sin el enunciado.
* Un selector, widget o control interactivo sin texto visible que explique la pregunta.
* Recomendaciones sin indicar claramente qué decisión debe tomar el usuario.

El enunciado DEBE aparecer como texto legible en la salida principal del agente. No es suficiente incluirlo únicamente en:

* Metadatos internos.
* Parámetros de una herramienta.
* Texto oculto.
* El título de un componente interactivo.
* Un placeholder.
* Una descripción que solo aparece al colocar el cursor.
* Una interfaz que pueda no renderizarse en todos los clientes.

Las opciones A, B, C y D NO DEBEN mostrarse hasta que el enunciado completo de la pregunta haya sido impreso.

#### Formato visible obligatorio

Toda pregunta con opciones DEBE renderizarse de forma equivalente al siguiente formato:

**Pregunta [N de TOTAL] — [tema corto]**

**Pregunta:**
[Enunciado completo, claro y concreto de la decisión que debe tomar el usuario]

**Por qué importa:**
[Explicación breve del impacto de esta decisión]

**Opciones:**

A) [opción concreta]
B) [opción concreta] — Recomendada
C) [opción concreta]
D) Otro — escribe tu respuesta

**Cómo responder:**
Responde con A, B, C o D, o escribe directamente tu decisión.

La palabra **“Pregunta:”** y el enunciado correspondiente DEBEN aparecer siempre, aunque el título `Pregunta [N de TOTAL]` ya proporcione contexto.

#### Uso de controles o componentes interactivos

Cuando el agente utilice un widget, selector, formulario, herramienta interactiva o componente visual para presentar las opciones, también DEBE imprimir el enunciado completo como texto visible antes del componente.

El control interactivo PUEDE complementar la pregunta, pero NO PUEDE reemplazar su enunciado.

Cuando el entorno no garantice que el componente interactivo será visible, el agente DEBE utilizar Markdown o texto plano como mecanismo principal y considerar el componente interactivo únicamente como apoyo opcional.

#### Verificación antes de esperar respuesta

Antes de detenerse para esperar la respuesta del usuario, el agente DEBE comprobar que la salida visible contiene:

* El identificador de la pregunta.
* La palabra `Pregunta:`.
* El enunciado completo.
* La explicación `Por qué importa:`.
* Todas las opciones disponibles.
* La instrucción para responder.

Si alguno de estos elementos no fue mostrado, quedó truncado o no pudo renderizarse, el agente NO DEBE esperar la respuesta.

En ese caso, DEBE volver a imprimir inmediatamente la pregunta completa utilizando Markdown o texto plano.

#### Prohibición de preguntas implícitas

Está prohibido obligar al usuario a deducir la pregunta a partir de las opciones.

Ejemplo prohibido:

A) 768px
B) 1024px — Recomendado
C) 1280px
D) Otro

Ejemplo obligatorio:

**Pregunta 2 de 4 — Ancho para tablet**

**Pregunta:**
¿A partir de qué ancho debe activarse el diseño para tablet?

**Por qué importa:**
Esta decisión determina cuándo cambia la distribución de los componentes.

**Opciones:**

A) 768px
B) 1024px — Recomendada
C) 1280px
D) Otro — escribe tu respuesta

**Cómo responder:**
Responde con A, B, C o D, o escribe directamente el ancho deseado.

---

# Modificación 5: actualizar el formato de pregunta con opciones

Dentro de la subsección `Formato de pregunta con opciones`, reemplaza el formato actual completo por el siguiente contenido:

### Formato de pregunta con opciones

**Pregunta [N de TOTAL] — [tema corto]**

**Pregunta:**
[Enunciado completo, claro, concreto y entendible de la pregunta]

**Por qué importa:**
[Una línea que explique el impacto funcional de la decisión]

**Opciones:**

A) [opción concreta con valor específico]
B) [opción concreta con valor específico] — Recomendada
C) [opción concreta con valor específico]
D) Otro — escribe tu respuesta

**Cómo responder:**
Responde con A, B, C o D, o escribe directamente tu decisión.

El enunciado completo DEBE estar presente y visible antes de las opciones. El título, las opciones y la recomendación NO sustituyen el enunciado de la pregunta.

---

# Modificación 6: actualizar el formato de pregunta Sí/No

Dentro de la subsección `Formato de pregunta Sí/No`, reemplaza el formato actual completo por el siguiente contenido:

### Formato de pregunta Sí/No

**Pregunta [N de TOTAL] — [tema corto]**

**Pregunta:**
[Enunciado completo, claro y entendible de la pregunta]

**Por qué importa:**
[Una línea que explique el impacto funcional de la decisión]

**Opciones:**

S) Sí — Recomendado
N) No

**Cómo responder:**
Responde S o N.

El enunciado completo DEBE estar presente y visible antes de las opciones. Las opciones `S` y `N` nunca DEBEN mostrarse de manera aislada.

---

# Validaciones obligatorias después de editar

Después de aplicar las modificaciones, revisa el documento completo y confirma que:

1. Se conservaron todos los principios existentes sin eliminaciones accidentales.
2. El nuevo Principio IX aparece inmediatamente después del Principio VIII.
3. Existe una única sección `Modo Interactivo de Preguntas`.
4. La delimitación funcional de `speckit.specify` está incluida.
5. `speckit.specify` no puede utilizarse para resolver decisiones técnicas propias de `speckit.plan`.
6. La regla de visibilidad aparece antes de los formatos de preguntas.
7. Toda pregunta debe imprimir el enunciado completo antes de las opciones.
8. La palabra `Pregunta:` es obligatoria en el formato visible.
9. El enunciado completo es obligatorio tanto para preguntas con opciones como para preguntas Sí/No.
10. Ninguna regla permite mostrar únicamente opciones, recomendaciones o controles interactivos.
11. Los widgets, formularios y selectores se definen como apoyo y no como sustituto del enunciado.
12. Si un componente visual no se renderiza, la pregunta debe volver a imprimirse mediante Markdown o texto plano.
13. Los requisitos de `spec.md` se mantienen separados de las decisiones de `plan.md`.
14. Las aclaraciones materiales bloquean la aprobación de una especificación.
15. El alcance y los elementos fuera de alcance son obligatorios cuando resulten aplicables.
16. No se introdujeron duplicaciones accidentales.
17. No se modificaron archivos distintos de `constitution.md`.
18. La metadata fue administrada mediante el comportamiento nativo de `speckit.constitution`.
19. No se hardcodearon números de versión ni fechas.
20. La versión, fecha y `Sync Impact Report` quedaron sincronizados automáticamente.

## Resultado esperado

Al terminar:

* Guarda los cambios en el `constitution.md` existente.
* Muestra la ruta del archivo modificado.
* Resume los principios y secciones agregados o modificados.
* Indica el tipo de impacto semántico detectado, sin haber hardcodeado el número resultante.
* Confirma que la metadata fue generada automáticamente por `speckit.constitution`.
* Muestra el resultado de las validaciones.
* Presenta el diff completo de los cambios realizados.
