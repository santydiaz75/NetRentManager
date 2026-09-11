# Especificación de la Funcionalidad: Alta de Propiedades

**Rama de la funcionalidad**: `006-properties-create`

**Creado**: 2026-09-11

**Estado**: Implementada

**Trazabilidad de estado**:

- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar prerrequisitos y checklist | Fecha: 2026-09-11
- `En implementación` -> `Implementada` | Motivo: 43 tareas completadas, migración nullable revisada, build correcto, 72 pruebas correctas y validación manual de POST, listado y asset público registrada en `quickstart.md` | Fecha: 2026-09-11

**Entrada**: Descripción de usuario: "Definir un nuevo caso de uso de alta de propiedades mediante un slice Minimal API en backend, con upload opcional de imagen PNG/JPG de hasta 5 MB."

## Clarificaciones

### Sesión 2026-09-11

- Q: ¿Cómo debe adaptarse el listado de la spec 004 cuando encuentre una propiedad creada sin imagen y `imageUrl` sea `null`? → A: El listado devuelve `imageUrl: null` para propiedades sin imagen.
- Q: ¿Qué respuesta debe devolver el endpoint si falla el almacenamiento de la imagen o la persistencia de la propiedad después de una solicitud válida? → A: HTTP 500 con `ProblemDetails`, limpieza compensatoria y sin propiedad parcial.
- Q: ¿Debe `status` ser obligatorio en la solicitud de creación o debe aplicarse `Available` cuando el cliente no lo envíe? → A: `status` obligatorio; ausencia devuelve HTTP 400.
- Q: ¿Debe permitirse crear varias propiedades con el mismo `title`? → A: Permitir títulos repetidos sin error de unicidad.
- Q: ¿Cómo debe representarse `status` en la respuesta JSON de creación? → A: Texto: `Available`, `Rented` o `Maintenance`.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Crear una propiedad sin imagen (Prioridad: P1)

Como usuario autorizado del backend, necesito registrar una propiedad sin adjuntar una imagen, para poder guardar primero sus datos esenciales y completar la imagen posteriormente.

**Por qué esta prioridad**: La creación sin imagen es el flujo mínimo y garantiza que la ausencia de un archivo no impida persistir una propiedad válida.

**Prueba independiente**: Enviar una solicitud válida sin archivo y comprobar que se crea una propiedad, que la respuesta identifica el registro creado y que `imageUrl` es `null`.

**Escenarios de aceptación**:

1. **Dado** un formulario válido sin imagen, **cuando** envío `POST /api/properties`, **entonces** recibo HTTP 201 y la propiedad queda persistida con todos sus datos, un `Guid Id` único y `imageUrl` igual a `null`.
2. **Dado** una propiedad creada sin imagen, **cuando** consulto posteriormente el listado, **entonces** el elemento completo conserva `imageUrl: null`, devuelve `status` como texto y no es omitido ni provoca un error 500.

### Historia de Usuario 2 - Crear una propiedad con imagen válida (Prioridad: P2)

Como usuario autorizado del backend, necesito adjuntar una imagen PNG o JPG al crear una propiedad, para que el registro quede disponible con una imagen pública consumible por clientes posteriores.

**Por qué esta prioridad**: La imagen mejora el registro, pero no debe bloquear el alta cuando todavía no existe un archivo.

**Prueba independiente**: Enviar una solicitud multipart válida con una imagen PNG o JPG de hasta 5 MB y comprobar que el archivo se almacena en la ubicación pública y que la propiedad conserva la URL pública o relativa acordada.

**Escenarios de aceptación**:

1. **Dado** un formulario válido con una imagen PNG menor o igual a 5 MB, **cuando** envío `POST /api/properties`, **entonces** recibo HTTP 201, la imagen queda almacenada en `wwwroot/assets/properties`, `status` se devuelve como texto y `imageUrl` contiene la ruta pública o relativa correspondiente bajo `/assets/properties/`.
2. **Dado** un formulario válido con una imagen JPG menor o igual a 5 MB, **cuando** envío `POST /api/properties`, **entonces** se aplica el mismo comportamiento exitoso y la respuesta identifica la propiedad creada.
3. **Dado** una imagen válida, **cuando** finaliza el alta, **entonces** la URL persistida no contiene rutas físicas, rutas bajo `support`, información del servidor ni el contenido binario del archivo.

### Historia de Usuario 3 - Rechazar solicitudes inválidas sin efectos parciales (Prioridad: P3)

Como responsable del sistema, necesito que las solicitudes inválidas y los uploads no permitidos se rechacen con errores claros, para evitar propiedades incompletas y archivos huérfanos.

**Por qué esta prioridad**: La validación protege la integridad de los datos y del almacenamiento, aunque se ejecuta después de definir el flujo principal de creación.

**Prueba independiente**: Enviar solicitudes con campos inválidos, formatos no permitidos, archivos mayores de 5 MB y archivos ausentes de contenido; comprobar HTTP 400, `ProblemDetails` o `ValidationProblemDetails`, y ausencia de registros o archivos parciales.

**Escenarios de aceptación**:

1. **Dado** un campo obligatorio ausente o inválido, incluido `status`, **cuando** envío la solicitud, **entonces** recibo HTTP 400 con detalle por campo y no se crea la propiedad.
2. **Dado** un archivo que no es PNG ni JPG, **cuando** envío la solicitud, **entonces** recibo HTTP 400 con error de validación y no se crea la propiedad ni se conserva el archivo.
3. **Dado** un archivo mayor de 5 MB, **cuando** envío la solicitud, **entonces** recibo HTTP 400 con error de tamaño y no se crea la propiedad ni se conserva el archivo.
4. **Dado** un archivo declarado como imagen pero con contenido incompatible con su formato, **cuando** envío la solicitud, **entonces** recibo HTTP 400 y no se crea la propiedad.
5. **Dado** un fallo al guardar la imagen o la propiedad después de validar la solicitud, **cuando** termina la operación, **entonces** recibo HTTP 500 con `ProblemDetails`, no queda una propiedad parcialmente creada y se ejecuta limpieza compensatoria del archivo.

### Casos límite

- Si no se envía el campo de archivo, la operación es válida y `imageUrl` debe persistirse como `null`.
- Si se envía un archivo vacío, debe rechazarse como upload inválido.
- Si el archivo tiene extensión permitida pero su contenido no corresponde a PNG o JPG, debe rechazarse.
- Si el archivo tiene mayúsculas en la extensión, la validación debe ser insensible a mayúsculas y minúsculas.
- Si el tamaño es exactamente 5 MB, debe aceptarse; si supera un byte ese límite, debe rechazarse.
- Si el nombre original contiene rutas, traversal o caracteres no seguros, el sistema debe generar un nombre de almacenamiento seguro y no usar el nombre como ruta física.
- Si dos solicitudes suben archivos con el mismo nombre original, no deben sobrescribirse ni producir URLs ambiguas.
- Si falla la persistencia después de escribir el archivo, el archivo debe eliminarse o quedar fuera del destino público consumible.
- Si falla el guardado de la imagen después de validar los campos, no debe insertarse la propiedad.
- Si el cliente envía un `Content-Type` de formulario incorrecto o una parte de archivo mal formada, debe recibir HTTP 400 con un detalle accionable.
- Si el cliente cancela la solicitud, la operación debe propagar la cancelación y limpiar cualquier archivo temporal ya creado.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El backend DEBE exponer un endpoint público `POST /api/properties` para crear propiedades.
- **RF-002**: El endpoint DEBE implementarse como un slice auto-descubrible mediante `ISlice` y vivir bajo `Features/Properties/CreateProperty` dentro de `app/backend/src/NetRentManagerApi`.
- **RF-003**: El caso de uso DEBE recibir los campos de creación de propiedad y un archivo de imagen opcional mediante una solicitud de formulario multipart.
- **RF-004**: La propiedad DEBE incluir como datos de creación `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount` y `areaSquareMeters`; `title` NO DEBE ser único porque la identidad de cada propiedad depende de su `Guid Id`.
- **RF-005**: Los campos obligatorios DEBEN validarse antes de persistir la propiedad o almacenar el archivo; los errores DEBEN devolver HTTP 400 con `ValidationProblemDetails` o `ProblemDetails` por campo.
- **RF-006**: `status` DEBE ser obligatorio y aceptar únicamente los estados persistentes permitidos por la spec 003: `Available`, `Rented` o `Maintenance`; si falta, DEBE devolverse HTTP 400.
- **RF-007**: Si no se envía imagen, la propiedad DEBE persistirse correctamente con `imageUrl` igual a `null`.
- **RF-008**: Si se envía una imagen, el sistema DEBE aceptar únicamente formatos PNG y JPG/JPEG válidos, verificando el contenido real del archivo además de su nombre o tipo declarado.
- **RF-009**: El tamaño máximo permitido DEBE ser 5 MB, donde 5 MB equivale a 5 * 1024 * 1024 bytes; un archivo de tamaño exactamente igual DEBE aceptarse.
- **RF-010**: Una imagen válida DEBE almacenarse en `app/backend/src/NetRentManagerApi/wwwroot/assets/properties` o en el destino runtime equivalente que sirva públicamente esa ruta.
- **RF-011**: La propiedad DEBE persistir una `imageUrl` pública o relativa bajo `/assets/properties/{fileName}` cuando se almacene una imagen válida.
- **RF-012**: La `imageUrl` persistida NO DEBE contener rutas físicas internas, rutas bajo `support`, nombres de directorio temporales ni el nombre original sin sanitizar cuando pueda producir colisiones o traversal.
- **RF-013**: El nombre físico de cada imagen DEBE ser seguro y suficientemente único para evitar sobrescrituras entre solicitudes concurrentes con el mismo nombre original.
- **RF-014**: La creación DEBE persistir la propiedad y la referencia de imagen de forma coherente; si falla cualquiera de las dos partes, no DEBE quedar una propiedad parcial ni un archivo público huérfano.
- **RF-015**: El endpoint DEBE devolver HTTP 201 al crear correctamente una propiedad y DEBE incluir un contrato explícito con el identificador, los datos persistidos, `status` representado como texto (`Available`, `Rented` o `Maintenance`) e `imageUrl` nullable.
- **RF-016**: La respuesta exitosa DEBE incluir una referencia `Location` o equivalente al recurso creado cuando la infraestructura existente lo permita, sin obligar a implementar un endpoint de consulta individual fuera de esta spec.
- **RF-017**: El endpoint DEBE devolver HTTP 400 para campos inválidos, formato no permitido, contenido de imagen incompatible, archivo vacío o tamaño superior a 5 MB.
- **RF-018**: Los fallos inesperados de almacenamiento o persistencia DEBEN devolver HTTP 500 con `ProblemDetails`, sin exponer stack traces, rutas físicas, secretos ni contenido del archivo, y DEBEN ejecutar limpieza compensatoria para evitar una propiedad parcial o un archivo huérfano.
- **RF-019**: El flujo DEBE propagar `CancellationToken` desde el endpoint al handler, lectura del archivo, escritura del archivo y persistencia asíncrona.
- **RF-020**: La lógica del caso de uso DEBE vivir en un handler que implemente `IHandler`; el endpoint DEBE limitarse a recibir la solicitud, delegar y mapear la respuesta.
- **RF-021**: El mapping DEBE ser explícito y no DEBE contener reglas de negocio, acceso a datos, escritura de archivos ni validación del upload.
- **RF-022**: El validator DEBE vivir junto al slice y registrarse mediante el auto-descubrimiento existente de FluentValidation, sin validación manual duplicada en el endpoint.
- **RF-023**: La solución NO DEBE introducir controllers, repositorios triviales, mapeadores automáticos, registros manuales de slices o handlers ni cambios fuera del alcance de alta de propiedades.
- **RF-024**: El modelo persistente DEBE permitir `imageUrl` nulo para conservar correctamente propiedades creadas sin imagen, y este cambio DEBE quedar trazado a una tarea de la iniciativa.
- **RF-025**: El caso de uso DEBE mantener compatibilidad con el listado de la spec 004: los clientes deben poder distinguir una propiedad sin imagen mediante `imageUrl: null`, y el listado DEBE devolver el elemento completo sin omitirlo ni responder HTTP 500 por esa ausencia válida.
- **RF-026**: Todo código, configuración, prueba, migración, contrato y cambio de asset DEBE quedar trazado a una tarea específica en `tasks.md` antes de marcarse como completado.

### Entidades clave

- **CreatePropertyRequest**: datos de formulario de una nueva propiedad y archivo opcional.
- **CreatePropertyResponse**: contrato explícito de la propiedad creada, incluyendo `imageUrl` nullable.
- **Property**: entidad persistente existente que incorporará la posibilidad de no tener imagen.
- **ImageUpload**: archivo recibido, validado por formato, contenido, tamaño y nombre seguro antes de almacenarse.
- **CreatePropertySlice**: slice que registra `POST /api/properties` mediante `ISlice`.
- **CreatePropertyHandler**: handler que coordina validación de dominio, almacenamiento de archivo, persistencia y limpieza ante fallos.
- **PropertyImageStorage**: capacidad de almacenamiento público de imágenes, solo si la implementación demuestra que debe extraerse para aislar I/O de archivos del handler.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **CE-001**: El 100% de las solicitudes válidas sin imagen crea una propiedad con HTTP 201 y `imageUrl: null`.
- **CE-002**: El 100% de las solicitudes válidas con PNG o JPG/JPEG de hasta 5 MB crea una propiedad con HTTP 201 y una `imageUrl` bajo `/assets/properties/`.
- **CE-003**: El 100% de los archivos de exactamente 5 MB se acepta y el 100% de los archivos mayores se rechaza con HTTP 400.
- **CE-004**: El 100% de los uploads con formato no permitido, contenido incompatible o tamaño inválido recibe HTTP 400 y no deja propiedad ni archivo público asociado.
- **CE-005**: El 100% de los campos obligatorios inválidos recibe errores identificables por campo antes de modificar persistencia o almacenamiento.
- **CE-006**: El 100% de las imágenes almacenadas puede localizarse mediante su URL pública o relativa y ninguna URL contiene `support`, rutas físicas o traversal.
- **CE-007**: Dos solicitudes concurrentes con el mismo nombre original no sobrescriben el archivo de la otra y cada propiedad conserva una URL distinta y válida.
- **CE-008**: El 100% de los fallos simulados durante almacenamiento o persistencia termina sin propiedad parcial ni archivo público huérfano verificable.
- **CE-009**: Las pruebas unitarias cubren el handler, validator, mapping, upload válido, ausencia de imagen, límites de tamaño, formatos, cancelación y limpieza ante error.
- **CE-010**: El endpoint se auto-descubre mediante `ISlice` y el handler mediante `IHandler` sin cambios manuales en registradores centrales.
- **CE-011**: La consulta de listado de la spec 004 continúa serializando `imageUrl: null` para propiedades creadas sin imagen.

## Suposiciones

- La iniciativa se implementará en `app/backend/src/NetRentManagerApi`, sin crear un proyecto paralelo.
- La ruta HTTP será `POST /api/properties` y el formato de entrada será multipart/form-data porque debe transportar campos y un archivo opcional.
- El campo de archivo se llamará `image`; el nombre podrá ajustarse en `plan.md` solo si una convención existente exige otro nombre.
- El contrato de creación devolverá la propiedad creada con `imageUrl` nullable; no se implementará un endpoint adicional de detalle salvo que una tarea lo justifique explícitamente.
- El límite de 5 MB se medirá en bytes usando 5 * 1024 * 1024.
- La ruta pública relativa canónica será `/assets/properties/{fileName}` y el servidor continuará sirviendo esa ubicación mediante la configuración de static files existente.
- La autenticación y autorización no forman parte de esta iniciativa; "público" describe la disponibilidad del endpoint dentro del backend actual.
- La spec 003 seguirá siendo la fuente de verdad de estados, campos de propiedad, PostgreSQL y seeding; esta iniciativa solo amplía la creación y la nulabilidad de imagen.
- Las pruebas de rollback/limpieza usarán dobles o almacenamiento temporal controlado, sin exigir infraestructura de integración adicional.

## Fuera de alcance

- Actualizar o eliminar propiedades.
- Reemplazar o eliminar la imagen de una propiedad ya creada.
- Endpoint de detalle, listado o consulta adicional; el listado existente solo se adapta para aceptar `imageUrl: null`.
- Conversión, redimensionamiento, thumbnails, antivirus, OCR o procesamiento de contenido de imágenes.
- Formatos GIF, WEBP, SVG, PDF, HEIC o cualquier formato distinto de PNG y JPG/JPEG.
- Autenticación, autorización, cuotas por usuario y rate limiting.
- Almacenamiento externo, CDN, object storage o migración de archivos fuera del runtime actual.
- Cambios en el frontend, contratos Refit o componentes Blazor.
- Controllers, mappers automáticos o una arquitectura paralela a Vertical Slice.
