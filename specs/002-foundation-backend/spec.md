# Especificación de la Funcionalidad: Fundación Interna del Backend

**Rama de la funcionalidad**: `002-foundation-backend`

**Creado**: 2026-09-10

**Estado**: Implementada

**Trazabilidad de estado**: `Aprobada` -> `En implementación` | Motivo: inicio de
`speckit.implement` tras validar prerrequisitos y checklist | Fecha: 2026-09-10

`En implementación` -> `Implementada` | Motivo: tareas T001-T033 completadas,
`tasks.md` sin pendientes y evidencia registrada en `quickstart.md` con
restore/build/test en verde | Fecha: 2026-09-10

**Entrada**: Descripción de usuario: "Crear la especificación 002-foundation-backend para establecer la estructura base interna del backend y sus capacidades transversales, sin entidades ni features de producto."

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Descubrir y mapear slices automáticamente (Prioridad: P1)

Como equipo de desarrollo, necesito una infraestructura base que descubra y mapee endpoints Minimal API mediante el contrato `ISlice`, para agregar futuras capacidades sin modificar `Program.cs` ni mantener registros manuales.

**Por qué esta prioridad**: El descubrimiento automático define el punto de extensión principal del backend y permite que las siguientes iniciativas agreguen slices de forma independiente.

**Prueba independiente**: Se define una implementación de `ISlice` exclusivamente en el proyecto de pruebas, se ejecuta el registro por assembly y se verifica que se descubre una sola vez; después se verifica que el endpoint `/health` se mapea mediante el mecanismo centralizado.

**Escenarios de aceptación**:

1. **Dado** un assembly que contiene una clase de prueba que implementa `ISlice`, **cuando** se ejecuta el registro de slices, **entonces** la clase se registra bajo `ISlice` una sola vez.
2. **Dado** el endpoint `/health` implementado como slice, **cuando** se inicia el backend, **entonces** responde correctamente sin requerir un registro manual adicional en `Program.cs`.
3. **Dado** un nuevo slice compatible, **cuando** se incorpora al assembly correspondiente, **entonces** el mapeo centralizado lo considera sin descubrir por nombre de clase ni usar un registro manual.

### Historia de Usuario 2 - Aplicar capacidades transversales de validación y handlers (Prioridad: P1)

Como equipo de desarrollo, necesito que validators y handlers futuros se registren automáticamente por assembly scanning, para que las features puedan concentrarse en sus casos de uso sin configuración individual repetitiva.

**Por qué esta prioridad**: La validación y el registro de handlers son capacidades compartidas que deben quedar establecidas antes de construir features funcionales.

**Prueba independiente**: Se ejecuta una validación con un `IValidator<T>` de prueba y otra sin validator; se comprueba respectivamente la ejecución de la validación y el pass-through. También se verifica el auto-registro de handlers mediante una clase de prueba que implemente `IHandler`.

**Escenarios de aceptación**:

1. **Dado** un request con un `IValidator<T>` registrado, **cuando** se procesa mediante el filtro de validación, **entonces** se ejecuta la validación correspondiente.
2. **Dado** un request sin validator registrado, **cuando** se procesa mediante el filtro, **entonces** continúa sin error de configuración ni fallo de validación inexistente.
3. **Dado** un validator que devuelve errores, **cuando** se procesa el request, **entonces** la respuesta es un `ValidationProblemDetails` con HTTP 400.
4. **Dado** un handler de prueba que implementa `IHandler`, **cuando** se ejecuta el registro por assembly scanning, **entonces** queda disponible bajo su contrato sin registro individual.

### Historia de Usuario 3 - Uniformar errores esperados y mantener un arranque mínimo (Prioridad: P1)

Como responsable técnico, necesito una representación común de resultados y errores, junto con logging estructurado y un `Program.cs` limitado a la composición de infraestructura, para que las futuras features tengan contratos consistentes y un punto de arranque controlado.

**Por qué esta prioridad**: Los contratos de error y el límite del arranque afectan a todas las futuras features y evitan que cada slice implemente una variante incompatible.

**Prueba independiente**: Se construye un resultado de error de prueba, se convierte mediante el mapeo centralizado y se verifica el status code y payload; además se inspecciona `Program.cs` para confirmar que solo compone servicios, middleware, infraestructura y mapeo centralizado.

**Escenarios de aceptación**:

1. **Dado** un `Result` de error con status y detalles definidos, **cuando** se convierte a `ProblemDetails`, **entonces** conserva el status code y el payload esperado.
2. **Dado** un evento relevante de infraestructura, **cuando** se registra, **entonces** utiliza logging estructurado mediante `ILogger`.
3. **Dado** el backend configurado, **cuando** se inspecciona `Program.cs`, **entonces** no contiene lógica de negocio ni registros individuales de slices, validators o handlers.

### Casos límite

- Si se intenta registrar dos veces el mismo assembly o el mismo tipo, el contenedor no debe contener duplicados del contrato correspondiente.
- Si no existe un `IValidator<T>` para un request, el filtro debe dejar pasar la ejecución sin lanzar una excepción por ausencia de configuración.
- Si un validator devuelve varios errores para un mismo request, todos deben quedar representados en la respuesta de validación HTTP 400.
- Si el mapeo recibe un resultado de error sin detalles opcionales, debe producir un `ProblemDetails` válido con el status code disponible.
- Si una clase de prueba de infraestructura se ubica en producción por error, el alcance de esta iniciativa debe impedir que se considere una feature de negocio.
- Si falta `global.json`, se debe bloquear la iniciativa; la versión de plataforma se deriva del archivo existente y no se sustituye con una configuración nueva.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El backend DEBE organizar su estructura interna para soportar Vertical Slice Architecture y separar las capacidades transversales de futuras features.
- **RF-002**: El backend DEBE definir el contrato `ISlice` para endpoints Minimal API.
- **RF-003**: El backend DEBE descubrir e inscribir implementaciones de `ISlice` mediante assembly scanning y reflection, sin registries manuales ni descubrimiento por nombre de clase.
- **RF-004**: El backend DEBE mapear los endpoints descubiertos mediante una operación centralizada, de modo que agregar un slice no requiera modificar `Program.cs`.
- **RF-005**: El backend DEBE registrar automáticamente handlers que implementen el marker interface `IHandler` mediante assembly scanning, sin implementar handlers concretos de negocio en esta iniciativa.
- **RF-006**: El backend DEBE registrar validators de FluentValidation mediante assembly scanning, sin registrar validators de forma individual.
- **RF-007**: La fábrica de filtros de validación DEBE detectar `IValidator<T>` por convención y ejecutar la validación cuando exista.
- **RF-008**: La ausencia de un validator para un tipo DEBE permitir el pass-through sin producir un error de configuración.
- **RF-009**: Los errores de validación DEBEN responder como `ValidationProblemDetails` con HTTP 400 y conservar los errores producidos por el validator.
- **RF-010**: El backend DEBE proporcionar una representación `Result` para errores esperados y una conversión centralizada a `ProblemDetails` que conserve status code y payload.
- **RF-011**: Las capacidades transversales DEBEN disponer de logging estructurado mediante `ILogger`.
- **RF-012**: El endpoint `/health` DEBE implementarse como un `ISlice` y servir como única sonda de infraestructura de extremo a extremo.
- **RF-013**: `Program.cs` DEBE limitarse a configurar servicios, middleware, registrar la infraestructura transversal y mapear endpoints mediante el mecanismo centralizado.
- **RF-014**: Las clases `ISlice`, validators y handlers de prueba DEBEN vivir exclusivamente en `app/backend/tests/NetRentManagerApiTests` y nunca en el proyecto de producción.
- **RF-015**: La iniciativa NO DEBE crear entidades de dominio, `AppDbContext`, configuraciones EF Core, migraciones, seeders, conexión a base de datos, endpoints de negocio, mappings de negocio ni features de producto.
- **RF-016**: La iniciativa NO DEBE usar controllers.
- **RF-017**: La versión de plataforma DEBE derivarse de `global.json` existente y la iniciativa NO DEBE modificar ese archivo.

### Entidades clave

Esta iniciativa no introduce entidades de dominio ni datos persistidos. Sus contratos técnicos son:

- **ISlice**: Contrato que representa un endpoint Minimal API descubrible y mapeable automáticamente.
- **IHandler**: Contrato marcador para el auto-registro de handlers de casos de uso futuros.
- **ValidationFilterFactory**: Capacidad transversal que localiza validators compatibles y aplica sus resultados a las respuestas HTTP.
- **Result y ProblemDetails**: Contratos para expresar errores esperados y transformarlos de manera uniforme.
- **Sonda de salud**: Endpoint de infraestructura `/health` usado para comprobar el descubrimiento y mapeo de slices.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **CE-001**: El 100% de las pruebas requeridas de descubrimiento, validación, errores y salud pasan en una ejecución completa del proyecto de pruebas backend.
- **CE-002**: Una implementación de `ISlice` definida en el proyecto de pruebas se descubre y registra una sola vez, sin modificar manualmente `Program.cs`.
- **CE-003**: El endpoint `/health` responde correctamente en el 100% de las verificaciones de arranque realizadas mediante el mecanismo centralizado.
- **CE-004**: Un request con validator produce HTTP 400 y `ValidationProblemDetails` cuando la validación falla, mientras que un request sin validator continúa sin error de configuración.
- **CE-005**: El mapeo de un `Result` de error conserva el 100% del status code y de los datos de error definidos por el caso de prueba.
- **CE-006**: La inspección del backend confirma cero entidades de dominio, cero conexiones a base de datos, cero controllers y cero features de producto introducidas por esta iniciativa.
- **CE-007**: La solución backend compila sin errores después de registrar la infraestructura transversal.

## Suposiciones

- La solución `app/NetRentManager.sln`, el proyecto `app/backend/src/NetRentManagerApi` y el proyecto de pruebas backend creados por la spec 001 ya existen y son la base de esta iniciativa.
- `global.json` es la fuente de verdad para la versión de plataforma y contiene la versión requerida por el repositorio.
- El assembly scanning se limita a los assemblies definidos por el plan y no requiere configuración individual de cada tipo.
- La sonda `/health` no consulta base de datos ni representa una feature de producto.
- Las pruebas de infraestructura pueden declarar tipos de prueba en el proyecto de tests sin incorporarlos al proyecto de producción.
- El detalle exacto de namespaces, nombres de archivos y APIs auxiliares se definirá en `plan.md`, respetando este alcance y las restricciones de la spec.
- La evidencia de validación final se registrará en `quickstart.md` antes de permitir la transición del estado `En implementación` a `Implementada`.

## Fuera de alcance

- Entidades de dominio y casos de uso de producto.
- `AppDbContext`, configuraciones EF Core, migraciones, seeders y conexiones a PostgreSQL.
- Endpoints, handlers, validators o mappings de negocio.
- Autenticación, autorización, usuarios, rentas, propiedades o cualquier feature funcional.
- Controllers y cualquier arquitectura alternativa a Minimal APIs.
- Modificación de `global.json`, recreación de la solución o recreación de los proyectos existentes.
