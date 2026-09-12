# Especificación de la Funcionalidad: Interactividad de Servidor en Blazor

**Rama de la funcionalidad**: `015-signalr-interactive-server`

**Creado**: 2026-09-12

**Estado**: En implementación

**Trazabilidad de estado**:

- `Borrador` -> `Aprobada` | Motivo: la especificación define un alcance frontend acotado, criterios de aceptación verificables y no requiere decisiones pendientes | Fecha: 2026-09-12
- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar prerrequisitos, tareas y checklist | Fecha: 2026-09-12

**Entrada**: Descripción de usuario: "Habilitar interactividad de servidor en las páginas Blazor del frontend NetRentManagerWeb para que los botones de navegación funcionen."

## Contexto

El frontend `NetRentManagerWeb` es una aplicación Blazor Web App. Sus páginas se renderizan actualmente mediante Static SSR, por lo que los manejadores interactivos como `@onclick` no se ejecutan porque no existe un circuito de servidor asociado a esas páginas.

La interactividad de servidor ya está registrada en la configuración de la aplicación. El problema está limitado a que las páginas host que contienen acciones interactivas no declaran el modo de render correspondiente.

La implementación debe ser granular: solo las páginas que necesitan interacción deben optar por interactividad de servidor. Las páginas de error y no encontrado, junto con los hosts globales, deben conservar Static SSR.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Paginar el listado sin recargar el navegador (Prioridad: P1)

Como usuario del listado de propiedades, quiero pulsar "Anterior" o "Siguiente" y ver la página correspondiente sin una recarga completa del navegador, para explorar los resultados de forma fluida.

**Por qué esta prioridad**: La paginación es el flujo interactivo principal del listado y actualmente sus eventos no llegan al circuito de Blazor.

**Prueba independiente**: Abrir `/`, pulsar los controles de paginación con resultados disponibles y comprobar que la página y la UI se actualizan dentro del circuito interactivo.

**Escenarios de aceptación**:

1. **Dado** un listado con una página siguiente disponible, **cuando** el usuario pulsa "Siguiente", **entonces** se carga la página siguiente y la UI se actualiza sin recargar el navegador.
2. **Dado** un listado con una página anterior disponible, **cuando** el usuario pulsa "Anterior", **entonces** se carga la página anterior y la UI se actualiza sin recargar el navegador.
3. **Dado** que no existe página anterior o siguiente, **cuando** el usuario observa los controles, **entonces** el control correspondiente permanece deshabilitado según `CanGoPrevious` o `CanGoNext`.

### Historia de Usuario 2 - Recuperar el listado desde el detalle (Prioridad: P1)

Como usuario de una propiedad, quiero volver al listado desde la página de detalle, incluso si la propiedad no existe o la carga falla, para continuar navegando por la aplicación.

**Por qué esta prioridad**: El retorno es una acción esencial en los estados de contenido, no encontrado y error del detalle.

**Prueba independiente**: Abrir una propiedad válida, una propiedad inexistente y una ruta que produzca error; pulsar "Volver al listado" en cada estado y comprobar que la navegación llega a `/`.

**Escenarios de aceptación**:

1. **Dado** el detalle cargado de una propiedad, **cuando** el usuario pulsa "Volver al listado", **entonces** navega a `/` sin recarga completa del navegador.
2. **Dado** el estado de propiedad no encontrada o de error, **cuando** el usuario pulsa "Volver al listado", **entonces** navega a `/` mediante el circuito interactivo.

### Historia de Usuario 3 - Reintentar una carga fallida (Prioridad: P1)

Como usuario que encuentra un error de carga, quiero pulsar "Reintentar" para volver a solicitar los datos sin abandonar la página.

**Por qué esta prioridad**: Permite recuperarse de errores transitorios de red o backend sin reiniciar manualmente el flujo.

**Prueba independiente**: Provocar un error de carga en el listado y en el detalle, restaurar el servicio y pulsar "Reintentar"; comprobar que se ejecuta nuevamente la carga y se muestra el resultado correspondiente.

**Escenarios de aceptación**:

1. **Dado** el estado de error del listado, **cuando** el usuario pulsa "Reintentar", **entonces** se vuelve a ejecutar la carga de propiedades y la UI refleja el resultado.
2. **Dado** el estado de error del detalle, **cuando** el usuario pulsa "Reintentar", **entonces** se vuelve a ejecutar la carga de la propiedad y la UI refleja el resultado.

### Historia de Usuario 4 - Mantener estáticas las páginas sin interacción (Prioridad: P2)

Como responsable de la aplicación, quiero que la interactividad se habilite solo en las páginas que contienen acciones interactivas, para mantener el alcance del circuito de servidor controlado.

**Por qué esta prioridad**: La interactividad granular evita cambios de comportamiento y coste innecesarios en páginas que solo muestran contenido estático.

**Prueba independiente**: Inspeccionar los hosts globales, `Error` y `NotFound`, y comprobar que no reciben un modo de render interactivo.

**Escenarios de aceptación**:

1. **Dado** `App.razor` y `Routes.razor`, **cuando** se revisa la configuración de renderizado, **entonces** no contienen una habilitación global de interactividad.
2. **Dado** las páginas `Error` y `NotFound`, **cuando** se renderizan, **entonces** permanecen en Static SSR.

## Casos límite

- Si el usuario está en la primera página, "Anterior" debe permanecer deshabilitado y no debe iniciar una carga.
- Si el usuario está en la última página, "Siguiente" debe permanecer deshabilitado y no debe iniciar una carga.
- Si el reintento vuelve a fallar, la página debe conservar el estado de error y no mostrar datos parciales.
- Si el detalle devuelve `404`, el botón de retorno debe seguir disponible y funcionar.
- Si el detalle devuelve un error distinto de `404`, el botón de retorno y el botón "Reintentar" deben seguir disponibles y funcionar.
- Si la conexión del circuito de servidor se interrumpe, la aplicación debe conservar el comportamiento estándar de reconexión de Blazor sin modificar el backend.
- Las páginas sin interacción no deben iniciar circuitos de servidor por una habilitación global accidental.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: La página de listado `Home.razor`, ruta `/`, DEBE declarar el modo de render interactivo de servidor de forma granular.
- **RF-002**: La página de detalle `PropertyDetailPage.razor`, ruta `/properties/{id}`, DEBE declarar el modo de render interactivo de servidor de forma granular.
- **RF-003**: La interactividad DEBE permitir que los controles "Anterior" y "Siguiente" del listado ejecuten sus manejadores existentes sin recargar completamente el navegador.
- **RF-004**: Los controles de paginación DEBEN conservar su estado deshabilitado según `CanGoPrevious` y `CanGoNext`.
- **RF-005**: El botón "Volver al listado" del detalle DEBE navegar a `/` desde el estado cargado, el estado de propiedad no encontrada y el estado de error.
- **RF-006**: Los estados de error del listado y del detalle DEBEN permitir ejecutar nuevamente la carga mediante el botón "Reintentar".
- **RF-007**: La aplicación NO DEBE habilitar interactividad global en `App.razor` ni en `Routes.razor` como parte de esta iniciativa.
- **RF-008**: Las páginas `Error` y `NotFound` DEBEN conservar Static SSR y no deben recibir un modo de render interactivo por esta iniciativa.
- **RF-009**: La implementación DEBE reutilizar los manejadores, code-behind, contratos Refit y lógica de carga existentes; no debe introducir una refactorización funcional de esos componentes.
- **RF-010**: La implementación DEBE quedar limitada al frontend y NO DEBE modificar backend, endpoints, contratos Refit, persistencia ni configuración de API.
- **RF-011**: La habilitación DEBE usar el modo de render de servidor ya registrado por la aplicación, sin migrar a Interactive WebAssembly ni a Auto.
- **RF-012**: Todo cambio de código, prueba o documentación DEBE quedar trazado a una tarea específica de `tasks.md` antes de considerarse completado.

### Entidades clave

- **Página de listado**: host de la ruta `/` que presenta propiedades y controles de paginación.
- **Página de detalle**: host de la ruta `/properties/{id}` que presenta una propiedad y acciones de retorno o reintento.
- **Circuito interactivo de servidor**: sesión de interacción que permite ejecutar eventos de los componentes sin recarga completa del documento.
- **Controles interactivos**: botones de paginación, retorno y reintento que ya existen en la experiencia frontend.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de los clics sobre "Siguiente" con `CanGoNext` verdadero actualiza el listado a la página siguiente sin recarga completa del navegador.
- **SC-002**: El 100% de los clics sobre "Anterior" con `CanGoPrevious` verdadero actualiza el listado a la página anterior sin recarga completa del navegador.
- **SC-003**: El 100% de los controles de paginación conserva correctamente su estado deshabilitado en los límites del listado.
- **SC-004**: El 100% de los estados de error del listado y del detalle permite un reintento que ejecuta nuevamente la carga.
- **SC-005**: El 100% de los estados del detalle, incluidos contenido, no encontrado y error, permite volver a `/` mediante el botón correspondiente.
- **SC-006**: `App.razor`, `Routes.razor`, `Error` y `NotFound` permanecen sin habilitación interactiva global o explícita no requerida.
- **SC-007**: Las pruebas automatizadas cubren la declaración granular del modo de render y los flujos de paginación, retorno y reintento sin cambios en backend o contratos Refit.

## Suposiciones

- `AddInteractiveServerComponents()` y `AddInteractiveServerRenderMode()` ya están registrados y funcionan en la aplicación actual.
- Los handlers y métodos de carga requeridos por los botones ya existen; esta iniciativa solo habilita el circuito que permite invocarlos.
- La ubicación actual de la página de detalle es `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor`; cualquier referencia conceptual a una página host de detalle se interpreta sobre ese archivo real.
- La paginación y la navegación pueden actualizar el estado mediante el patrón de componentes existente sin modificar los contratos HTTP.
- La validación se realizará con pruebas unitarias y una comprobación manual del flujo en el frontend local.

## Compatibilidad y gobernanza

- Esta iniciativa depende de `012-foundation-blazor-frontend`, `013-blazor-list-properties` y `014-blazor-property-detail`.
- Debe respetar .NET 10, Blazor Web App, Razor Components y el modo Interactive Server ya registrado.
- No requiere cambios en backend, Refit, OpenAPI, base de datos ni estilos visuales.
- Debe respetar el flujo Spec-Driven: esta spec precede a `plan.md`, `tasks.md` e implementación.
- El estado inicial queda aprobado para continuar con `speckit.plan`; las transiciones posteriores deben ejecutarse por el flujo Speckit con trazabilidad de estado.

## Fuera de alcance

- Cambios en backend, endpoints, persistencia o contratos Refit.
- Migración a Interactive WebAssembly o Auto.
- Habilitación de interactividad global en `App.razor` o `Routes.razor`.
- Cambios en las páginas `Error` y `NotFound`, salvo pruebas que confirmen que permanecen estáticas.
- Refactorización de `PropertyList`, `PropertyDetail`, code-behind o lógica de carga existente.
- Rediseño visual, nuevos estilos, nuevos iconos o nuevas funcionalidades de negocio.
