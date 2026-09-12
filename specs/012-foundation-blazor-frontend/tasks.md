---

description: "Task list for feature implementation"
---

# Tasks: Fundación del Frontend Blazor

**Input**: Documentos de diseño de `specs/012-foundation-blazor-frontend/`

**Prerrequisitos**: plan.md (requerido), spec.md (requerido para historias de usuario), research.md, data-model.md, quickstart.md

**Pruebas**: Se incluyen tareas de prueba porque `plan.md` (sección "Diseño técnico > Validación") las exige explícitamente para esta iniciativa.

**Organización**: Las tareas están agrupadas por historia de usuario para permitir implementación y prueba independiente de cada una.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece (US1-US5)
- Cada tarea incluye la ruta exacta de archivo

## Phase 1: Setup (Infraestructura compartida)

**Propósito**: Agregar las dependencias NuGet que necesitan las historias de comunicación e íconos.

- [X] T001 Agregar las referencias NuGet `Refit`, `Refit.HttpClientFactory` y `BlazorBlueprint.Icons.Lucide` en `app/frontend/src/NetRentManagerWeb/NetRentManagerWeb.csproj`.

**Checkpoint**: El proyecto restaura correctamente con las nuevas dependencias antes de continuar.

---

## Phase 2: Foundational (Base visual compartida)

**Propósito**: Dejar en `wwwroot/app.css` los tokens y componentes visuales genéricos que consumen varias historias, antes de construir el layout, los íconos o los estados de pantalla.

- [X] T002 Agregar tokens en `:root` (colores, espaciado, bordes, sombras, anchos de sidebar) a `app/frontend/src/NetRentManagerWeb/wwwroot/app.css`.
- [X] T003 Agregar clases reutilizables de botones, cards, formularios y badges a `app/frontend/src/NetRentManagerWeb/wwwroot/app.css`, usando únicamente los tokens de T002.

**Checkpoint**: Los tokens y componentes visuales base existen antes de construir layout, íconos o estados de pantalla.

---

## Phase 3: User Story 1 - Arrancar la aplicación sin dependencias visuales externas (Priority: P1) 🎯 MVP

**Objetivo de la historia**: Confirmar y preservar que `NetRentManagerWeb` no carga ningún framework CSS ni librería de íconos de terceros.

**Prueba independiente**: Compilar y ejecutar el proyecto, inspeccionar el HTML servido y confirmar ausencia de referencias a Bootstrap, Bootstrap Icons, Tailwind, Font Awesome o Fluent Icons.

- [X] T004 [US1] Auditar `app/frontend/src/NetRentManagerWeb/Components/App.razor` y `app/frontend/src/NetRentManagerWeb/wwwroot/` y confirmar que no existen enlaces, scripts ni carpetas de frameworks CSS o de íconos de terceros.
- [X] T005 [US1] Añadir en `app/frontend/test/NetRentManagerWeb/ThirdPartyDependencyTests.cs` una prueba que lea `Components/App.razor` y confirme ausencia de referencias a `bootstrap`, `font-awesome`, `fluent-icons` u otras librerías de terceros, y que `wwwroot/lib/` no exista o esté vacío de dichas librerías.

**Checkpoint**: La ausencia de dependencias visuales externas queda verificada automáticamente (SC-001).

---

## Phase 4: User Story 2 - Ver el layout base con sidebar fija y contenido principal (Priority: P1)

**Objetivo de la historia**: Layout desktop-first con sidebar fija a la izquierda, navegación representativa y contenido principal amplio, responsive en tablet y móvil.

**Prueba independiente**: Ejecutar la aplicación en un viewport de escritorio y confirmar mediante inspección del DOM que existen `.app-shell`, `.app-sidebar`, `.app-main` y `.app-content`, y que la sidebar se adapta en tablet/móvil.

- [X] T006 [US2] Agregar a `app/frontend/src/NetRentManagerWeb/wwwroot/app.css` las clases de layout (`.app-shell`, `.app-sidebar`, `.sidebar-brand`, `.sidebar-nav`, `.sidebar-link`, `.sidebar-link-active`, `.app-main`, `.app-content`, `.page-container`, `.page-header`, `.page-title`, `.page-subtitle`, `.page-actions`) junto con los puntos de quiebre responsive de tablet y móvil definidos en `research.md`.
- [X] T007 [US2] Crear `app/frontend/src/NetRentManagerWeb/Components/Layout/NavMenu.razor` con enlaces de navegación representativos ("Inicio" apuntando a `/`, "Propiedades" apuntando a una ruta vacía o `/`), usando las clases `.sidebar-nav`/`.sidebar-link`/`.sidebar-link-active`.
- [X] T008 [US2] Reescribir `app/frontend/src/NetRentManagerWeb/Components/Layout/MainLayout.razor` para renderizar `.app-shell` con `.app-sidebar` (incluyendo `NavMenu`) y `.app-main`/`.app-content` alrededor de `@Body`, y vaciar o eliminar `MainLayout.razor.css` migrando cualquier estilo necesario a `app.css`.
- [X] T009 [US2] Adaptar `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor` para usar `.page-container`/`.page-header` con un mensaje placeholder genérico, sin lógica de negocio.
- [X] T010 [US2] Adaptar `app/frontend/src/NetRentManagerWeb/Components/Pages/Error.razor` y `app/frontend/src/NetRentManagerWeb/Components/Pages/NotFound.razor` para usar `.page-container`/`.page-header` y las clases del sistema visual, sin clases de Bootstrap.
- [X] T011 [US2] Añadir en `app/frontend/test/NetRentManagerWeb/MainLayoutCompositionTests.cs` una prueba que confirme que `MainLayout.razor` referencia `app-shell`, `app-sidebar`, `app-main` y `app-content`, y que `NavMenu.razor` define al menos los enlaces "Inicio" y "Propiedades".

**Checkpoint**: El layout base y la navegación representativa quedan disponibles y verificados (SC-002).

---

## Phase 5: User Story 3 - Usar íconos Lucide consistentes (Priority: P2)

**Objetivo de la historia**: Los íconos de la sidebar se renderizan con Lucide Icons y clases CSS reutilizables, sin ninguna otra librería de íconos.

**Prueba independiente**: Inspeccionar el HTML renderizado de la sidebar y confirmar SVGs con clases `.icon`/`.icon-sm` y ausencia de otras librerías de íconos.

- [X] T012 [US3] Agregar a `app/frontend/src/NetRentManagerWeb/wwwroot/app.css` las clases `.icon`, `.icon-sm`, `.icon-md`, `.icon-lg` y las variantes de color (`.icon-muted`, `.icon-primary`, `.icon-success`, `.icon-warning`, `.icon-danger`) según `frontend.instructions.md`.
- [X] T013 [US3] Añadir íconos de `BlazorBlueprint.Icons.Lucide` a los enlaces de `app/frontend/src/NetRentManagerWeb/Components/Layout/NavMenu.razor` (creado en T007), usando las clases `.icon`/`.icon-sm`.
- [X] T014 [US3] Añadir en `app/frontend/test/NetRentManagerWeb/IconSystemTests.cs` una prueba que confirme que `NavMenu.razor` referencia componentes de `BlazorBlueprint.Icons.Lucide` y que el proyecto no referencia Bootstrap Icons, Font Awesome ni Fluent Icons.

**Checkpoint**: El sistema de íconos Lucide queda integrado y verificado (SC-003).

---

## Phase 6: User Story 4 - Tener el andamiaje base de comunicación tipada con el backend (Priority: P2)

**Objetivo de la historia**: Un `HttpClient` tipado hacia el backend, registrado con `IHttpClientFactory` y listo para que features futuras agreguen interfaces Refit, sin endpoints de negocio.

**Prueba independiente**: Inspeccionar el registro de servicios y confirmar el `HttpClient` nombrado con la URL base configurada, y que ningún componente usa `HttpClient` directamente.

- [X] T015 [US4] Agregar la clave `ApiSettings:BaseUrl` con la URL del backend a `app/frontend/src/NetRentManagerWeb/appsettings.json` y `app/frontend/src/NetRentManagerWeb/appsettings.Development.json`.
- [X] T016 [US4] Registrar en `app/frontend/src/NetRentManagerWeb/Program.cs` un `HttpClient` nombrado hacia el backend mediante `IHttpClientFactory`, leyendo `ApiSettings:BaseUrl` vía `IConfiguration` y lanzando un error claro si falta, dejando el registro listo para `AddRefitClient<TInterface>()` de features futuras.
- [X] T017 [US4] Añadir en `app/frontend/test/NetRentManagerWeb/ApiClientRegistrationTests.cs` una prueba que confirme el registro del `HttpClient` nombrado con la base address esperada, y una revisión de que ningún `.razor`/`.razor.cs` referencia `HttpClient` directamente ni `RestService.For<T>()` fuera del registro central.

**Checkpoint**: El andamiaje de comunicación con el backend está disponible para features futuras sin endpoints de negocio (SC-004).

---

## Phase 7: User Story 5 - Reutilizar patrones base de estados de pantalla (Priority: P3)

**Objetivo de la historia**: Clases CSS reutilizables para los estados de cargando, vacío, error y éxito con datos.

**Prueba independiente**: Revisar `app.css` y confirmar que existen las cuatro clases de estado, sin que ninguna pantalla de negocio las consuma todavía.

- [X] T018 [US5] Agregar a `app/frontend/src/NetRentManagerWeb/wwwroot/app.css` las clases `.state-loading`, `.state-empty`, `.state-error` y `.state-success`, usando los tokens definidos en T002.
- [X] T019 [US5] Añadir en `app/frontend/test/NetRentManagerWeb/ScreenStateStylesTests.cs` una prueba que confirme que las cuatro clases de estado existen en `app.css`.

**Checkpoint**: Los patrones reutilizables de estados de pantalla quedan disponibles para features futuras (SC-005).

---

## Phase 8: Regresión y cierre

**Propósito**: Validar el conjunto completo y dejar evidencia de cierre.

- [X] T020 Ejecutar `dotnet build app/NetRentManager.sln --no-restore` y corregir únicamente errores en archivos trazados por esta iniciativa.
- [X] T021 Ejecutar `dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore` y confirmar que toda la suite (incluidas las pruebas nuevas de T005, T011, T014, T017, T019) pasa sin errores.
- [X] T022 Ejecutar manualmente los pasos de `specs/012-foundation-blazor-frontend/quickstart.md` (viewports de escritorio/tablet/móvil, íconos, `HttpClient`, clases de estado) y registrar la evidencia al final del archivo.
- [X] T023 Confirmar que todas las tareas de `specs/012-foundation-blazor-frontend/tasks.md` estén marcadas `[X]` solo después de build, tests y evidencia de `quickstart.md` completos.

---

## Dependencies & Execution Order

### Orden entre fases

- **Setup (Phase 1)**: Sin dependencias. Bloquea a US3 (paquete Lucide) y US4 (paquete Refit).
- **Foundational (Phase 2)**: Sin dependencias de Setup. Bloquea a US2 (usa los tokens de T002) y a US5 (usa los tokens de T002).
- **US1 (Phase 3)**: Sin dependencias de Foundational; puede ejecutarse en paralelo con Setup/Foundational.
- **US2 (Phase 4)**: Depende de Foundational (T002) para los tokens usados en el layout.
- **US3 (Phase 5)**: Depende de Setup (T001, paquete Lucide) y de US2 (T007, `NavMenu.razor` ya creado, para agregarle íconos en T013).
- **US4 (Phase 6)**: Depende de Setup (T001, paquete Refit).
- **US5 (Phase 7)**: Depende de Foundational (T002).
- **Regresión (Phase 8)**: Depende de todas las historias anteriores.

### Dependencias dentro de cada historia

- US1: T004 (auditoría) antes de T005 (prueba automatizada que formaliza la auditoría).
- US2: T006 (CSS de layout) antes de T007 (`NavMenu.razor` usa esas clases); T007 y T008 antes de T011 (prueba de composición); T009 y T010 son independientes entre sí y de T006-T008 salvo por depender del nuevo `MainLayout`.
- US3: T012 (clases de ícono) antes de T013 (`NavMenu.razor` usa esas clases); T013 antes de T014 (prueba).
- US4: T015 (configuración) antes de T016 (registro en `Program.cs`); T016 antes de T017 (prueba).
- US5: T018 (clases CSS) antes de T019 (prueba).

### Oportunidades de paralelización

- T002 y T003 pueden ejecutarse en paralelo si se coordinan cuidadosamente los tokens usados (ambos tocan `app.css`; en la práctica conviene secuenciarlos).
- US1 (Phase 3) puede ejecutarse en paralelo con Setup y Foundational, ya que no depende de ellos.
- T009 y T010 (páginas placeholder) pueden ejecutarse en paralelo entre sí una vez completado T008.

## Implementation Strategy

### Entrega de MVP (Historias 1 y 2)

1. Completar Phase 1 (Setup) y Phase 2 (Foundational).
2. Completar Phase 3 (US1: confirmar ausencia de dependencias externas).
3. Completar Phase 4 (US2: layout base con sidebar y contenido principal).
4. **PARAR y validar**: en este punto la aplicación tiene una base visual propia, sin dependencias externas — MVP entregable.

### Entrega incremental

1. Agregar Phase 5 (US3: íconos Lucide) sobre la sidebar ya creada.
2. Agregar Phase 6 (US4: andamiaje Refit/HttpClient) de forma independiente.
3. Agregar Phase 7 (US5: estados de pantalla reutilizables).
4. Cerrar con Phase 8 (build, tests y evidencia de `quickstart.md`).
