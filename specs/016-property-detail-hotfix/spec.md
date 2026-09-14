# Especificación de la Funcionalidad: Hotfix del Detalle de Propiedades

**Feature Branch**: `[016-property-detail-hotfix]`

**Created**: 2026-09-13

**Estado**: Implementada

**Input**: Documentación retrospectiva del hotfix ya implementado, basada en el estado actual del repositorio, las pruebas relacionadas y el comportamiento observable del frontend.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Abrir el detalle sin conflicto de navegación (Priority: P1)

Como usuario del listado de propiedades, quiero abrir una propiedad concreta desde su URL de detalle y ver su contenido directamente, para continuar la navegación sin errores bloqueantes.

**Why this priority**: Si la URL de detalle no puede resolverse de forma inequívoca, el flujo principal desde el listado queda interrumpido y la pantalla no se puede usar.

**Independent Test**: Navegar a una URL válida de detalle de propiedad y comprobar que la aplicación muestra una única vista de detalle en lugar de un error de coincidencia ambigua.

**Acceptance Scenarios**:

1. **Given** una URL de detalle válida, **When** el usuario navega a la pantalla de detalle, **Then** la aplicación resuelve una única vista para esa URL.
2. **Given** una propiedad existente, **When** el usuario abre su detalle, **Then** la pantalla carga el contenido esperado y no muestra un error de navegación ambiguo.

---

### User Story 2 - Ver el detalle con composición visual correcta (Priority: P1)

Como usuario que consulta una propiedad, quiero ver el encabezado, la acción de regreso y el contenido principal en posiciones claras y consistentes, para comprender la información sin confusión visual.

**Why this priority**: La pantalla de detalle es una vista de lectura principal y una composición incorrecta degrada la legibilidad y la percepción de calidad.

**Independent Test**: Abrir una propiedad válida y comprobar que la acción "Volver al listado" permanece en el encabezado superior, mientras que la imagen y los datos del inmueble aparecen dentro del bloque principal del detalle.

**Acceptance Scenarios**:

1. **Given** una propiedad con imagen disponible, **When** se muestra el detalle, **Then** la imagen se presenta dentro del contenido principal y no desplazada junto al botón de regreso.
2. **Given** la pantalla de detalle cargada, **When** el usuario observa el encabezado, **Then** la acción de regreso aparece separada del bloque visual del inmueble.
3. **Given** una propiedad sin imagen, **When** se muestra el detalle, **Then** la pantalla mantiene una composición estable con un marcador visual en lugar de un espacio roto.

---

### User Story 3 - Navegar con señales visuales coherentes (Priority: P2)

Como usuario de la aplicación, quiero ver iconos válidos en la navegación principal y una vía clara de retorno al listado, para moverme por la interfaz sin indicadores erróneos o confusos.

**Why this priority**: Aunque no bloquea toda la funcionalidad, un icono roto en la navegación transmite un estado erróneo y reduce la claridad de la interfaz.

**Independent Test**: Cargar la aplicación, revisar la navegación principal y comprobar que el acceso a propiedades muestra un icono válido y una etiqueta legible.

**Acceptance Scenarios**:

1. **Given** la aplicación cargada, **When** el usuario observa el enlace principal de propiedades, **Then** se muestra un icono válido en lugar de un símbolo de alerta.
2. **Given** el usuario está en el detalle de una propiedad, **When** decide volver al listado, **Then** encuentra una acción visible y comprensible para regresar.

---

### Edge Cases

- Si la URL de detalle apunta a un identificador inexistente, la aplicación debe evitar errores de navegación ambiguos y mostrar un estado comprensible para el usuario.
- Si la propiedad no tiene imagen, la vista debe mantener una estructura estable y legible.
- Si el usuario vuelve al listado desde el detalle, la navegación debe seguir siendo clara aunque la propiedad esté en estado no encontrado o error.
- Si existe más de una definición funcional para la misma URL de detalle, el sistema debe comportarse como si solo hubiera una vista canónica disponible para el usuario.
- Si la navegación principal contiene un icono no reconocido, la interfaz no debe depender de ese estado defectuoso para seguir siendo interpretable.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema DEBE resolver una única vista para cada URL válida de detalle de propiedad.
- **FR-002**: El sistema DEBE permitir abrir el detalle de una propiedad sin mostrar errores de coincidencia ambigua durante la navegación.
- **FR-003**: La vista de detalle DEBE mantener el encabezado y la acción de regreso separados del bloque principal de contenido.
- **FR-004**: La vista de detalle DEBE presentar la información principal de la propiedad en una composición legible y consistente.
- **FR-005**: Cuando no exista imagen disponible, la vista DEBE mostrar un marcador visual coherente sin romper la disposición general.
- **FR-006**: El usuario DEBE disponer de una forma visible de volver al listado desde el detalle.
- **FR-007**: La navegación principal DEBE mostrar un icono válido y reconocible para el acceso al listado de propiedades.
- **FR-008**: La corrección retrospectiva DEBE conservar el comportamiento actual ya observable en la aplicación sin introducir flujos nuevos.

### Key Entities *(include if feature involves data)*

- **Vista de detalle de propiedad**: Pantalla que presenta la información pública de una propiedad concreta y ofrece una acción de retorno al listado.
- **Ruta de detalle**: Dirección navegable que identifica de forma inequívoca la pantalla de una propiedad específica.
- **Elemento de navegación principal**: Acceso visible al listado de propiedades dentro del menú lateral de la aplicación.
- **Estado visual del detalle**: Presentación observable de encabezado, acción de regreso, imagen o marcador, y datos principales del inmueble.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: El 100% de las aperturas manuales de una URL válida de detalle muestran una única pantalla de detalle y no un error de coincidencia ambigua.
- **SC-002**: El 100% de las revisiones visuales del detalle muestran la acción de regreso en el encabezado y el contenido del inmueble en su bloque principal.
- **SC-003**: El 100% de las propiedades sin imagen conservan una presentación estable y legible mediante un marcador visual.
- **SC-004**: El 100% de las cargas de la navegación principal muestran un icono válido en el acceso a propiedades, sin símbolos de alerta derivados de un icono no reconocido.

## Assumptions

- El diff actual de Git no contiene cambios funcionales activos; la documentación retrospectiva se apoya en el estado presente del repositorio y en las pruebas disponibles.
- El hotfix documentado se limita a estabilizar el flujo de detalle de propiedades y su navegación asociada, sin añadir capacidades nuevas.
- El listado principal y la obtención de datos de propiedades ya existían antes del hotfix y se mantienen como base del comportamiento observable.
- La validación principal de esta documentación retrospectiva se apoya en revisión manual del frontend y en pruebas estáticas o unitarias ya presentes en el proyecto.
