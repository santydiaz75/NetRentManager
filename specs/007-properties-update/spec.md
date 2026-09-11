# Especificación de la Funcionalidad: Actualización de Propiedades

**Rama de la funcionalidad**: `007-properties-update`

**Creado**: 2026-09-11

**Estado**: Implementada

**Trazabilidad de estado**:

- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar prerrequisitos, diseño, tareas y checklist | Fecha: 2026-09-11
- `En implementación` -> `Implementada` | Motivo: 49 tareas completadas, build correcto, 92 pruebas correctas, validación manual HTTP, p95 inferior a 500 ms y evidencia registrada en `quickstart.md` | Fecha: 2026-09-11

**Entrada**: Descripción de usuario: "Definir un nuevo caso de uso de actualización de propiedades mediante un slice Minimal API en backend, con actualización por id, campos editables y upload opcional de imagen PNG/JPG de hasta 5 MB, conservando la lógica de manejo de imágenes de la creación de propiedades."

## Clarificaciones

### Sesión 2026-09-11

- Q: ¿La actualización `PUT` debe reemplazar todos los campos editables o permitir omitirlos para conservar sus valores? → A: Reemplazo completo; todos los campos editables son obligatorios y solo `image` es opcional.
- Q: ¿Cuándo debe eliminarse la imagen anterior al reemplazarla? → A: Después de confirmar la actualización en base de datos; un fallo de limpieza se registra sin revertir una actualización ya consistente.
- Q: ¿Debe esta iniciativa agregar control de concurrencia para actualizaciones simultáneas? → A: No; prevalece la última actualización válida confirmada.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Actualizar una propiedad sin imagen (Prioridad: P1)

Como usuario autorizado del backend, necesito modificar los datos de negocio de una propiedad existente sin adjuntar una nueva imagen, para corregir o mantener su información sin perder la imagen que ya tiene.

**Por qué esta prioridad**: La actualización de datos sin imagen es el flujo esencial y debe demostrar que la imagen existente se conserva cuando el cliente no solicita reemplazarla.

**Prueba independiente**: Persistir una propiedad con `imageUrl` existente, enviar una solicitud multipart válida sin la parte `image` y comprobar que los campos editables cambian, que la respuesta es exitosa y que `imageUrl` conserva exactamente su valor anterior.

**Escenarios de aceptación**:

1. **Dado** un id existente y un formulario válido sin imagen, **cuando** envío `PUT /api/properties/{id}`, **entonces** recibo HTTP 200, los campos de negocio válidos quedan actualizados y `imageUrl` mantiene el valor anterior.
2. **Dado** un id existente y un formulario válido sin imagen cuya propiedad anterior no tiene imagen, **cuando** envío la actualización, **entonces** la propiedad se actualiza correctamente y `imageUrl` continúa siendo `null`.
3. **Dado** un id inexistente, **cuando** envío un formulario de actualización, **entonces** recibo HTTP 404 y no se modifica ningún registro ni se almacena ningún archivo.

### Historia de Usuario 2 - Reemplazar la imagen de una propiedad (Prioridad: P2)

Como usuario autorizado del backend, necesito actualizar una propiedad adjuntando una imagen PNG o JPG válida, para reemplazar su imagen pública junto con los datos editables del registro.

**Por qué esta prioridad**: El reemplazo de imagen amplía la actualización básica y reutiliza las garantías de validación, nombre seguro y publicación definidas para el alta de propiedades.

**Prueba independiente**: Persistir una propiedad, enviar un formulario válido con una imagen PNG y otro con una imagen JPG/JPEG de hasta 5 MB, y comprobar que cada respuesta es exitosa, que los campos se actualizan y que `imageUrl` apunta a la nueva imagen almacenada bajo `/assets/properties/`.

**Escenarios de aceptación**:

1. **Dado** un id existente y una imagen PNG válida de hasta 5 MB, **cuando** envío la actualización, **entonces** recibo HTTP 200, se almacena una nueva imagen con nombre interno UUID y se persiste su URL pública o relativa bajo `/assets/properties/`.
2. **Dado** un id existente y una imagen JPG/JPEG válida de hasta 5 MB, **cuando** envío la actualización, **entonces** se aplica el mismo comportamiento exitoso y la nueva URL reemplaza la URL anterior.
3. **Dado** una imagen válida cuyo nombre original coincide con otro upload, **cuando** envío la actualización, **entonces** el nombre UUID evita sobrescribir el archivo existente y la URL resultante es distinta y válida.
4. **Dado** una actualización con imagen válida, **cuando** la nueva imagen y los datos se guardan correctamente, **entonces** la respuesta no expone rutas físicas, directorios temporales, `support` ni el nombre original sin sanitizar.

### Historia de Usuario 3 - Rechazar actualizaciones inválidas sin efectos parciales (Prioridad: P3)

Como responsable del sistema, necesito que las actualizaciones inválidas o fallidas se rechacen con códigos consistentes y sin cambios parciales, para proteger la integridad de las propiedades y de sus archivos públicos.

**Por qué esta prioridad**: Las garantías negativas evitan corrupción de datos y archivos huérfanos, aunque se validan después de los flujos exitosos.

**Prueba independiente**: Enviar solicitudes con campos de negocio inválidos, archivos vacíos, formatos no permitidos, contenido real incompatible, tamaños superiores a 5 MB y fallos simulados de almacenamiento; comprobar el código HTTP, el detalle de error y que los datos, `imageUrl` y archivos permanecen consistentes.

**Escenarios de aceptación**:

1. **Dado** un id existente y un campo de negocio ausente o inválido, **cuando** envío la actualización, **entonces** recibo HTTP 400 con detalle por campo y la propiedad conserva todos sus valores anteriores.
2. **Dado** un id existente y un archivo vacío o con contenido real incompatible con PNG/JPG, **cuando** envío la actualización, **entonces** recibo HTTP 415 y no cambia la propiedad, su `imageUrl` ni el almacenamiento público.
3. **Dado** un id existente y un archivo mayor de 5 MB, **cuando** envío la actualización, **entonces** recibo HTTP 413 y no cambia la propiedad, su `imageUrl` ni el almacenamiento público.
4. **Dado** un id existente y un formato no permitido, **cuando** envío la actualización, **entonces** recibo HTTP 415 y no cambia la propiedad ni se conserva un archivo generado para esa solicitud.
5. **Dado** un id existente y un fallo interno al almacenar la nueva imagen, **cuando** termina la operación, **entonces** recibo HTTP 500 con `ProblemDetails`, la propiedad conserva sus valores anteriores y se limpia cualquier archivo nuevo parcial.
6. **Dado** un id existente y un fallo de persistencia después de almacenar la nueva imagen, **cuando** termina la operación, **entonces** recibo HTTP 500, se revierte la actualización de datos, se conserva la URL anterior y se elimina el archivo nuevo o se impide que quede publicado como resultado parcial.

### Casos límite

- Si el id no existe, el sistema debe responder 404 antes de aplicar campos o escribir una imagen.
- Si no se envía la parte `image`, `imageUrl` no debe modificarse, incluso cuando el valor anterior sea `null`.
- Si se envía una parte `image` de cero bytes, debe responder 415 y mantener intacto el estado previo.
- Si la extensión o el MIME parecen válidos pero los bytes reales no son PNG/JPG, debe responder 415.
- Si el tamaño es exactamente 5 MiB (`5 * 1024 * 1024` bytes), debe aceptarse; si supera el límite por un byte, debe responder 413.
- Las extensiones deben validarse sin distinguir mayúsculas y minúsculas, normalizando la extensión persistida a `.png` o `.jpg`.
- El nombre original puede contener rutas, traversal o caracteres inseguros, pero nunca debe usarse como nombre físico ni como parte no validada de `imageUrl`.
- Una colisión de nombre original no debe sobrescribir archivos; el nombre UUID debe generar una URL independiente.
- Si falla la actualización después de escribir la nueva imagen, la URL anterior debe seguir siendo la referencia de la propiedad y el archivo nuevo debe eliminarse o quedar fuera del destino público.
- La imagen anterior no debe eliminarse antes de que la nueva imagen y la actualización de la propiedad hayan quedado confirmadas.
- Un formulario mal formado o un tipo de contenido incompatible debe responder 400 con un detalle accionable sin modificar la propiedad.
- La cancelación de la solicitud debe propagarse y limpiar cualquier archivo temporal o nuevo creado durante el intento.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El backend DEBE exponer un endpoint público `PUT /api/properties/{id}` para actualizar una propiedad por su identificador.
- **RF-002**: El endpoint DEBE recibir una solicitud `multipart/form-data` con los campos editables de la propiedad y una parte de archivo `image` opcional.
- **RF-003**: Antes de actualizar, el caso de uso DEBE buscar la propiedad por `id` en la persistencia.
- **RF-004**: Si la propiedad no existe, el endpoint DEBE devolver HTTP 404 con `ProblemDetails` y NO DEBE modificar datos ni escribir archivos.
- **RF-005**: El caso de uso DEBE aceptar como campos editables obligatorios `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount` y `areaSquareMeters`, conforme al modelo y reglas vigentes de las specs 003 y 006; la ausencia de cualquiera DEBE devolver HTTP 400.
- **RF-006**: Los campos de negocio DEBEN validarse antes de persistir cambios o almacenar una nueva imagen; los errores de negocio DEBEN devolver HTTP 400 con `ValidationProblemDetails` o `ProblemDetails` identificable por campo.
- **RF-007**: `status` DEBE aceptar únicamente `Available`, `Rented` o `Maintenance`, manteniendo la representación textual definida por las specs anteriores.
- **RF-008**: Si la solicitud no incluye `image`, el sistema DEBE conservar sin cambios la `imageUrl` existente, incluyendo su valor `null` cuando corresponda.
- **RF-009**: Si la solicitud incluye `image`, el sistema DEBE aceptar únicamente contenido real PNG o JPG/JPEG válido, usando extensión y MIME solo como señales auxiliares y verificando los bytes del archivo.
- **RF-010**: Un archivo vacío o con formato no permitido DEBE devolver HTTP 415 y NO DEBE modificar la propiedad ni dejar un archivo nuevo publicado.
- **RF-011**: El tamaño máximo permitido DEBE ser 5 MiB, equivalente a `5 * 1024 * 1024` bytes; un archivo exactamente de ese tamaño DEBE aceptarse y uno mayor DEBE devolver HTTP 413.
- **RF-012**: Una imagen válida DEBE almacenarse en `wwwroot/assets/properties` o en el destino runtime equivalente que sirva públicamente `/assets/properties/`.
- **RF-013**: Cada imagen nueva DEBE recibir un nombre interno único basado en UUID y una extensión normalizada válida, sin usar directamente el nombre proporcionado por el cliente.
- **RF-014**: Cuando se almacene una imagen nueva, la propiedad DEBE persistir una `imageUrl` pública o relativa bajo `/assets/properties/{fileName}` y esa URL DEBE reemplazar la referencia anterior solo tras completar correctamente la actualización.
- **RF-015**: La `imageUrl` persistida NO DEBE contener rutas físicas internas, rutas bajo `support`, nombres temporales, traversal ni información del servidor.
- **RF-016**: La actualización con imagen DEBE ser atómica respecto de los datos y la referencia: si falla el almacenamiento, la persistencia o la confirmación de la operación, la propiedad DEBE conservar sus campos y `imageUrl` anteriores, y el archivo nuevo DEBE limpiarse o quedar fuera del destino público consumible.
- **RF-017**: La imagen anterior NO DEBE eliminarse antes de confirmar la nueva referencia en base de datos; después del commit, el sistema DEBE intentar eliminarla y un fallo de esa limpieza DEBE registrarse sin revertir una propiedad que ya apunta a una imagen nueva válida.
- **RF-018**: El endpoint DEBE devolver HTTP 200 con el contrato explícito de la propiedad actualizada, incluyendo `id`, campos persistidos, `status` textual e `imageUrl` nullable.
- **RF-019**: Los fallos internos de almacenamiento, persistencia o coordinación DEBEN devolver HTTP 500 con `ProblemDetails`, sin stack traces, rutas físicas, secretos ni contenido binario.
- **RF-020**: Los formularios mal formados o incompatibles con el contrato de entrada DEBEN devolver HTTP 400 sin aplicar cambios.
- **RF-021**: El flujo DEBE propagar `CancellationToken` desde el endpoint al handler, lectura, escritura y persistencia, ejecutando limpieza ante cancelación.
- **RF-022**: El caso de uso DEBE implementarse como un slice bajo `Features/Properties/UpdateProperty` que exponga `PUT /api/properties/{id}` y se auto-registre mediante `ISlice`.
- **RF-023**: La coordinación de búsqueda, validación de reglas del upload, almacenamiento, actualización y compensación DEBE vivir en un handler que implemente `IHandler`.
- **RF-024**: El mapping DEBE ser explícito, limitarse a transformar requests, entidades y responses, y NO DEBE contener lógica de negocio, acceso a datos, validación de bytes ni escritura de archivos.
- **RF-025**: El validator DEBE vivir junto al slice y registrarse mediante el auto-descubrimiento existente de FluentValidation, sin duplicar validaciones manuales en el endpoint.
- **RF-026**: La implementación NO DEBE introducir controllers, repositorios triviales, mapeadores automáticos, registros manuales de slices o handlers ni una arquitectura paralela.
- **RF-027**: La respuesta y los errores DEBEN mantener compatibilidad con ProblemDetails, el modelo de propiedades y la representación de `imageUrl` nullable definida por las specs 001 a 006.
- **RF-028**: La iniciativa NO DEBE agregar `rowVersion`, `updatedAt` obligatorio, bloqueos de propiedad ni respuesta HTTP 409; ante actualizaciones simultáneas válidas prevalece la última actualización confirmada.
- **RF-029**: Todo código, configuración, migración, contrato, prueba y cambio de asset DEBE quedar trazado a una tarea específica de `tasks.md` antes de marcarse como completado.

### Entidades clave

- **UpdatePropertyRequest**: datos de formulario editables y archivo `image` opcional para una propiedad existente.
- **UpdatePropertyResponse**: contrato de la propiedad actualizada, con estado textual e `imageUrl` nullable.
- **Property**: entidad persistente existente que se busca por id y se modifica solo después de validar el request.
- **PropertyImageUpdate**: intento de reemplazo de imagen que contiene el archivo validado, nombre UUID, ruta pública y datos necesarios para compensación.
- **UpdatePropertySlice**: slice que registra el endpoint `PUT /api/properties/{id}` mediante `ISlice`.
- **UpdatePropertyHandler**: handler que coordina lectura, validación, almacenamiento, persistencia y rollback compensatorio mediante `IHandler`.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las actualizaciones válidas sin imagen devuelve HTTP 200, actualiza todos los campos válidos y conserva exactamente la `imageUrl` previa.
- **SC-002**: El 100% de las actualizaciones válidas con PNG o JPG/JPEG de hasta 5 MiB devuelve HTTP 200, persiste una nueva `imageUrl` bajo `/assets/properties/` y deja disponible el archivo correspondiente.
- **SC-003**: El 100% de los ids inexistentes devuelve HTTP 404 sin cambios en datos ni archivos.
- **SC-004**: El 100% de los archivos vacíos, de formato no permitido o con contenido real inválido devuelve HTTP 415 sin cambiar la propiedad ni publicar archivos parciales.
- **SC-005**: El 100% de los archivos exactamente de 5 MiB se acepta y el 100% de los archivos que superan ese límite devuelve HTTP 413 sin cambiar la propiedad.
- **SC-006**: El 100% de los campos de negocio inválidos devuelve HTTP 400 con un detalle identificable por campo y conserva el estado anterior.
- **SC-007**: El 100% de los fallos simulados durante almacenamiento o persistencia devuelve HTTP 500, conserva los valores anteriores y no deja una nueva imagen pública huérfana verificable.
- **SC-008**: En una muestra funcional de al menos 100 solicitudes válidas de actualización sin imagen y con imagen, el percentil 95 de latencia de respuesta será inferior a 500 ms en el entorno de validación definido en `quickstart.md`.
- **SC-009**: Dos solicitudes que actualicen con el mismo nombre original generan nombres internos UUID distintos y no sobrescriben archivos ni comparten una URL ambigua.
- **SC-010**: Las pruebas unitarias cubren búsqueda por id, ausencia de entidad, actualización sin imagen, reemplazo con PNG/JPG, validación 400/415/413, fallos 500, cancelación, compensación y mapping explícito.
- **SC-011**: El endpoint se auto-descubre mediante `ISlice` y el handler mediante `IHandler`, sin registros manuales nuevos en `Program` ni registradores centrales.
- **SC-012**: Las propiedades existentes creadas por las specs 003 y 006 continúan siendo actualizables, y las propiedades sin imagen conservan `imageUrl: null` al actualizarse sin archivo.

## Suposiciones

- La iniciativa se implementará dentro de `app/backend/src/NetRentManagerApi` y reutilizará la entidad, persistencia, errores y convenciones de las specs 001 a 006.
- La ruta pública canónica será `/assets/properties/{fileName}` y el destino físico será `wwwroot/assets/properties` o su equivalente runtime ya configurado.
- El campo multipart de archivo será `image`, igual que en el alta de propiedades.
- La respuesta exitosa será HTTP 200 y no se añadirá en esta iniciativa un endpoint de detalle separado.
- `PUT` será de reemplazo completo para todos los campos editables; solo la parte `image` es opcional y su ausencia conserva la `imageUrl` anterior.
- “Público” describe la disponibilidad del endpoint en el backend actual; autenticación, autorización y permisos no forman parte de esta iniciativa.
- Los nombres UUID se generan en el servidor y la extensión persistida se normaliza a `.png` o `.jpg` según el contenido válido.
- La actualización de la entidad se considera confirmada junto con la persistencia de la nueva URL; la eliminación de la imagen anterior se intenta solo después del commit y su fallo puede dejar un archivo huérfano, pero no una URL persistida rota.
- La medición de latencia p95 se realizará con la configuración local y la carga funcional documentadas en `quickstart.md`, sin convertirla en una prueba de carga de producción.
- Las pruebas de atomicidad, rollback y limpieza usarán dobles o almacenamiento temporal controlado, sin exigir infraestructura de integración externa no aprobada.

## Compatibilidad y gobernanza

- Esta iniciativa depende de las bases de solución, backend, persistencia, listado y alta definidas por las specs `001-NetRentManager-solution-foundation`, `002-foundation-backend`, `003-properties-persistence-seeding`, `004-properties-list-pagination`, `005-swagger-ui` y `006-properties-create`.
- Debe conservar la arquitectura Vertical Slice existente: slice, handler, mapping y validator por caso de uso, con auto-registro mediante `ISlice` e `IHandler`.
- Debe respetar .NET 10, ASP.NET Core Minimal APIs, EF Core, PostgreSQL, FluentValidation y ProblemDetails definidos por la constitución, sin controllers ni frameworks paralelos.
- Debe respetar el flujo Spec-Driven: esta spec precede a `plan.md`, `tasks.md` e implementación; ningún código o migración puede realizarse sin una tarea trazable.
- El estado inicial es `Borrador`; las transiciones posteriores deben ser realizadas por el flujo Speckit con trazabilidad de origen, destino, motivo y fecha ISO.

## Fuera de alcance

- Crear, eliminar o listar propiedades.
- Actualizar propiedades mediante endpoints distintos de `PUT /api/properties/{id}`.
- Eliminar explícitamente una imagen sin proporcionar una nueva.
- Conversión, redimensionamiento, thumbnails, antivirus, OCR, CDN, object storage o formatos distintos de PNG y JPG/JPEG.
- Cambios en autenticación, autorización, cuotas, rate limiting, frontend, contratos Refit o componentes Blazor.
- Modificar migraciones históricas o cambiar campos de negocio fuera de la actualización descrita.
