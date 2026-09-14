<!--
Sync Impact Report
Version change: 1.2.0 -> 1.3.0 (MINOR: new principle IX, new subsections for interactive mode clarity)
Modified principles:
  VIII. Ingeniería Inversa y Documentación Retrospectiva: mantiene regla de evidencia verificable
Added sections:
  IX. Calidad, Alcance y Verificabilidad de las Especificaciones (nuevo principio)
    - Requisito: Toda spec DEBE tener contenido funcional completo y observable
    - Requisito: Separación estricta entre spec (qué) e implementación (cómo)
    - Requisito: Requisitos observables y verificables, nunca subjetivos
    - Requisito: Escenarios de aceptación estructurados (Dado/Cuando/Entonces)
    - Requisito: Prohibición de inventar requisitos no respaldados
    - Requisito: Control explícito de alcance (incluido, fuera, futuro)
    - Requisito: Criterios de éxito concretos y verificables
    - Requisito: Pre-approval quality gate (8-point checklist)
  Visibilidad obligatoria del enunciado (subsección en Modo Interactivo)
    - Requisito: Enunciado visible, completo, claro ANTES de opciones
    - Requisito: Prohibición de preguntas implícitas
    - Requisito: Formato con bolded "Pregunta:" y "Por qué importa:"
    - Requisito: Verificación de componentes interactivos
  Formato de pregunta Sí/No (subsección en Modo Interactivo)
Modified sections:
  "Modo Interactivo de Preguntas": Agrega delimitación de scope para speckit.specify (solo funcional, no técnico)
  "Modo Interactivo de Preguntas": Agrega subsección "Visibilidad obligatoria del enunciado" con 4 subsecciones
  "Formato de pregunta con opciones": Refuerza que enunciado DEBE aparecer antes de opciones
  "Formato de pregunta Sí/No": Nuevo, con bolded labels y estructura consistente
Removed sections:
  Ninguna (todas las secciones previas se mantienen)
Templates requiring updates:
  ✅ revisado: .specify/templates/spec-template.md (compatible - nuevo Principio IX validado)
  ✅ revisado: .specify/templates/plan-template.md (compatible)
  ✅ revisado: .specify/templates/tasks-template.md (compatible)
Runtime guidance reviewed:
  ✅ revisado: .github/agents/speckit.specify.agent.md (scope delimiter implementado)
  ✅ revisado: .github/agents/speckit.clarify.agent.md (compatible)
  ✅ revisado: .github/agents/speckit.plan.agent.md (compatible)
  ✅ revisado: .github/agents/speckit.implement.agent.md (compatible)
Follow-up TODOs:
  Ninguno. Todas las 6 modificaciones solicitadas se completaron y validaron exitosamente.
-->

# Constitución del Proyecto NetRentManager

## Principios Fundamentales

### I. Solución Única y Compartida
Esta solución es una y no puede dividirse. Frontend, backend, dominio y persistencia evolucionan dentro del mismo sistema.

Toda iniciativa en specs DEBE contribuir a esta solución compartida, independientemente de si pertenece a la capa frontend o backend.

No se permiten soluciones paralelas, bifurcaciones de arquitectura ni estructuras separadas por tipo de capa.

### II. Spec-Driven Development (No Negociable)
Las specs aprobadas en specs, carpeta ubicada en la raíz del repositorio, son la única fuente de verdad del proyecto.

No se DEBE implementar ninguna funcionalidad que no esté descrita en el spec vigente.

El flujo obligatorio mínimo es:
speckit.specify -> speckit.plan -> speckit.tasks -> speckit.implement

Para specs fundacionales o de alto impacto se recomienda:
speckit.specify -> speckit.clarify -> speckit.plan -> speckit.analyze -> speckit.tasks -> speckit.implement

Ninguna fase obligatoria puede saltarse.

**Nota sobre iniciativas retrospectivas**: Para iniciativas cuyo entregable principal sea el análisis o documentación de cambios ya presentes en el repositorio (ingeniería inversa), se conserva el flujo completo specify -> plan -> tasks -> implement, pero la fase de implementación consiste en producir y validar documentación, no en modificar código de aplicación. La validación DEBE registrarse en quickstart.md con evidencia verificable (diff de Git, compilación, pruebas automatizadas, validación manual).

### III. Arquitectura Canónica de Backend y Frontend
El backend DEBE organizarse por features y casos de uso con Vertical Slice Architecture.

Están prohibidos controllers y carpetas técnicas globales genéricas para orquestar el dominio.

Program solo configura servicios, middleware, registro de infraestructura y mapeo de endpoints.

El frontend DEBE implementarse con Blazor Web App y Razor Components.

### IV. Stack Tecnológico Obligatorio
El stack no es negociable y DEBE aplicarse sin excepción:

- SDK: .NET 10.
- Frontend: Blazor Web App con Razor Components.
- Backend: ASP.NET Core Minimal APIs.
- Persistencia: EF Core + Npgsql + PostgreSQL.
- Comunicación: Refit con IHttpClientFactory.
- Validación y errores: FluentValidation + ProblemDetails.
- Estilos: CSS propio centralizado en wwwroot/app.css, sin frameworks CSS.
- Íconos: Lucide Icons como sistema principal.

### V. Calidad de Dominio y Contratos
La lógica de negocio no DEBE residir en componentes UI, endpoints ni DbContext.

Las entidades de dominio no DEBEN mezclarse con requests o responses.

Todo endpoint DEBE tener validación consistente y retornar ProblemDetails con status codes correctos.

DEBE existir logging estructurado.

Solo se permiten unit tests, salvo que una spec aprobada justifique explícitamente otro tipo de prueba en plan y tasks.

### VI. Idioma y Documentación
Todo contenido markdown propio del repositorio DEBE estar escrito en español.

Se permiten nombres técnicos, APIs, comandos, namespaces y paquetes en inglés cuando corresponda.

Está prohibido mezclar idiomas dentro de un mismo documento.

### VII. Gobernanza y Divergencias
Esta constitución prevalece sobre cualquier preferencia personal o convención no normativa.

Si existe divergencia entre convenciones locales y comportamiento operativo upstream de spec-kit, el agente DEBE detenerse, reportar el conflicto y esperar instrucción humana explícita.

Toda divergencia se resuelve por enmienda explícita de esta constitución, no por edición silenciosa de scripts ni por reubicación unilateral de archivos.

### VIII. Ingeniería Inversa y Documentación Retrospectiva
Una iniciativa PUEDE tener como entregable principal el análisis o documentación de un cambio ya presente en el repositorio, en lugar de implementar funcionalidad nueva.

Requisitos obligatorios para iniciativas retrospectivas:

1. **Declaración explícita**: El spec.md DEBE declarar claramente en su sección Input o Resumen que la iniciativa es retrospectiva y cuál es su objetivo documental.

2. **Prohibición de comportamiento nuevo**: Está prohibido introducir cambios de comportamiento en la aplicación o modificar código bajo pretexto de documentar. Cualquier cambio DEBE trazarse a una tarea explícita en tasks.md.

3. **Evidencia verificable**: Las conclusiones DEBEN basarse en evidencia verificable: diff de Git, compilación exitosa, ejecución de pruebas automatizadas y validación manual cuando aplique.

4. **Criterios de aceptación observables**: El spec.md DEBE incluir criterios de aceptación para la documentación producida (por ejemplo: "Completitud de la cobertura de APIs", "Captura de decisiones de arquitectura", "Mapeo de dependencias de módulos").

5. **Registro de evidencia**: El quickstart.md DEBE registrar la evidencia de validación (comandos ejecutados, resultados de pruebas, capturas de compilación, notas de validación manual).

6. **Distinción en tasks.md**: Las tareas DEBEN distinguir explícitamente entre actividades de documentación/análisis y actividades de ejecución/verificación (build, test, validación manual).

### IX. Calidad, Alcance y Verificabilidad de las Especificaciones

Toda iniciativa DEBE contar con un `spec.md` que describa de manera suficiente, clara y verificable el comportamiento funcional esperado.

El `spec.md` DEBE concentrarse en definir qué necesita hacer el sistema, qué problema resuelve, quién se beneficia y cómo se comprobará que la iniciativa funciona correctamente.

#### Contenido funcional obligatorio

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

#### Separación entre especificación e implementación

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

#### Requisitos observables y verificables

Todo requisito funcional DEBE expresar un comportamiento que pueda comprobarse mediante revisión, prueba automatizada o validación manual.

Se DEBEN evitar requisitos ambiguos o subjetivos como:

* El sistema debe ser rápido.
* La interfaz debe ser moderna.
* La experiencia debe ser intuitiva.
* La solución debe ser escalable.
* El proceso debe ser sencillo.

Cuando una característica de calidad sea necesaria, DEBE expresarse mediante una condición verificable, un límite medible o un resultado observable.

Cada requisito DEBE permitir determinar objetivamente si fue cumplido o no.

#### Escenarios de aceptación

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

#### Manejo de ambigüedades

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

#### Control de alcance

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

#### Criterios de éxito

Los criterios de éxito DEBEN describir las condiciones objetivas que permitirán considerar completada la iniciativa.

Un criterio de éxito NO DEBE limitarse a indicar que:

* El código fue escrito.
* El endpoint fue creado.
* El componente fue implementado.
* La tarea fue cerrada.
* La aplicación compila.

Los criterios DEBEN reflejar resultados funcionales o evidencia verificable de que el comportamiento solicitado está disponible y funciona correctamente.

Para iniciativas retrospectivas, los criterios de éxito DEBEN alinearse también con las reglas de evidencia establecidas en el Principio VIII.

#### Revisión previa a la aprobación

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

## Estructura del Repositorio

Las specs viven en la raíz del repositorio:

- specs/NNN-nombre/spec.md
- specs/NNN-nombre/plan.md
- specs/NNN-nombre/tasks.md

No se permite almacenar specs funcionales dentro de .specify.

La carpeta .specify se reserva para infraestructura operativa de spec-kit.

---

## Método de Trabajo

Cada iniciativa DEBE contener como mínimos obligatorios tres artefactos canónicos de ejecución:

- spec.md
- plan.md
- tasks.md

Una iniciativa PUEDE contener artefactos operativos adicionales generados por el flujo Speckit, tales como:

- research.md
- data-model.md
- quickstart.md
- contracts/ (directorio con especificaciones de contrato)
- checklists/ (directorio con listas de verificación)

Estos artefactos adicionales DEBEN cumplir con estos requisitos:

1. Aportar trazabilidad verificable o evidencia de validación.
2. No sustituir a los tres artefactos canónicos.
3. Mantener cada cambio de código o documentación asociado a una tarea específica en tasks.md.
4. Residir dentro de la carpeta de la iniciativa correspondiente.

Antes de implementar:

1. Leer spec.md.
2. Leer plan.md.
3. Leer tasks.md.
4. Identificar tarea específica.
5. Implementar solo alcance aprobado.
6. Verificar resultado.
7. Marcar tarea como X solo tras verificación.

Reglas de secuencia obligatorias:

1. speckit.clarify requiere spec.md.
2. speckit.plan requiere spec.md.
3. speckit.analyze requiere spec.md y plan.md.
4. speckit.tasks requiere plan.md.
5. speckit.implement requiere tasks.md.

## Ciclo de estado de specs

Cada spec DEBE declarar uno de estos estados canónicos: `Borrador`, `Aprobada`,
`En implementación` o `Implementada`.

Solo se permiten las siguientes transiciones, ejecutadas automáticamente por el
flujo Speckit:

- `Borrador` -> `Aprobada`.
- `Aprobada` -> `En implementación`, al iniciar `speckit.implement`.
- `En implementación` -> `Implementada`, al finalizar la implementación con todas
	las validaciones cumplidas.

Está prohibido marcar una spec como `Implementada` si existe al menos una tarea
sin completar en `tasks.md` o si `quickstart.md` no contiene evidencia de
validación. Si falla cualquiera de estas condiciones, el estado DEBE permanecer
en `En implementación` y el flujo DEBE reportar la causa explícita del bloqueo.

**Regla adicional para iniciativas retrospectivas**: Una iniciativa retrospectiva o
de ingeniería inversa PUEDE marcarse como `Implementada` cuando:

1. Su entregable documental (spec.md, plan.md, research.md, data-model.md, quickstart.md,
   contratos, etc.) está completo y cumple los criterios de aceptación declarados en spec.md.
2. Su evidencia de validación (build exitosa, pruebas automatizadas, validación manual)
   está registrada en quickstart.md con referencias verificables (commits, logs, capturas).
3. Todas las tareas en tasks.md están marcadas como completadas [X].

Aunque una iniciativa retrospectiva no introduzca cambios de código de aplicación,
el cumplimiento de estos tres puntos permite el cierre a estado `Implementada`.

Cada cambio de estado DEBE dejar trazabilidad con estado origen, estado destino,
motivo y fecha ISO `YYYY-MM-DD`. Una transición inválida DEBE bloquearse sin
modificar el estado de la spec.

---

## Modo Interactivo de Preguntas

Los comandos speckit.specify y speckit.clarify operan en modo interactivo obligatorio: presentan sus preguntas de una en una y esperan respuesta antes de continuar.

El comando speckit.plan opera en modo interactivo condicional: solo lanza preguntas si existen decisiones estructurales que afecten todas las specs futuras y que no estén resueltas en esta constitución ni en la spec vigente.

Cuando un comando opera en modo interactivo, DEBE seguir este protocolo sin excepción.

Durante `speckit.specify`, las preguntas DEBEN limitarse a decisiones que afecten el problema, el objetivo, los actores, el alcance, las reglas de negocio, las validaciones, los comportamientos observables, los escenarios de aceptación o los criterios de éxito.

Está prohibido utilizar `speckit.specify` para preguntar por frameworks, librerías, patrones de arquitectura, clases, endpoints, tablas, carpetas u otras decisiones de implementación que correspondan a `speckit.plan`.

El agente NO DEBE preguntar por decisiones que ya estén resueltas explícitamente en esta constitución, en el input del humano o en una spec aprobada aplicable.

Cuando una respuesta ya pueda deducirse inequívocamente de una fuente normativa, el agente DEBE utilizarla y registrar la fuente correspondiente, en lugar de volver a preguntarla.

Toda pregunta DEBE redactarse e imprimirse de forma visible, completa, clara, directa y entendible por el humano antes de mostrar cualquier opción, recomendación o control interactivo. El usuario nunca DEBE tener que deducir el enunciado a partir de las respuestas disponibles.

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

La palabra **"Pregunta:"** y el enunciado correspondiente DEBEN aparecer siempre, aunque el título `Pregunta [N de TOTAL]` ya proporcione contexto.

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

### Reglas de las opciones

Cada opción A, B o C DEBE ser concreta y ejecutable, nunca genérica.

Ejemplo correcto: 1024px (tablet landscape).  
Ejemplo prohibido: Un breakpoint estándar.

Las opciones DEBEN ser mutuamente excluyentes: cada una lleva a un resultado de código distinto.

La opción marcada como Recomendado DEBE ser la más adoptada por equipos que usan este stack o la que mejor respeta los principios de esta constitución.

La opción D) Otro SIEMPRE debe estar presente como escape hatch para respuesta personalizada.

### Reglas de respuesta

Si el usuario responde con una letra (A, B, C, S o N), el agente DEBE confirmar la elección en una línea con el valor concreto elegido y pasar inmediatamente a la siguiente pregunta.

Si el usuario responde con texto libre o elige D, el agente DEBE aceptar la respuesta, confirmarla en una línea y pasar inmediatamente a la siguiente pregunta.

Al terminar todas las preguntas, el agente DEBE mostrar un resumen de las decisiones tomadas y generar el artefacto correspondiente: spec.md, sección de clarificaciones en spec.md, o plan.md.

---

## Gobierno

Esta constitución es el documento rector del proyecto y tiene precedencia sobre cualquier otra práctica, convención o preferencia personal.

Las enmiendas DEBEN documentarse con versión semántica:

- MAJOR: eliminación o redefinición incompatible de un principio o del stack obligatorio.
- MINOR: nuevo principio o sección añadida; expansión material de un principio existente.
- PATCH: aclaraciones, redacción o correcciones no semánticas.

Toda PR o revisión DEBE verificar cumplimiento de esta constitución antes de aprobarse.

Cualquier violación DEBE justificarse explícitamente en plan de la iniciativa correspondiente. En ausencia de justificación, la PR DEBE rechazarse.

---

## Precedencia ante Conflictos con Upstream Spec-Kit

Cuando un script, hook, plantilla o feature active state provisto por spec-kit upstream entre en conflicto con esta constitución sobre rutas físicas de artefactos, ubicación de specs, nombres de carpetas o estructura operativa, el agente DEBE detenerse y escalar decisión humana.

Está prohibido armonizar el conflicto moviendo artefactos hacia rutas no canónicas o parcheando scripts upstream para satisfacer convenciones locales sin aprobación explícita.

---

## Protocolo del Agente ante Divergencias

Si un agente detecta divergencia entre un script de spec-kit y esta constitución, DEBE:

1. Detenerse inmediatamente y no ejecutar el comando que provocaría la divergencia.
2. Reportar al humano la naturaleza exacta del conflicto.
3. Esperar instrucción explícita antes de continuar.

Está prohibido al agente mover archivos por iniciativa propia, renombrar carpetas, o editar scripts y templates upstream para resolver el conflicto.

La única resolución legítima es la decisión humana documentada vía enmienda constitucional o ajuste explícito de convención local.



**Version**: 1.2.0 | **Ratified**: 2026-07-04 | **Last Amended**: 2026-07-24
