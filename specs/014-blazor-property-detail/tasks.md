# Tasks: Detalle de Propiedad en Blazor

**Input**: Documentos de diseño de `specs/014-blazor-property-detail/`

**Prerequisitos**: `plan.md` (requerido), `spec.md` (requerido para historias de usuario), `research.md`, `data-model.md`, `quickstart.md`

**Pruebas**: Se incluyen tareas de prueba porque la validación del plan exige comprobar carga, estados, navegación y render del detalle.

**Organización**: Las tareas están agrupadas por historia de usuario para permitir implementación y prueba independiente de cada una.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece (`US1`, `US2`, `US3`)
- Cada tarea incluye la ruta exacta del archivo afectado

---

## Phase 1: Setup y base compartida

**Propósito**: Dejar listo el contrato de detalle y los estilos compartidos antes de construir la vista de detalle.

- [X] T001 [P] Crear `app/frontend/src/NetRentManagerWeb/Services/Api/Properties/PropertyDetailResponse.cs` con el contrato público del detalle de propiedad y sus propiedades `Id`, `Title`, `Description`, `Address`, `Price`, `Status`, `BedroomCount`, `BathroomCount`, `AreaSquareMeters` e `ImageUrl`.
- [X] T002 [P] Actualizar `app/frontend/src/NetRentManagerWeb/Services/Api/Properties/IPropertiesApi.cs` para incluir `GetPropertyByIdAsync(string id, CancellationToken cancellationToken = default)` con el endpoint `GET /api/properties/{id}`.
- [X] T003 Crear `app/frontend/src/NetRentManagerWeb/wwwroot/app.css` con estilos adicionales para `.property-detail`, `.detail-hero`, `.detail-content`, `.detail-empty-state`, `.detail-error-state` y el marcador de imagen por defecto.
- [X] T004 [P] Confirmar en `app/frontend/src/NetRentManagerWeb/Features/_Imports.razor` que existen los `using` necesarios para Lucide y los componentes Blazor de Features para la página de detalle.

**Checkpoint**: El contrato de detalle y los estilos base quedan preparados antes de la pantalla.

---

## Phase 2: User Story 1 - Ver el detalle completo de una propiedad (Priority: P1) 🎯 MVP

**Objetivo de la historia**: Al abrir `/properties/{id}`, la página carga la propiedad y muestra todos sus datos públicos.

**Prueba independiente**: Abrir `/properties/{id}` con un id válido y comprobar que la vista renderiza el título, la imagen, la descripción, la dirección, el precio, el estado y las métricas de la propiedad.

### Implementación para User Story 1

- [X] T005 [P] [US1] Crear `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor` con la ruta `@page "/properties/{id}"`, el parámetro `Id`, la estructura principal del layout y el enlace de retorno al listado.
- [X] T006 [US1] Crear `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor.cs` con la lógica de carga inicial y la invocación a `IPropertiesApi.GetPropertyByIdAsync(Id)` usando `OnParametersSetAsync`.
- [X] T007 [US1] En `PropertyDetailPage.razor` renderizar el contenido cargado con título, imagen o marcador por defecto, descripción, dirección, precio, estado, habitaciones, baños y metros cuadrados manteniendo el sistema visual del proyecto.
- [X] T008 [P] [US1] Añadir en `app/frontend/test/NetRentManagerWeb/PropertyDetailPageTests.cs` una prueba que valide que un id existente renderiza el detalle completo y no deja estados vacíos ni de error.

**Checkpoint**: La vista de detalle funcional y el contrato de datos están verificados por la historia principal.

---

## Phase 3: User Story 2 - Reconocer carga, no encontrado y error (Priority: P1)

**Objetivo de la historia**: Diferenciar claramente entre carga en curso, propiedad inexistente y fallo de backend o red.

**Prueba independiente**: Simular `loading`, `404 not found`, `500` o fallo de red y comprobar que cada caso muestra su estado correspondiente sin mezclar mensajes ni contenido parcial.

### Implementación para User Story 2

- [X] T009 [US2] Actualizar `PropertyDetailPage.razor.cs` para mantener estados `IsLoading`, `HasNotFound`, `HasError` y `Property` y para distinguir `404` de errores generales antes de renderizar.
- [X] T010 [US2] En `PropertyDetailPage.razor` añadir los bloques de estado para carga, propiedad no encontrada y error general con mensajes consistentes con la UX del sistema visual.
- [X] T011 [P] [US2] Añadir en `app/frontend/test/NetRentManagerWeb/PropertyDetailPageTests.cs` pruebas para `404 NotFound`, error de red y estado de carga en curso.

**Checkpoint**: La UX del detalle distingue cargas y errores de forma explícita.

---

## Phase 4: User Story 3 - Navegar desde el listado hasta el detalle (Priority: P1)

**Objetivo de la historia**: Que cada tarjeta del listado principal lleve a la vista de detalle correcta con el `id` de la propiedad.

**Prueba independiente**: Desde la página principal, pulsar “Ver” sobre una tarjeta y comprobar que la navegación lleva a `/properties/{id}` y muestra el detalle correspondiente.

### Implementación para User Story 3

- [X] T012 [P] [US3] Actualizar `app/frontend/src/NetRentManagerWeb/Features/Properties/List/PropertyCard.razor` para añadir el botón “Ver” con navegación a `/properties/{id}` mediante `NavigationManager` o enlace de Blazor.
- [X] T013 [US3] Validar en `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor` que la tarjeta se siga renderizando sin romper el listado actual y que la navegación no agrega lógica de negocio ni acceso directo a `HttpClient`.
- [X] T014 [P] [US3] Añadir en `app/frontend/test/NetRentManagerWeb/PropertyCardNavigationTests.cs` una prueba que confirme que el botón “Ver” genera la ruta correcta para la propiedad seleccionada.

**Checkpoint**: El flujo principal desde listado al detalle queda completamente operativo.

---

## Phase 5: Regresión y cierre

**Propósito**: Validar la feature completa y dejar evidencia de ejecución antes de marcarla como implementada.

- [X] T015 [P] Revisar la responsividad y la composición visual final en `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor` y `app/frontend/src/NetRentManagerWeb/wwwroot/app.css`, ajustando solo los estilos trazados por la feature.
- [X] T016 Ejecutar `dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore` y confirmar que las pruebas de detalle y navegación pasan sin regresiones.
- [X] T017 Ejecutar los pasos manuales de `specs/014-blazor-property-detail/quickstart.md` para comprobar detalle válido, id inexistente, estado de carga y volver al listado principal, registrando la evidencia en el mismo documento.
- [X] T018 Verificar que todas las tareas de esta feature están marcadas con `[X]` solo después de build, tests y validación manual completa, antes de cerrar la spec.

---

## Dependencies & Execution Order

### Orden entre fases

- **Setup (Phase 1)**: sin dependencias; prepara el contrato de detalle y los estilos base
- **US1 (Phase 2)**: depende de Setup y entrega el MVP visual del detalle
- **US2 (Phase 3)**: depende de US1 y define el manejo completo de estados
- **US3 (Phase 4)**: depende de US1 y completa el flujo de usuario desde el listado
- **Regresión (Phase 5)**: depende de US1 + US2 + US3

### Dependencias dentro de cada historia

- US1: `T005` y `T008` son paralelos en archivos distintos; `T006` y `T007` dependen del contrato base de `T001` y `T002`.
- US2: `T009` y `T010` comparten el mismo flujo de render del detalle y deben ejecutarse secuencialmente; `T011` depende de la implementación.
- US3: `T012` y `T014` pueden desarrollarse en paralelo después de que la estructura del detalle ya exista; `T013` depende de la navegación del componente.

### Oportunidades de paralelización

- `T001`, `T002` y `T004` pueden ejecutarse en paralelo en la fase 1.
- `T005`, `T008` y `T012` se pueden preparar en paralelo una vez que el contrato base está listo.
- Las pruebas de `US1`, `US2` y `US3` pueden correr de forma independiente después de completar la implementación de cada historia.

---

## Implementation Strategy

### MVP First

1. Completar Phase 1 (contrato + estilos base).
2. Completar Phase 2 (US1: detalle cargado correctamente).
3. Validar el MVP del detalle en una propiedad existente.
4. Completar Phase 3 y Phase 4 para cerrar la experiencia completa.

### Entrega incremental

1. Añadir la vista de detalle con datos válidos.
2. Añadir estados de carga, error y no encontrado.
3. Añadir navegación desde la tarjeta del listado.
4. Ejecutar regresión, validación manual y cierre de la spec.
