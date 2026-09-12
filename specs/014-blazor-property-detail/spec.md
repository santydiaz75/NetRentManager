# Especificación de la Funcionalidad: Detalle de Propiedad en Blazor

**Rama de la funcionalidad**: `014-blazor-property-detail`

**Creado**: 2026-09-12

**Estado**: Implementada

**Trazabilidad de estado**:

- `Borrador` -> `Aprobada` | Motivo: la especificación ha sido creada con alcance claro, requisitos firmes y criterios de validación definidos | Fecha: 2026-09-12
- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar prerrequisitos, tareas y checklist | Fecha: 2026-09-12
- `En implementación` -> `Implementada` | Motivo: 18 tareas completadas, build de solución correcto, 40 pruebas frontend correctas, 140 pruebas backend correctas y validación HTTP/renderizada registrada en quickstart.md | Fecha: 2026-09-12

**Entrada**: Descripción de usuario: "Crear una nueva spec llamada 014-blazor-property-detail para agregar una página de detalle de una propiedad en el frontend Blazor del proyecto NetRentManagerWeb (app/frontend/src/NetRentManagerWeb)."

## Clarifications

### Session 2026-09-12

- Q: ¿Cuál es la ruta canónica de la página de detalle? → A: `/properties/{id}`.
- Q: ¿Qué debe mostrarse cuando `imageUrl` es nulo? → A: Mostrar un marcador visual por defecto consistente con el diseño del sistema visual del proyecto.
- Q: ¿Qué debe ocurrir si el id de la propiedad no existe? → A: Mostrar un estado de error explícito para “propiedad no encontrada” y distinguirlo de errores de red o backend.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Ver el detalle completo de una propiedad (Prioridad: P1)

Como usuario del sitio, quiero abrir la página de detalle de una propiedad con su id en la ruta, para revisar todos sus datos públicos sin necesidad de navegar por el backend ni consultar código interno.

**Por qué esta prioridad**: Es el valor principal de la iniciativa; sin esta vista no existe una forma de explorar la información completa de una propiedad desde la interfaz.

**Prueba independiente**: Abrir `/properties/{id}` con un id válido, esperar a que la página cargue el detalle y comprobar que se muestran todos los campos públicos de la propiedad y el estado visual correcto.

**Escenarios de aceptación**:

1. **Dado** un id de propiedad existente, **cuando** el usuario navega a `/properties/{id}`, **entonces** la página consulta `GET /api/properties/{id}` y muestra el detalle completo de la propiedad.
2. **Dado** una propiedad con imagen disponible, **cuando** la vista carga el detalle, **entonces** se muestra la imagen principal junto con el resto de los datos públicos.
3. **Dado** una propiedad sin `imageUrl`, **cuando** se renderiza la vista, **entonces** se muestra un marcador visual por defecto en lugar de una imagen rota o un hueco vacío.

### Historia de Usuario 2 - Reconocer estados de carga, error y no encontrado (Prioridad: P1)

Como usuario, quiero distinguir claramente entre carga en curso, propiedad inexistente y errores del backend o de red, para entender si el dato está pendiente, ausente o no disponible temporalmente.

**Por qué esta prioridad**: Las experiencias de carga y error son esenciales para que la UI sea usable y no dé la impresión de que la aplicación está rota o bloqueada.

**Prueba independiente**: Cargar una ruta válida, luego un id inexistente y finalmente simular un error de red o de backend para comprobar que cada caso muestra el estado correcto y no mezcla mensajes.

**Escenarios de aceptación**:

1. **Dado** que la solicitud al backend aún está en curso, **cuando** la página de detalle se está cargando, **entonces** muestra un estado de carga explícito usando las clases del sistema visual.
2. **Dado** un id que no corresponde a ninguna propiedad, **cuando** la API responde `404`, **entonces** la página muestra un estado claro de “propiedad no encontrada” y no un error genérico.
3. **Dado** un fallo de red o un error del backend distinto de `404`, **cuando** la petición falla, **entonces** la página muestra un estado de error claro y no expone detalles internos técnicos.

### Historia de Usuario 3 - Navegar desde el listado hasta el detalle (Prioridad: P1)

Como usuario del listado principal, quiero pulsar el botón “Ver” de cualquier tarjeta de propiedad y ser redirigido a la vista de detalle correspondiente, para acceder a la información completa de la propiedad seleccionada.

**Por qué esta prioridad**: El flujo desde la página principal hasta el detalle es la ruta de entrada principal para esta funcionalidad y debe ser explícita y consistente con la navegación de Blazor.

**Prueba independiente**: Desde la página principal, hacer clic en “Ver” sobre una tarjeta y comprobar que la navegación lleva a `/properties/{id}` y que la vista carga el detalle correcto.

**Escenarios de aceptación**:

1. **Dado** una tarjeta de propiedad visible en el listado principal, **cuando** el usuario hace clic en “Ver”, **entonces** la navegación se realiza usando el sistema de Blazor y la ruta incluye el `id` de la propiedad.
2. **Dado** que el usuario está en la página de detalle, **cuando** desea volver al listado, **entonces** debe existir una forma de regreso clara, por ejemplo mediante navegación a la ruta principal o un enlace de retorno.
3. **Dado** cualquier tarjeta del listado, **cuando** se hace clic en “Ver”, **entonces** la operación funciona sin lógica de negocio ni acceso directo a `HttpClient` dentro del componente.

### Historia de Usuario 4 - Revisar el detalle de una propiedad (Prioridad: P2)

Como usuario, quiero ver los datos públicos de una propiedad ordenados y legibles, para evaluar rápidamente si cumple con mis criterios de búsqueda o interés.

**Por qué esta prioridad**: Facilita la comprensión del contenido del inmueble, aunque el valor principal ya está cubierto por la obtención y navegación del dato.

**Prueba independiente**: Cargar una propiedad válida y comprobar que se muestran título, imagen, descripción, dirección, precio, estado, dormitorios, baños y metros cuadrados.

**Escenarios de aceptación**:

1. **Dado** una propiedad válida, **cuando** se abre la vista de detalle, **entonces** se muestran todos los datos públicos definidos en el contrato del backend.
2. **Dado** un conjunto de datos con distintos valores, **cuando** la propiedad se renderiza, **entonces** los campos se presentan en un formato legible y consistente con la aplicación.

## Casos límite

- Si el `id` de la ruta es inválido, la API debe responder `400 Bad Request` y la vista debe mostrar un estado de error de validación.
- Si el `id` no corresponde a ninguna propiedad, la API debe responder `404 Not Found` y la vista debe distinguir el caso como “no encontrado”.
- Si `imageUrl` es `null`, la página debe mostrar un marcador por defecto y no una imagen rota.
- Si la red falla o el backend responde con un error distinto de `404`, la vista debe mostrar el estado de error y no datos parciales inconsistente.
- Si el usuario hace clic en “Ver” desde una tarjeta del listado principal, la navegación debe llevar exactamente a la ruta del detalle con el `id` correcto.
- Si la propiedad ya cargó correctamente, la vista no debe mostrar contenido mezclado con mensajes de error ni con un estado de carga persistente.
- Si el cliente cancela la solicitud, la vista no debe quedar en un estado intermedio ambiguo ni actualizar datos inexistentes.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: La aplicación DEBE exponer una página Blazor accesible por la ruta `GET /properties/{id}` para mostrar el detalle de una propiedad.
- **RF-002**: Cuando la página carga, DEBE consultar `GET /api/properties/{id}` mediante el cliente Refit tipado existente en `IPropertiesApi`, sin usar `HttpClient` directo en componentes Razor.
- **RF-003**: La vista DEBE mostrar los campos públicos de la propiedad: `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- **RF-004**: La página DEBE manejar explícitamente los estados de carga, contenido cargado y error.
- **RF-005**: El estado de error DEBE distinguir claramente entre “propiedad no encontrada” (`404`) y el resto de errores de backend o red.
- **RF-006**: Cuando `imageUrl` es `null`, la página DEBE mostrar un marcador o imagen por defecto consistente con el diseño del proyecto.
- **RF-007**: Cada tarjeta del listado principal DEBE incluir un botón con el texto “Ver” que navegue a la vista de detalle de la propiedad asociada usando el `id`.
- **RF-008**: La vista de detalle DEBE incluir una forma de volver al listado principal, manteniendo la navegación del sistema Blazor y la coherencia de la aplicación.
- **RF-009**: La funcionalidad DEBE reutilizar el contrato tipado del frontend para el detalle de propiedad, sin duplicar modelos manualmente fuera del patrón establecido por el proyecto.
- **RF-010**: La organización del código DEBE seguir una estructura por vertical slice dentro de `Features/Properties`, con una carpeta de detalle coherente con la lista existente.
- **RF-011**: El botón “Ver” DEBE usar la navegación de Blazor (`NavigationManager` o enlace de navegación) y NO DEBE incluir lógica de negocio dentro del componente de tarjeta.
- **RF-012**: Todo el estilo DEBE usar el sistema visual canónico de `wwwroot/app.css`; no se permite inline styles, colores hardcodeados ni frameworks CSS externos.
- **RF-013**: Los íconos, si se usan en la vista de detalle o en controles de navegación, DEBEN ser Lucide Icons y mantener la convención del proyecto.
- **RF-014**: La vista DEBE usar las clases de estado del sistema visual (`loading-state`, `error-state`, `empty-state`, etc.) para carga, vacío y error.
- **RF-015**: La implementación DEBE ser responsive y desktop first, manteniendo una experiencia clara en escritorio, tablet y móvil.
- **RF-016**: Esta iniciativa DEBE ser frontend-only y NO DEBE modificar ni extender el backend ni el contrato del endpoint.
- **RF-017**: La funcionalidad NO DEBE incluir edición, eliminación, cambio de estado ni carga de imágenes en esta vista.
- **RF-018**: El único cambio permitido sobre el listado existente es agregar el botón “Ver” y la navegación asociada, sin rediseñar la tarjeta ni alterar el resto de la información que ya muestra.
- **RF-019**: El flujo debe respetar el seguimiento de Spec-Driven Development: cada cambio de UI, componente, estilo y navegación debe quedar trazado a una tarea de `tasks.md` antes de considerarse finalizado.
- **RF-020**: La navegación desde el detalle de propiedad al listado debe ser compatible con el patrón actual de navegación del frontend y no debe depender de un flujo no documentado ni de llamadas directas a APIs.

### Entidades clave

- **PropertyDetailResponse**: contrato de respuesta del backend para la vista de detalle, con los datos públicos de una propiedad.
- **PropertyDetailPage**: página Blazor responsable de cargar el detalle por id y mostrar los estados de carga, error y contenido.
- **PropertyCard**: componente del listado principal que incluye el botón “Ver” y la navegación a la vista de detalle.
- **NavigationState**: flujo de navegación desde la tarjeta del listado hacia la ruta `/properties/{id}`.
- **PropertyDetailViewModel**: representación consumida por la vista para renderizar el detalle con manejo de estados.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las propiedades existentes con un id válido se muestran en la vista de detalle con todos los datos públicos esperados.
- **SC-002**: El 100% de los clics sobre “Ver” del listado llevan al usuario a la ruta correcta con el `id` asociado y muestran el detalle correspondiente.
- **SC-003**: El 100% de las consultas con un id inexistente muestran un estado de error explícito de “propiedad no encontrada” y no un mensaje genérico.
- **SC-004**: El 100% de las solicitudes con error de red o backend muestra un estado de error claro, sin detalles internos ni contenido parcial inconsistente.
- **SC-005**: El 100% de las propiedades con `imageUrl` nulo muestra un marcador visual por defecto y no una imagen rota.
- **SC-006**: El 100% de las vistas en escritorio, tablet y móvil conserva una composición legible y consistente con el diseño canónico del proyecto.
- **SC-007**: El 100% de las llamadas HTTP al backend se ejecuta a través del cliente Refit tipado ya registrado, sin uso directo de `HttpClient` en componentes.
- **SC-008**: El 100% de las rutas de detalle se carga desde el listado y permiten volver al listado principal sin perder el contexto de navegación de la aplicación.

## Suposiciones

- La vista de detalle reutiliza el cliente Refit existente `IPropertiesApi` y el contrato del backend ya definido en `PropertyDetailResponse`.
- La ruta canónica de la página será `/properties/{id}` y el id se pasará como parámetro de ruta de Blazor.
- El comportamiento de los errores `400` y `404` del backend será manejado por la vista para distinguir validación, no encontrado y error general.
- Si una propiedad no tiene imagen, se mostrará un marcador visual por defecto definido en el diseño del sistema visual actual.
- El listado principal ya existe y solo se añadirá el botón “Ver” y la navegación; el resto del contenido y la estructura del componente no se rediseñarán.
- La aplicación sigue usando Blazor Web App con Razor Components y no se introducen frameworks CSS ni patrones fuera del stack tecnológico obligatorio.
- La navegación de retorno al listado se realizará con los mecanismos estándar de Blazor y no con lógica de negocio ni llamadas API extras.

## Compatibilidad y gobernanza

- Esta iniciativa depende de `010-update-status`, `011-properties-list-pagination` y de la infraestructura frontend previamente establecida por `012-foundation-blazor-frontend`.
- Debe respetar .NET 10, Blazor Web App, Razor Components, Refit con `IHttpClientFactory`, CSS centralizado en `wwwroot/app.css` y Lucide Icons según la constitución.
- Debe mantener la semántica de la página principal y no expandir el alcance a CRUD ni cambios de backend.
- Debe respetar el flujo Spec-Driven: la spec debe preceder a `plan.md`, `tasks.md` e implementación; ninguna tarea puede ejecutarse sin trazabilidad.
- El estado inicial es `Borrador`; las transiciones posteriores deben realizarse por el flujo Speckit, con trazabilidad de origen, destino, motivo y fecha ISO.

## Fuera de alcance

- Modificar el backend, la API o el contrato OpenAPI del endpoint `GET /api/properties/{id}`.
- Implementar edición, borrado o cambio de estado de propiedades desde la vista de detalle.
- Agregar subida de imágenes, gestión de archivos o carga de multipart/form-data.
- Rediseñar el listado principal completo ni eliminar campos que ya existen.
- Introducir frameworks CSS, JavaScript adicional o librerías de iconos distintas de Lucide.
- Crear nuevas rutas o vistas no relacionadas con una propiedad concreta.
- Añadir autenticación, autorización o permisos nuevos para esta vista.
- Incluir filtros avanzados, búsquedas, ordenación o paginación extra distinta de la navegación estándar del listado.
