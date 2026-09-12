---

description: "Task list for feature implementation"
---

# Tasks: Listado de Propiedades en Página Principal

**Input**: Documentos de diseño de `specs/013-blazor-list-properties/`

**Prerrequisitos**: plan.md (requerido), spec.md (requerido para historias de usuario), research.md, data-model.md, quickstart.md

**Pruebas**: Se incluyen tareas de prueba porque `plan.md` (sección "Diseño técnico > Validación") las exige explícitamente para esta iniciativa.

**Organización**: Las tareas están agrupadas por historia de usuario para permitir implementación y prueba independiente de cada una.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece (US1-US5)
- Cada tarea incluye la ruta exacta de archivo

## Phase 1: Foundational (Cliente Refit y grilla compartidos)

**Propósito**: Dejar disponible el cliente Refit tipado y la grilla responsive antes de construir cualquier historia de usuario, ya que todas la consumen.

- [X] T001 [P] Crear `app/frontend/src/NetRentManagerWeb/Services/Api/Properties/PagedPropertiesResponse.cs` y `app/frontend/src/NetRentManagerWeb/Services/Api/Properties/PropertyListItem.cs` como DTO de presentación en camelCase, según `data-model.md`.
- [X] T002 [US1] Crear `app/frontend/src/NetRentManagerWeb/Services/Api/Properties/IPropertiesApi.cs` con `GetPropertiesAsync(int page, int pageSize, CancellationToken)` retornando `Task<ApiResponse<PagedPropertiesResponse>>` mapeado a `GET /api/properties`.
- [X] T003 Crear `app/frontend/src/NetRentManagerWeb/Services/Api/Properties/PropertiesApiClientRegistration.cs` con `AddPropertiesApiClient(this IServiceCollection)`, registrando `AddRefitClient<IPropertiesApi>` con `httpClientName: NetRentManagerApiClientRegistration.HttpClientName` y `RefitSettings` usando `SystemTextJsonContentSerializer`/`JsonSerializerDefaults.Web`.
- [X] T004 Registrar `AddPropertiesApiClient()` en `app/frontend/src/NetRentManagerWeb/Program.cs`, después de `AddNetRentManagerApiClient`.
- [X] T005 [P] Agregar la clase `.properties-grid` (3 columnas por defecto, 2 bajo `max-width: 1024px`, 1 bajo `max-width: 640px`) a `app/frontend/src/NetRentManagerWeb/wwwroot/app.css`, reutilizando los puntos de quiebre de `012-foundation-blazor-frontend`.

**Checkpoint**: El cliente Refit y la grilla están listos antes de construir la página.

---

## Phase 2: User Story 1 - Ver la primera página de propiedades al entrar (Priority: P1) 🎯 MVP

**Objetivo de la historia**: Al abrir `/`, se consulta el backend con página/tamaño por defecto y se muestran las propiedades junto con sus metadatos.

**Prueba independiente**: Abrir `/` con propiedades sembradas y confirmar que se renderiza la grilla con los datos de cada propiedad y los metadatos de paginación.

- [X] T006 [P] [US1] Crear `app/frontend/src/NetRentManagerWeb/Features/Properties/List/PropertyCard.razor` que renderiza título, precio, estado e imagen (o un marcador visual si `ImageUrl` es nulo), usando clases de `app.css` (`.card`, `.badge-*`).
- [X] T007 [US1] Reescribir `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor` y crear `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor.cs`: leer `page`/`pageSize` con `[SupplyParameterFromQuery]` (por defecto `1`/`6`), invocar `IPropertiesApi.GetPropertiesAsync` en `OnInitializedAsync`, y renderizar `.properties-grid` con `PropertyCard` y los metadatos (página actual, total de páginas, total de elementos) cuando la respuesta es exitosa y contiene elementos.
- [X] T008 [P] [US1] Añadir en `app/frontend/test/NetRentManagerWeb/PropertiesListPageTests.cs` pruebas que confirmen que `Home.razor.cs` usa `page=1`/`pageSize=6` por defecto cuando la query string no los provee, usando un `IPropertiesApi` de prueba.
- [X] T009 [P] [US1] Añadir en `app/frontend/test/NetRentManagerWeb/PropertiesGridStylesTests.cs` una prueba que confirme que `.properties-grid` y sus reglas de 2/1 columnas existen en `app.css`.

**Checkpoint**: La carga inicial del listado con datos queda funcional y verificada (SC-001).

---

## Phase 3: User Story 2 - Navegar entre páginas de resultados (Priority: P1)

**Objetivo de la historia**: Controles "Anterior"/"Siguiente" que navegan entre páginas mediante query string, deshabilitados según `hasPrevious`/`hasNext`.

**Prueba independiente**: Con más propiedades que el tamaño de página, hacer clic en "Siguiente" y "Anterior" y confirmar que el listado se actualiza; confirmar que los controles se deshabilitan en los extremos.

- [X] T010 [US2] Crear `app/frontend/src/NetRentManagerWeb/Features/Properties/List/PaginationControls.razor`, que recibe la página actual, `pageSize`, `hasPrevious` y `hasNext` como parámetros, y renderiza enlaces `<a href="/?page={page±1}&pageSize={pageSize}">` para "Anterior"/"Siguiente"; cuando el control no aplica, renderiza un elemento sin `href` con apariencia deshabilitada.
- [X] T011 [US2] Integrar `PaginationControls` en `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor`, pasándole los metadatos de la respuesta (`Page`, `PageSize`, `HasPrevious`, `HasNext`).
- [X] T012 [US2] Añadir en `app/frontend/test/NetRentManagerWeb/PaginationControlsTests.cs` pruebas que confirmen los `href` generados para "Anterior"/"Siguiente" cuando están habilitados, y la ausencia de `href` cuando `HasPrevious`/`HasNext` son `false`.

**Checkpoint**: La navegación entre páginas queda funcional y verificada (SC-002).

---

## Phase 4: User Story 3 - Reconocer cuándo no hay resultados (Priority: P2)

**Objetivo de la historia**: Mostrar un estado vacío claro cuando la respuesta no contiene propiedades.

**Prueba independiente**: Consultar una página sin propiedades (por ejemplo, fuera de rango) y confirmar que se muestra el estado vacío en lugar de una grilla en blanco.

- [X] T013 [US3] En `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor`/`Home.razor.cs`, renderizar `.state-empty` (definido en `012-foundation-blazor-frontend`) cuando la respuesta exitosa tenga `Items` vacío, en lugar de la grilla o el estado de error.
- [X] T014 [US3] Añadir en `app/frontend/test/NetRentManagerWeb/PropertiesListPageTests.cs` una prueba que confirme que el estado vacío se activa cuando `IPropertiesApi` devuelve `Items` vacío.

**Checkpoint**: El estado vacío queda verificado (SC-003).

---

## Phase 5: User Story 4 - Reconocer errores de comunicación con el backend (Priority: P2)

**Objetivo de la historia**: Mostrar un estado de error claro ante fallos de validación, error interno o fallo de red, sin exponer detalles técnicos.

**Prueba independiente**: Simular una respuesta no exitosa y un fallo de red al consultar el backend y confirmar que ambos casos muestran el mismo estado de error genérico.

- [X] T015 [US4] En `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor.cs`, capturar `ApiResponse.IsSuccessful == false` y excepciones de comunicación (`ApiException`/`HttpRequestException`) y renderizar `.state-error` (definido en `012-foundation-blazor-frontend`) con un mensaje genérico, sin exponer el cuerpo técnico de la respuesta ni la excepción cruda.
- [X] T016 [US4] Añadir en `app/frontend/test/NetRentManagerWeb/PropertiesListPageTests.cs` pruebas que confirmen que el estado de error se activa tanto ante una respuesta no exitosa como ante una excepción de comunicación, y que el mensaje mostrado no contiene el detalle técnico crudo.

**Checkpoint**: El estado de error queda verificado (SC-004).

---

## Phase 6: User Story 5 - Ver un indicador de carga mientras se consulta el backend (Priority: P3)

**Objetivo de la historia**: Mostrar un estado de carga mientras la solicitud está en curso, antes de que se resuelvan los demás estados.

**Prueba independiente**: Verificar que, mientras la solicitud no ha finalizado, el componente expone un estado de carga distinto de los estados vacío, error o con datos.

- [X] T017 [US5] En `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor.cs`, exponer una bandera `IsLoading` (verdadera antes de completar `OnInitializedAsync`, falsa al finalizar) y renderizar `.state-loading` (definido en `012-foundation-blazor-frontend`) mientras sea verdadera.
- [X] T018 [US5] Añadir en `app/frontend/test/NetRentManagerWeb/PropertiesListPageTests.cs` una prueba que confirme que `IsLoading` es verdadero antes de completar la carga y falso después, independientemente del resultado (éxito, vacío o error).

**Checkpoint**: El estado de carga queda verificado (SC-004/SC-005 en conjunto con los demás estados).

---

## Phase 7: Regresión y cierre

**Propósito**: Validar el conjunto completo y dejar evidencia de cierre.

- [X] T019 Ejecutar `dotnet build app/NetRentManager.sln --no-restore` y corregir únicamente errores en archivos trazados por esta iniciativa.
- [X] T020 Ejecutar `dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore` y confirmar que toda la suite (incluidas las pruebas nuevas de T008, T009, T012, T014, T016, T018) pasa sin errores.
- [X] T021 Ejecutar `dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj --no-restore` y confirmar que no hay regresiones en el backend (esta spec no lo modifica).
- [X] T022 Ejecutar manualmente los pasos de `specs/013-blazor-list-properties/quickstart.md` (datos, paginación, vacío, error, grilla responsive) y registrar la evidencia al final del archivo.
- [X] T022a Corregir defectos reales detectados en T022: agregar la referencia NuGet `Refit.Reflection` a `NetRentManagerWeb.csproj` (Refit exigía el generador de solicitudes por reflexión en runtime) y crear `app/frontend/src/NetRentManagerWeb/Features/_Imports.razor` con `@using BlazorBlueprint.Icons.Lucide.Components` para que los componentes bajo `Features/` puedan renderizar íconos Lucide.
- [X] T023 Confirmar que todas las tareas de `specs/013-blazor-list-properties/tasks.md` estén marcadas `[X]` solo después de build, tests y evidencia de `quickstart.md` completos.

---

## Dependencies & Execution Order

### Orden entre fases

- **Foundational (Phase 1)**: Sin dependencias de historias de usuario. Bloquea a todas las historias (T002 es en sí parte de US1 por ser su contrato de datos, pero el resto de T001/T003-T005 son transversales).
- **US1 (Phase 2)**: Depende de Foundational (cliente Refit, DTOs, grilla). Es el MVP.
- **US2 (Phase 3)**: Depende de US1 (la página y sus metadatos ya deben existir para agregar controles de paginación).
- **US3 (Phase 4)**: Depende de US1 (mismo componente `Home.razor`/`Home.razor.cs`).
- **US4 (Phase 5)**: Depende de US1 (mismo componente).
- **US5 (Phase 6)**: Depende de US1 (mismo componente).
- **Regresión (Phase 7)**: Depende de todas las historias anteriores.

### Dependencias dentro de cada historia

- Foundational: T001 y T005 son independientes entre sí; T002 depende de T001 (usa los DTOs); T003 depende de T002; T004 depende de T003.
- US1: T006 es independiente de T007 (archivos distintos); T007 depende de T002/T003/T004 (Foundational); T008 y T009 dependen de T007/T005 respectivamente.
- US2: T010 es independiente de T007; T011 depende de T007 y T010; T012 depende de T010.
- US3/US4/US5: cada tarea de implementación (T013, T015, T017) modifica el mismo `Home.razor.cs` de US1, por lo que se ejecutan secuencialmente entre sí; sus pruebas (T014, T016, T018) dependen de su propia tarea de implementación.

### Oportunidades de paralelización

- T001 y T005 en paralelo (Foundational, archivos distintos).
- T006, T008 y T009 pueden prepararse en paralelo mientras T007 está en progreso, siempre que no dependan de su resultado final antes de ejecutarse.

## Implementation Strategy

### Entrega de MVP (Historias 1 y 2)

1. Completar Phase 1 (Foundational: cliente Refit, DTOs, grilla).
2. Completar Phase 2 (US1: carga inicial con datos).
3. Completar Phase 3 (US2: navegación entre páginas).
4. **PARAR y validar**: en este punto el listado es usable de extremo a extremo — MVP entregable.

### Entrega incremental

1. Agregar Phase 4 (US3: estado vacío).
2. Agregar Phase 5 (US4: estado de error).
3. Agregar Phase 6 (US5: estado de carga).
4. Cerrar con Phase 7 (build, tests y evidencia de `quickstart.md`).
