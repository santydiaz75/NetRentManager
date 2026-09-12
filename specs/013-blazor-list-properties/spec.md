# Feature Specification: Listado de Propiedades en Página Principal

**Feature Branch**: `013-blazor-list-properties`

**Created**: 2026-09-12

**Estado**: Implementada

**Input**: User description: "Convertir la página principal (/) de NetRentManagerWeb en un listado paginado de propiedades, consumiendo GET /api/properties del backend mediante un cliente Refit tipado (sin HttpClient directo en componentes), mostrando metadatos de paginación y controles anterior/siguiente, con estados de carga, vacío, error y éxito, respetando el sistema visual de app.css (grilla de 3 columnas en desktop, 2 en tablet, 1 en móvil) y usando Lucide Icons si se requieren íconos. Sin CRUD, filtros avanzados ni cambios de backend."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Ver la primera página de propiedades al entrar (Priority: P1)

Como usuario, quiero abrir la página principal y ver de inmediato una lista de propiedades disponibles, para conocer la oferta sin realizar ninguna acción adicional.

**Why this priority**: Es el valor central de la funcionalidad; sin esta consulta inicial no existe listado que mostrar ni paginación que navegar.

**Independent Test**: Abrir `/` con propiedades existentes en el backend y comprobar que se muestra una grilla con los datos de cada propiedad (título, precio, estado, imagen) y los metadatos de paginación (página actual, total de páginas, total de elementos).

**Acceptance Scenarios**:

1. **Given** el backend tiene propiedades persistidas, **When** el usuario abre `/`, **Then** la página consulta el backend con una página y un tamaño de página por defecto, y muestra las propiedades devueltas junto con los metadatos de paginación.
2. **Given** la respuesta del backend, **When** se renderiza cada propiedad, **Then** se muestran al menos título, precio, estado e imagen (o un marcador visual si no tiene imagen).
3. **Given** un viewport de escritorio, tablet o móvil, **When** se muestra la grilla de propiedades, **Then** se usan 3 columnas en escritorio, 2 en tablet y 1 en móvil, sin estilos inline ni colores hardcodeados.

---

### User Story 2 - Navegar entre páginas de resultados (Priority: P1)

Como usuario, quiero avanzar y retroceder entre páginas de resultados, para explorar todas las propiedades disponibles sin recibir demasiadas a la vez.

**Why this priority**: La paginación es el segundo pilar explícito de la spec; sin ella el listado solo podría mostrar una página fija de resultados.

**Independent Test**: Con más propiedades que el tamaño de página, hacer clic en "Siguiente" y comprobar que se consulta la página siguiente y se actualiza el listado; hacer clic en "Anterior" y comprobar que regresa a la página previa.

**Acceptance Scenarios**:

1. **Given** existen más propiedades que el tamaño de página, **When** el usuario hace clic en "Siguiente", **Then** la página consulta el backend con el número de página incrementado y muestra los nuevos resultados.
2. **Given** el usuario está en una página distinta de la primera, **When** hace clic en "Anterior", **Then** la página consulta el backend con el número de página decrementado y muestra los resultados correspondientes.
3. **Given** el usuario está en la primera página, **When** observa los controles de paginación, **Then** el control "Anterior" aparece deshabilitado según `hasPrevious`.
4. **Given** el usuario está en la última página, **When** observa los controles de paginación, **Then** el control "Siguiente" aparece deshabilitado según `hasNext`.

---

### User Story 3 - Reconocer cuándo no hay resultados (Priority: P2)

Como usuario, quiero ver un mensaje claro cuando no existen propiedades para mostrar, para entender que la ausencia de resultados es un estado válido y no un error.

**Why this priority**: Mejora la comprensión del usuario en un escenario real (catálogo vacío o página fuera de rango), pero no bloquea el valor principal de las Historias 1 y 2.

**Independent Test**: Consultar una página sin propiedades (por ejemplo, un catálogo vacío) y comprobar que se muestra un estado vacío reconocible, usando las clases de estado del sistema visual.

**Acceptance Scenarios**:

1. **Given** el backend no tiene propiedades o la página consultada no contiene elementos, **When** se muestra el resultado, **Then** la página presenta un estado vacío claro en lugar de una grilla en blanco o un error.

---

### User Story 4 - Reconocer errores de comunicación con el backend (Priority: P2)

Como usuario, quiero ver un mensaje claro cuando el listado no puede cargarse por un error del backend o de red, para saber que debo reintentar más tarde en lugar de asumir que no hay propiedades.

**Why this priority**: Protege la experiencia ante fallos reales de comunicación, complementando el camino feliz de las Historias 1 y 2 sin ser un bloqueo para entregarlas primero.

**Independent Test**: Simular una respuesta de error o fallo de red al consultar el backend y comprobar que se muestra un estado de error claro, sin exponer detalles técnicos internos.

**Acceptance Scenarios**:

1. **Given** el backend responde con un error de validación o un error interno, **When** la página intenta cargar el listado, **Then** se muestra un estado de error claro usando las clases de estado del sistema visual, sin datos parciales inconsistentes.
2. **Given** ocurre un fallo de red al consultar el backend, **When** la página intenta cargar el listado, **Then** se muestra el mismo estado de error claro.

---

### User Story 5 - Ver un indicador de carga mientras se consulta el backend (Priority: P3)

Como usuario, quiero ver un indicador de carga mientras la página consulta el backend, para saber que la aplicación está trabajando y no que se ha quedado sin respuesta.

**Why this priority**: Mejora la percepción de la aplicación durante la espera de red, pero es un refinamiento incremental sobre el flujo principal ya cubierto por las historias anteriores.

**Independent Test**: Consultar el listado y comprobar que, mientras la solicitud está en curso, se muestra un estado de carga reconocible antes de que aparezcan los resultados, el estado vacío o el estado de error.

**Acceptance Scenarios**:

1. **Given** la página está consultando el backend, **When** la solicitud aún no ha finalizado, **Then** se muestra un estado de carga claro en lugar de una grilla vacía o contenido parcial.

---

### Edge Cases

- ¿Qué sucede si el usuario navega a una página fuera de rango (mayor que el total de páginas)? El backend la trata como válida y devuelve una lista vacía; la página debe mostrar el estado vacío de la Historia 3, no un error.
- ¿Qué sucede si una propiedad no tiene imagen? La grilla debe mostrar el elemento igualmente, con un marcador visual definido por el sistema visual en lugar de una imagen rota.
- ¿Qué sucede si el usuario hace clic repetidamente en "Siguiente" o "Anterior" antes de que la solicitud anterior finalice? La página debe reflejar consistentemente el resultado de la última página solicitada, sin mezclar resultados de distintas páginas.
- ¿Qué sucede si el backend devuelve `400 Bad Request` por parámetros inválidos generados internamente? Debe tratarse como el mismo estado de error claro de la Historia 4.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: La ruta principal (`/`) DEBE mostrar un listado paginado de propiedades consultado desde `GET /api/properties` del backend `NetRentManagerApi`.
- **FR-002**: La consulta inicial y cada consulta posterior DEBEN enviar explícitamente los parámetros de query `Page` y `PageSize`.
- **FR-003**: Cada propiedad listada DEBE mostrar al menos título, precio, estado e imagen (o un marcador visual si `imageUrl` es nulo).
- **FR-004**: La página DEBE mostrar los metadatos de paginación devueltos por el backend: página actual, total de páginas y total de elementos.
- **FR-005**: La página DEBE incluir controles "Anterior" y "Siguiente" que consulten, respectivamente, la página anterior y la siguiente.
- **FR-006**: El control "Anterior" DEBE deshabilitarse cuando el backend indique `hasPrevious = false`; el control "Siguiente" DEBE deshabilitarse cuando indique `hasNext = false`.
- **FR-007**: La página DEBE mostrar un estado de carga mientras la solicitud al backend está en curso.
- **FR-008**: La página DEBE mostrar un estado vacío claro cuando la respuesta no contenga propiedades, sin tratarlo como error.
- **FR-009**: La página DEBE mostrar un estado de error claro cuando el backend responda con un error de validación, un error interno, o cuando ocurra un fallo de comunicación (red), sin exponer detalles técnicos internos.
- **FR-010**: La grilla de propiedades DEBE usar 3 columnas en escritorio, 2 en tablet y 1 en móvil, mediante clases del sistema visual central (`wwwroot/app.css`), sin estilos inline ni colores hardcodeados.
- **FR-011**: El consumo del backend DEBE realizarse mediante un cliente Refit tipado registrado con `IHttpClientFactory`; ningún componente Razor DEBE usar `HttpClient` directamente ni invocar `RestService.For<T>()` fuera del registro central.
- **FR-012**: Si la interfaz requiere íconos, DEBEN usarse Lucide Icons; no se permite introducir ninguna otra librería de íconos ni framework CSS externo.
- **FR-013**: Esta funcionalidad NO DEBE incluir creación, edición, eliminación ni cambio de estado de propiedades, ni filtros o búsqueda avanzada.
- **FR-014**: Esta funcionalidad NO DEBE modificar el backend, sus contratos ni su comportamiento; DEBE consumir `GET /api/properties` tal como está definido hoy.

### Key Entities

- **PropertyListItem (presentación)**: representación en el frontend de una propiedad listada, con título, descripción, dirección, precio, estado, dormitorios, baños, área e imagen (posiblemente ausente).
- **Página de resultados**: agrupación de propiedades correspondiente a una combinación de número de página y tamaño de página, junto con los metadatos que indican si existen páginas anterior y siguiente.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: El 100% de las cargas exitosas de `/` muestra las propiedades devueltas por el backend junto con los metadatos de paginación, sin intervención adicional del usuario.
- **SC-002**: El 100% de los clics en "Siguiente"/"Anterior" cuando el control está habilitado actualiza el listado a la página correspondiente.
- **SC-003**: El 100% de las respuestas sin propiedades muestra el estado vacío en lugar de una grilla en blanco o un error.
- **SC-004**: El 100% de los errores de backend o de red al cargar el listado muestra el estado de error, sin datos parciales ni detalles técnicos expuestos.
- **SC-005**: El 100% de las capturas de pantalla en escritorio, tablet y móvil muestra la grilla con 3, 2 y 1 columnas respectivamente.
- **SC-006**: El 100% de las llamadas HTTP hacia el backend se realiza mediante el cliente Refit tipado, verificable por ausencia de uso directo de `HttpClient` en componentes.

## Assumptions

- El tamaño de página por defecto enviado por el frontend es `6`, consistente con el valor por defecto documentado del backend (`004-properties-list-pagination`) y con una grilla de 3 columnas en escritorio (dos filas completas).
- El backend `GET /api/properties` y su contrato (`PagedPropertiesResponse`, `PropertyListItem`) ya existen y no se modifican; esta spec solo agrega un consumidor frontend.
- El enlace de navegación "Propiedades" definido en `012-foundation-blazor-frontend` puede seguir apuntando a una ruta distinta de `/`; esta spec no está obligada a modificar la navegación de la sidebar, solo el contenido de la ruta `/`.
- Los íconos, si se usan (por ejemplo, en los controles de paginación), reutilizan el paquete Lucide ya integrado en `012-foundation-blazor-frontend`.
- La interfaz Refit y su registro DI se ubican en una organización por feature bajo el proyecto `NetRentManagerWeb`, sin necesidad de modificar el backend.
- Una página fuera de rango (mayor al total de páginas) es un caso válido que el backend responde con una lista vacía; el frontend lo trata como estado vacío, no como error.
