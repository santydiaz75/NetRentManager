# Tareas: Interactividad de Servidor en Blazor

**Entrada**: Documentos de diseño de `specs/015-signalr-interactive-server/`

**Prerequisitos**: `spec.md`, `plan.md`, `research.md`, `data-model.md` y `quickstart.md`

**Pruebas**: Se incluyen tareas de inspección automatizada, build y validación manual porque la spec exige verificar tanto la configuración granular como el comportamiento real del circuito de servidor.

**Organización**: Las tareas están agrupadas por historia de usuario y mantienen el alcance frontend-only de la iniciativa.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo con otras tareas sin modificar el mismo archivo ni depender de una tarea incompleta.
- **[Story]**: Historia de usuario asociada (`US1`, `US2`, `US3`, `US4`).
- Cada tarea incluye la ruta exacta del archivo o comando afectado.

---

## Phase 1: Setup y verificación de prerrequisitos

**Propósito**: Confirmar que la infraestructura de Interactive Server ya existe y que el alcance se limita a las páginas host previstas.

- [X] T001 [P] Verificar en `app/frontend/src/NetRentManagerWeb/Program.cs` que `AddInteractiveServerComponents()` y `AddInteractiveServerRenderMode()` ya están registrados, sin modificar el archivo.
- [X] T002 [P] Verificar en `app/frontend/src/NetRentManagerWeb/Components/App.razor` y `app/frontend/src/NetRentManagerWeb/Components/Routes.razor` que no existe una habilitación global de `InteractiveServer`.
- [X] T003 [P] Verificar en `app/frontend/src/NetRentManagerWeb/Components/Pages/Error.razor` y `app/frontend/src/NetRentManagerWeb/Components/Pages/NotFound.razor` que no declaran un modo de render interactivo.

**Checkpoint**: La infraestructura global está disponible y las superficies estáticas permanecen fuera del circuito.

---

## Phase 2: User Story 1 - Paginar el listado sin recargar el navegador (Priority: P1) 🎯 MVP

**Objetivo de la historia**: Activar el circuito de servidor únicamente en el host del listado para que los controles existentes de paginación puedan ejecutar sus eventos y conservar sus estados límite.

**Prueba independiente**: Abrir `/`, comprobar que el host declara `InteractiveServer`, pulsar `Siguiente` y `Anterior` cuando están habilitados y confirmar que el listado cambia sin recarga completa.

### Pruebas para User Story 1

- [X] T004 [P] [US1] Crear `app/frontend/test/NetRentManagerWeb/InteractiveServerRenderModeTests.cs` con una prueba que confirme que `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor` contiene exactamente la declaración `@rendermode InteractiveServer`.
- [X] T005 [P] [US1] Añadir en `app/frontend/test/NetRentManagerWeb/InteractiveServerBehaviorTests.cs` una prueba de inspección que confirme que `Home.razor` conserva los componentes de paginación y no añade una segunda lógica de carga ni acceso directo a `HttpClient`.

### Implementación para User Story 1

- [X] T006 [US1] Añadir `@rendermode InteractiveServer` a `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor`, sin modificar `Home.razor.cs`, `PaginationControls.razor`, los contratos Refit ni la lógica de carga.
- [X] T007 [US1] Confirmar en `app/frontend/src/NetRentManagerWeb/Features/Properties/List/PaginationControls.razor` que los estados deshabilitados de `Anterior` y `Siguiente` continúan dependiendo de `HasPrevious` y `HasNext`, sin alterar su comportamiento.

**Checkpoint**: El listado opta por Interactive Server de forma granular y sus controles conservan la semántica existente.

---

## Phase 3: User Story 2 - Recuperar el listado desde el detalle (Priority: P1)

**Objetivo de la historia**: Activar el circuito de servidor en el host del detalle para que el retorno al listado funcione en contenido cargado, no encontrado y error.

**Prueba independiente**: Abrir una propiedad válida, una ruta inexistente y un estado de error; pulsar `Volver al listado` y confirmar la navegación a `/` sin recarga completa.

### Pruebas para User Story 2

- [X] T008 [P] [US2] Añadir en `app/frontend/test/NetRentManagerWeb/InteractiveServerRenderModeTests.cs` una prueba que confirme que `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor` contiene exactamente la declaración `@rendermode InteractiveServer`.
- [X] T009 [P] [US2] Añadir en `app/frontend/test/NetRentManagerWeb/InteractiveServerBehaviorTests.cs` una prueba que confirme que la página de detalle mantiene el enlace `Volver al listado` con destino `/` en sus estados de renderizado.

### Implementación para User Story 2

- [X] T010 [US2] Añadir `@rendermode InteractiveServer` a `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor`, sin modificar `PropertyDetailPage.razor.cs`, el cliente Refit ni la lógica de carga.
- [X] T011 [US2] Confirmar en `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor` que el enlace de retorno permanece disponible en el contenido cargado y en los estados de propiedad no encontrada y error.

**Checkpoint**: El detalle usa Interactive Server solo en su host y conserva el retorno a `/` en todos los estados requeridos.

---

## Phase 4: User Story 3 - Reintentar una carga fallida (Priority: P1)

**Objetivo de la historia**: Verificar que el circuito habilitado por las páginas host permite ejecutar los botones `Reintentar` existentes sin cambiar la lógica de carga.

**Prueba independiente**: Provocar un error de listado y de detalle, restaurar el servicio y comprobar que `Reintentar` inicia nuevamente la carga en cada página.

- [ ] T012 [US3] Añadir en `app/frontend/test/NetRentManagerWeb/InteractiveServerBehaviorTests.cs` una prueba que compruebe que el host del listado conserva el estado de error y el punto de interacción de `Reintentar` definido por la implementación vigente.
- [ ] T013 [US3] Añadir en `app/frontend/test/NetRentManagerWeb/InteractiveServerBehaviorTests.cs` una prueba que compruebe que el host del detalle conserva el estado de error y el punto de interacción de `Reintentar` definido por la implementación vigente.
- [ ] T014 [US3] Validar en `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor` y `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor` que la habilitación del render mode no modifica ni duplica los handlers de carga existentes.

**Checkpoint**: Los reintentos siguen delegando en la lógica existente y quedan cubiertos por el circuito de servidor, sin refactor funcional.

---

## Phase 5: User Story 4 - Mantener estáticas las páginas sin interacción (Priority: P2)

**Objetivo de la historia**: Garantizar que solo las dos páginas host previstas crean circuitos interactivos.

**Prueba independiente**: Revisar los hosts globales y las páginas de error y confirmar que no contienen declaraciones de `InteractiveServer`.

- [X] T015 [P] [US4] Completar en `app/frontend/test/NetRentManagerWeb/InteractiveServerRenderModeTests.cs` la comprobación de que `App.razor`, `Routes.razor`, `Error.razor` y `NotFound.razor` no contienen `@rendermode` ni `InteractiveServer`.
- [X] T016 [US4] Confirmar que el conjunto de declaraciones `InteractiveServer` del frontend queda limitado a `Home.razor` y `PropertyDetailPage.razor`, sin cambios en `app/frontend/src/NetRentManagerWeb/Components/App.razor` ni `app/frontend/src/NetRentManagerWeb/Components/Routes.razor`.

**Checkpoint**: La interactividad permanece granular y las páginas estáticas no reciben circuitos por configuración global.

---

## Phase 6: Regresión y cierre

**Propósito**: Ejecutar la validación completa, documentar evidencia y cerrar la iniciativa solo cuando todas las tareas estén verificadas.

- [X] T017 Ejecutar `dotnet build app/frontend/src/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore` y corregir únicamente errores de archivos trazados por esta iniciativa.
- [X] T018 Ejecutar `dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore` y confirmar que pasan las pruebas existentes y las nuevas de interactividad de servidor.
- [ ] T019 Ejecutar los pasos de `specs/015-signalr-interactive-server/quickstart.md` con backend y frontend locales, comprobando circuito, paginación, retorno, reintento y páginas estáticas.
- [ ] T020 Registrar en `specs/015-signalr-interactive-server/quickstart.md` la evidencia de build, pruebas, inspección de declaraciones y validación manual sin recarga completa.
- [ ] T021 Confirmar que no existen modificaciones en backend, contratos Refit, `Program.cs`, code-behind, `App.razor` ni `Routes.razor` fuera de las verificaciones trazadas y marcar todas las tareas anteriores como `[X]` únicamente después de completar las validaciones.

---

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Setup (Phase 1)**: sin dependencias; verifica que la infraestructura requerida ya existe.
- **US1 (Phase 2)**: depende de Setup y es el MVP de la iniciativa.
- **US2 (Phase 3)**: depende de Setup; puede ejecutarse en paralelo con US1 porque modifica otro host.
- **US3 (Phase 4)**: depende de US1 y US2 para validar los estados de error de ambos hosts.
- **US4 (Phase 5)**: depende de las declaraciones de US1 y US2 para comprobar que no se habilitó interactividad fuera de alcance.
- **Regresión (Phase 6)**: depende de todas las historias anteriores.

### Dependencias dentro de cada historia

- US1: T004 y T005 pueden prepararse en paralelo; T006 es la implementación y T007 verifica que no se altera la paginación.
- US2: T008 y T009 pueden prepararse en paralelo; T010 es la implementación y T011 valida los estados del enlace de retorno.
- US3: T012 y T013 son pruebas independientes en el mismo archivo y deben coordinarse para evitar ediciones simultáneas; T014 depende de ambas comprobaciones.
- US4: T015 y T016 dependen de que T006 y T010 estén completadas.

### Oportunidades de paralelización

- T001, T002 y T003 pueden ejecutarse en paralelo porque son verificaciones de archivos distintos.
- T004/T005 y T008/T009 pueden prepararse en paralelo antes de aplicar las declaraciones de render mode.
- T017 y la revisión de documentación de T020 no deben ejecutarse en paralelo con cambios pendientes en código.

---

## Estrategia de implementación

### MVP primero

1. Completar Phase 1.
2. Ejecutar US1: declarar `InteractiveServer` en `Home.razor` y verificar paginación.
3. Ejecutar build y pruebas frontend.
4. Validar manualmente `/` antes de continuar con el detalle.

### Entrega incremental

1. Activar interactividad granular en el listado.
2. Activar interactividad granular en el detalle.
3. Verificar reintentos y estados de error existentes.
4. Confirmar que los hosts globales y páginas de error permanecen estáticos.
5. Ejecutar regresión, quickstart y cierre de la spec.

### Alcance explícito

No se agregan endpoints, DTOs, contratos Refit, migraciones, estilos, componentes nuevos de negocio ni refactorizaciones de code-behind. El cambio funcional aprobado se limita a las declaraciones de render mode y a las pruebas/documentación que demuestran su efecto.
