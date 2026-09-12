# Feature Specification: Fundación del Frontend Blazor

**Feature Branch**: `012-foundation-blazor-frontend`

**Created**: 2026-09-12

**Estado**: Implementada

**Input**: User description: "Crear la especificación 012-foundation-blazor-frontend para establecer la estructura base interna del proyecto Blazor NetRentManagerWeb (app/frontend/src/NetRentManagerWeb), sin lógica de negocio ni pantallas funcionales de propiedades. Es una spec de fundación del frontend, equivalente en espíritu a la 002-foundation-backend pero aplicada a la capa de presentación. Objetivos: (1) garantizar que la aplicación no cargue ningún framework CSS de terceros (Bootstrap, Tailwind, Bulma) ni librería de íconos externa como fuente principal; (2) establecer wwwroot/app.css como única fuente de verdad del sistema visual, con tokens en :root y clases base de layout, botones, cards, formularios, badges, estados de pantalla e íconos; (3) definir el layout base desktop first con sidebar fija a la izquierda y contenido principal amplio a la derecha, con comportamiento responsive en tablet y móvil; (4) establecer Lucide Icons como sistema principal de íconos; (5) establecer el andamiaje base de comunicación tipada con el backend mediante Refit registrado vía IHttpClientFactory, sin endpoints de negocio; (6) definir patrones base reutilizables de estados de pantalla (loading, empty, error, success, con datos); (7) limpiar cualquier estilo hardcodeado heredado de la plantilla por defecto de Blazor. Fuera de alcance: pantallas o CRUD de propiedades, interfaces Refit con endpoints reales, cambios en el backend."

## Clarifications

### Session 2026-09-12

- Q: ¿Qué debe mostrar la navegación de la sidebar en esta fundación, sin pantallas de negocio aún? → A: Incluir enlaces de navegación representativos de las secciones futuras (ej. "Inicio", "Propiedades"), apuntando a páginas vacías o `/`, sin lógica de negocio real.
- Q: ¿De dónde debe leerse la URL base del backend para el HttpClient tipado? → A: Configurar la URL base en `appsettings.json`/`appsettings.Development.json` del frontend, leída vía `IConfiguration`.
- Q: ¿Se debe adaptar el contenido de Home.razor al nuevo sistema visual, o solo envolverlo en el nuevo layout sin tocar su contenido? → A: Adaptar `Home.razor` al nuevo layout con un mensaje placeholder genérico, usando `.page-container`/`.page-header`, sin lógica de negocio.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Arrancar la aplicación sin dependencias visuales externas (Priority: P1)

Como desarrollador del frontend, quiero que `NetRentManagerWeb` arranque sin cargar ningún framework CSS ni librería de íconos de terceros, para cumplir con la constitución del proyecto y evitar dependencias visuales externas antes de construir cualquier pantalla de negocio.

**Why this priority**: Es el requisito de gobernanza más crítico: ninguna pantalla futura puede construirse sobre una base que viole las instrucciones de frontend.

**Independent Test**: Compilar y ejecutar `NetRentManagerWeb`, inspeccionar el HTML servido y confirmar que no se carga ningún CSS ni script de terceros (Bootstrap, Bootstrap Icons, Tailwind, Font Awesome, Fluent Icons).

**Acceptance Scenarios**:

1. **Given** el proyecto `NetRentManagerWeb` compilado, **When** se solicita cualquier página, **Then** el HTML servido únicamente enlaza `app.css` y los assets propios del proyecto, sin referencias a frameworks CSS externos.
2. **Given** el código fuente del proyecto, **When** se revisa `wwwroot/` y los componentes Razor, **Then** no existen carpetas ni referencias a librerías CSS o de íconos de terceros.

---

### User Story 2 - Ver el layout base con sidebar fija y contenido principal (Priority: P1)

Como desarrollador del frontend, quiero un layout base con sidebar fija a la izquierda y contenido principal amplio a la derecha, definido en `wwwroot/app.css` con tokens propios, para que las futuras pantallas de negocio se construyan sobre una estructura visual consistente.

**Why this priority**: El layout base es el cimiento visual de toda la aplicación; sin él, cada feature futura tendría que definir su propia estructura de forma inconsistente.

**Independent Test**: Ejecutar `NetRentManagerWeb` en un viewport de escritorio y verificar visualmente (o mediante inspección del DOM) que existen los contenedores `.app-shell`, `.app-sidebar`, `.app-main` y `.app-content`, y que la sidebar permanece fija al navegar.

**Acceptance Scenarios**:

1. **Given** la aplicación cargada en un viewport de escritorio, **When** el usuario observa la página, **Then** ve una sidebar fija a la izquierda con navegación vertical y un área de contenido principal amplia a la derecha.
2. **Given** la sidebar renderizada, **When** el usuario pasa el cursor sobre un ítem de navegación o lo activa, **Then** el ítem refleja visualmente los estados hover y active definidos en `app.css`.
3. **Given** la aplicación cargada en un viewport de tablet o móvil, **When** el usuario observa la página, **Then** la sidebar se adapta a una versión compacta o colapsada según lo definido en `app.css`, sin romper el contenido principal.
4. **Given** la sidebar renderizada, **When** el usuario observa sus ítems de navegación, **Then** ve enlaces representativos de las secciones futuras del producto (por ejemplo "Inicio" y "Propiedades"), aunque apunten a páginas vacías o a `/`, sin lógica de negocio real.

---

### User Story 3 - Usar íconos Lucide consistentes (Priority: P2)

Como desarrollador del frontend, quiero que los íconos de la navegación y del layout base se rendericen con Lucide Icons y clases CSS reutilizables, para tener un sistema de íconos único y consistente antes de construir pantallas de negocio.

**Why this priority**: Los íconos son parte del layout base (sidebar) definido en la Historia 2, pero su integración como sistema reutilizable es un valor incremental independiente que otras pantallas futuras consumirán.

**Independent Test**: Inspeccionar el HTML renderizado de la sidebar y confirmar que los íconos se renderizan como SVG con clases `.icon`/`.icon-sm`/`.icon-md`/`.icon-lg` y heredan color mediante `currentColor`, sin ninguna librería de íconos adicional cargada.

**Acceptance Scenarios**:

1. **Given** la sidebar renderizada, **When** se inspeccionan sus ítems de navegación, **Then** cada ícono es un SVG (directo o vía componente Razor de Lucide) con clases de tamaño e color definidas en `app.css`.
2. **Given** el proyecto compilado, **When** se revisan sus dependencias, **Then** no existe ninguna referencia a Bootstrap Icons, Font Awesome o Fluent Icons.

---

### User Story 4 - Tener el andamiaje base de comunicación tipada con el backend (Priority: P2)

Como desarrollador del frontend, quiero que el proyecto tenga registrado un cliente tipado Refit hacia el backend mediante `IHttpClientFactory`, sin endpoints de negocio todavía, para que las features futuras solo necesiten agregar sus interfaces Refit sin reconfigurar la comunicación base.

**Why this priority**: Es un requisito de fundación técnica reutilizable por toda spec futura que consuma el backend, pero no bloquea la entrega visual de las Historias 1-3.

**Independent Test**: Inspeccionar el registro de servicios de la aplicación y confirmar que existe un `HttpClient` con nombre y base address configurados hacia el backend, listo para que interfaces Refit futuras se registren sobre él, sin que ningún componente Razor use `HttpClient` directamente.

**Acceptance Scenarios**:

1. **Given** el proyecto `NetRentManagerWeb` compilado, **When** se inspecciona el registro de servicios de arranque, **Then** existe un `HttpClient` nombrado configurado con la URL base del backend mediante `IHttpClientFactory`, sin interfaces Refit de negocio registradas todavía.
2. **Given** el código fuente del proyecto, **When** se revisan los componentes Razor existentes, **Then** ninguno usa `HttpClient` directamente ni invoca `RestService.For<T>()` fuera del registro central.

---

### User Story 5 - Reutilizar patrones base de estados de pantalla (Priority: P3)

Como desarrollador del frontend, quiero clases CSS reutilizables para los estados de pantalla (cargando, vacío, error y éxito con datos), para que las features futuras muestren feedback visual consistente sin inventar estilos nuevos cada vez.

**Why this priority**: Es un patrón reutilizable de valor incremental para futuras pantallas, pero no bloquea la fundación visual ni de comunicación ya cubiertas por las historias anteriores.

**Independent Test**: Revisar `wwwroot/app.css` y confirmar que existen clases documentadas para representar los cuatro estados (cargando, vacío, error, éxito con datos), sin que ninguna pantalla de negocio las consuma todavía.

**Acceptance Scenarios**:

1. **Given** `wwwroot/app.css`, **When** se revisan sus clases, **Then** existen clases reutilizables para los estados de carga, vacío, error y éxito con datos, siguiendo los tokens visuales del sistema.

---

### Edge Cases

- ¿Qué sucede si una future spec necesita un ícono que Lucide no provee? Debe evaluarse como una decisión explícita de esa spec futura, no de esta fundación.
- ¿Qué sucede si el viewport es más angosto que el punto de quiebre definido para tablet? La sidebar debe colapsar o convertirse en navegación alternativa según lo definido en `app.css`, sin ocultar contenido de forma irrecuperable.
- ¿Qué sucede si una página no define contenido dentro de `.app-content`? El layout base debe seguir renderizando `.app-shell`, `.app-sidebar` y `.app-main` de forma consistente, sin errores visuales.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: `NetRentManagerWeb` NO DEBE cargar ningún framework CSS de terceros (Bootstrap, Tailwind, Bulma o similar) ni ninguna librería de íconos externa (Bootstrap Icons, Font Awesome, Fluent Icons) como fuente principal.
- **FR-002**: `wwwroot/app.css` DEBE ser la única fuente de verdad del sistema visual del proyecto, con tokens definidos en `:root` para colores, espaciado, bordes, sombras y layout.
- **FR-003**: `wwwroot/app.css` DEBE definir las clases base de layout (`.app-shell`, `.app-sidebar`, `.app-main`, `.app-content`, `.page-container`, `.page-header`, `.page-title`, `.page-subtitle`, `.page-actions`), botones, cards, formularios y badges.
- **FR-004**: El layout base DEBE presentar una composición desktop-first con sidebar fija a la izquierda y contenido principal amplio a la derecha, reemplazando el `MainLayout` mínimo actual del proyecto.
- **FR-005**: La sidebar DEBE mostrar navegación vertical con estados hover y active definidos en `app.css`, incluyendo enlaces representativos de las secciones futuras del producto (por ejemplo "Inicio" y "Propiedades"), aunque apunten a páginas vacías o a `/` y no incluyan lógica de negocio.
- **FR-006**: El layout DEBE adaptarse de forma responsive en tablet (sidebar compacta) y en móvil (navegación colapsada o tipo drawer), sin ocultar contenido de forma irrecuperable.
- **FR-007**: Los íconos de la navegación y del layout base DEBEN renderizarse mediante Lucide Icons (SVG o componentes Razor), heredar color mediante `currentColor` cuando sea posible, y usar las clases `.icon`, `.icon-sm`, `.icon-md`, `.icon-lg` definidas en `app.css`.
- **FR-008**: El proyecto DEBE registrar un `HttpClient` tipado hacia el backend mediante `IHttpClientFactory`, con la URL base configurada en `appsettings.json`/`appsettings.Development.json` del frontend y leída vía `IConfiguration`, listo para que interfaces Refit de features futuras se apoyen en él, sin incluir endpoints de negocio en esta fundación.
- **FR-009**: Ningún componente Razor DEBE usar `HttpClient` directamente ni invocar `RestService.For<T>()` fuera del registro central de servicios.
- **FR-010**: `wwwroot/app.css` DEBE definir clases reutilizables para los estados de pantalla: cargando, vacío, error y éxito con datos.
- **FR-011**: Cualquier estilo hardcodeado heredado de la plantilla por defecto de Blazor (colores, bordes, referencias a variables de Bootstrap) DEBE eliminarse o reemplazarse por tokens propios de `app.css`, y `Home.razor` DEBE adaptarse al nuevo layout con un mensaje placeholder genérico usando `.page-container`/`.page-header`, sin introducir lógica de negocio.
- **FR-012**: Esta fundación NO DEBE incluir pantallas, componentes o interfaces Refit de negocio relacionadas con propiedades (listados, creación, edición, paginación).

### Key Entities

*No aplica: esta fundación no introduce entidades de dominio ni datos persistentes.*

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: El proyecto `NetRentManagerWeb` compila y arranca sin cargar ningún CSS o script de terceros.
- **SC-002**: El 100% de las páginas renderizadas muestran la sidebar fija y el contenido principal usando exclusivamente clases definidas en `wwwroot/app.css`.
- **SC-003**: El 100% de los íconos visibles en el layout base se renderizan mediante Lucide Icons.
- **SC-004**: El registro base de comunicación con el backend está disponible para que una feature futura agregue su primera interfaz Refit sin modificar la configuración de arranque existente.
- **SC-005**: Un desarrollador nuevo puede identificar visualmente los cuatro estados de pantalla (cargando, vacío, error, éxito con datos) revisando únicamente `wwwroot/app.css`.

## Assumptions

- El proyecto sobre el que aplica esta spec es el existente `NetRentManagerWeb` en `app/frontend/src/NetRentManagerWeb`; no se crea ni renombra a ningún otro proyecto.
- El proyecto actual ya no incluye Bootstrap CSS ni `wwwroot/lib/bootstrap/` (el template base de .NET 10 usado no los genera); los requisitos de ausencia de frameworks CSS se tratan como una condición a preservar y verificar, no como una eliminación de archivos existentes.
- El `HttpClient` tipado hacia el backend se registrará con un nombre y base address consistentes con el proyecto backend existente (`NetRentManagerApi`), configurados en `appsettings.json`/`appsettings.Development.json` del frontend.
- La integración exacta del paquete de Lucide Icons para Blazor (nombre de paquete, versión, forma de renderizado SVG vs. componente Razor) queda para `plan.md` de esta iniciativa.
- Los puntos de quiebre responsive exactos (tablet, móvil) quedan para `plan.md`, siguiendo el estilo visual objetivo descrito en `frontend.instructions.md` y el skill `blazor-app-css-design-system`.
