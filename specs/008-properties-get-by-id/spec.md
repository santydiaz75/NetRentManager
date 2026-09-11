# Especificación de la Funcionalidad: Consulta de Propiedad por Id

**Rama de la funcionalidad**: `008-properties-get-by-id`

**Creado**: 2026-09-11

**Estado**: Implementada

**Trazabilidad de estado**:

- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar prerrequisitos, checklist, plan y tareas | Fecha: 2026-09-11
- `En implementación` -> `Implementada` | Motivo: 40 tareas completadas, build correcto, 107 pruebas correctas, validación manual HTTP y p95 de 176.37 ms registrados en `quickstart.md` | Fecha: 2026-09-11

**Entrada**: Descripción de usuario: "Crear la especificación 008-properties-get-by-id para definir un endpoint público de consulta por id que devuelva una única propiedad mediante GET /api/properties/{id}."

## Clarificaciones

### Decisiones de contrato

- Un `id` con formato que no sea un `Guid` no coincide con la ruta pública restringida y devuelve HTTP 404.
- Un `Guid` válido que no corresponda a una propiedad persistida también devuelve HTTP 404 con `ProblemDetails`.
- La respuesta unitaria reutiliza exactamente los campos públicos del elemento definido por `004-properties-list-pagination`, sin metadatos de paginación.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Consultar una propiedad existente (Prioridad: P1)

Como consumidor del backend, necesito consultar una propiedad concreta por su id, para mostrar sus datos públicos sin descargar el listado completo.

**Por qué esta prioridad**: La consulta individual es el flujo principal y proporciona el recurso base para clientes que ya conocen la identidad de una propiedad.

**Prueba independiente**: Persistir una propiedad con todos sus campos públicos y consultar `GET /api/properties/{id}`; comprobar HTTP 200, el contrato completo y que no se incluyen entidades EF ni metadatos de paginación.

**Escenarios de aceptación**:

1. **Dado** un `Guid` que identifica una propiedad existente, **cuando** envío `GET /api/properties/{id}`, **entonces** recibo HTTP 200 con `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
2. **Dado** una propiedad existente con imagen pública persistida y una request `https://api.example.test`, **cuando** consulto su id, **entonces** `imageUrl` es una URL absoluta `https://api.example.test/assets/properties/{fileName}` construida con el esquema y host de esa request.
3. **Dado** una propiedad existente, **cuando** consulto su id, **entonces** la respuesta no contiene `items`, `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` ni `hasPrevious`.

### Historia de Usuario 2 - Consultar una propiedad sin imagen (Prioridad: P2)

Como consumidor del backend, necesito consultar una propiedad que no tiene imagen, para recibir una representación válida sin que la ausencia de asset rompa la consulta.

**Por qué esta prioridad**: Las propiedades sin imagen son válidas desde las specs 003 y 006, y el contrato debe conservar su `imageUrl: null` sin inventar una URL.

**Prueba independiente**: Persistir una propiedad con `ImageUrl = null`, consultar su id con distintos esquemas y hosts, y comprobar HTTP 200, los campos públicos completos e `imageUrl: null`.

**Escenarios de aceptación**:

1. **Dado** una propiedad existente sin imagen, **cuando** envío `GET /api/properties/{id}`, **entonces** recibo HTTP 200 con `imageUrl: null` y el resto de campos públicos.
2. **Dado** una propiedad sin imagen y una request con esquema u host diferente, **cuando** consulto el recurso, **entonces** la respuesta mantiene `imageUrl: null` y no genera una URL relativa o física.

### Historia de Usuario 3 - Resolver ids inválidos y datos inconsistentes (Prioridad: P3)

Como responsable del sistema, necesito respuestas deterministas para ids inválidos y referencias de imagen inconsistentes, para que los clientes distingan recursos ausentes de fallos internos.

**Por qué esta prioridad**: El flujo negativo no entrega datos de negocio nuevos, pero protege el contrato público y evita respuestas parciales o rutas internas.

**Prueba independiente**: Consultar un id con formato inválido, un `Guid` inexistente y una propiedad con `ImageUrl` inválida; comprobar respectivamente `404`, `404` y `500` con `ProblemDetails` sin información interna.

**Escenarios de aceptación**:

1. **Dado** un segmento `{id}` que no es un `Guid`, **cuando** envío `GET /api/properties/{id}`, **entonces** la ruta no coincide y recibo HTTP 404.
2. **Dado** un `Guid` válido que no existe, **cuando** consulto el endpoint, **entonces** recibo HTTP 404 con `ProblemDetails` y no se expone información sobre otras propiedades.
3. **Dado** una propiedad con una `ImageUrl` física, relativa inválida, con `support`, traversal o sin nombre de archivo válido, **cuando** consulto el endpoint, **entonces** recibo HTTP 500 con `ProblemDetails`, sin respuesta parcial ni ruta interna.
4. **Dado** una solicitud cancelada, **cuando** la consulta aún está en ejecución, **entonces** el `CancellationToken` se propaga y no se continúa procesando innecesariamente la lectura.

### Casos límite

- Un id no convertible a `Guid` debe devolver 404 por no coincidir con la ruta restringida.
- Un `Guid` válido pero inexistente debe devolver 404 con `ProblemDetails`.
- Una propiedad sin imagen debe devolver `imageUrl: null`, no una URL relativa, vacía o física.
- Una `ImageUrl` válida debe convertirse en una URL absoluta usando exactamente el esquema y host de la request actual.
- El resultado debe funcionar con `http` y `https`, y con hosts que incluyan puerto.
- La URL pública debe conservar solo el nombre de archivo y nunca permitir traversal, `support`, una URI absoluta persistida o una ruta física.
- Una `ImageUrl` inválida no debe omitirse ni permitir una respuesta parcial; debe producir 500.
- La respuesta no debe incluir metadatos de paginación ni la colección `items`.
- La consulta debe respetar la cancelación y no exponer stack traces, rutas del servidor o entidades persistentes.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El backend DEBE exponer un endpoint público `GET /api/properties/{id}` para consultar una propiedad por identificador.
- **RF-002**: El endpoint DEBE aceptar únicamente identificadores con formato `Guid` en la ruta; un segmento que no sea `Guid` DEBE responder HTTP 404 por no coincidencia de la ruta.
- **RF-003**: Cuando el `Guid` sea válido pero no exista una propiedad, el endpoint DEBE devolver HTTP 404 con `ProblemDetails` y no exponer datos de otras propiedades.
- **RF-004**: Cuando la propiedad exista, el endpoint DEBE devolver HTTP 200 con un contrato explícito que incluya `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- **RF-005**: El contrato de respuesta DEBE ser consistente con el elemento público definido por `004-properties-list-pagination`, incluyendo `status` como texto y los mismos nombres y tipos públicos.
- **RF-006**: La respuesta unitaria NO DEBE incluir `items`, `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` ni `hasPrevious`.
- **RF-007**: Si la propiedad tiene una referencia de imagen válida, `imageUrl` DEBE ser una URL absoluta pública con formato `http(s)://{host}/assets/properties/{fileName}`.
- **RF-008**: La URL absoluta DEBE construirse durante el mapping usando el esquema y host de la request actual, incluyendo el puerto cuando forme parte del host público.
- **RF-009**: Si la propiedad no tiene imagen (`ImageUrl == null`), el endpoint DEBE devolver HTTP 200 con `imageUrl: null` y el resto del contrato completo.
- **RF-010**: El endpoint NO DEBE devolver rutas físicas, rutas bajo `support`, URLs relativas persistidas, URI absolutas almacenadas ni segmentos con traversal en `imageUrl`.
- **RF-011**: Si la `ImageUrl` persistida no permite producir una URL pública segura, el endpoint DEBE devolver HTTP 500 con `ProblemDetails` y no una respuesta parcial.
- **RF-012**: El caso de uso DEBE implementarse como un slice bajo `Features/Properties/GetPropertyById` mediante Minimal API y auto-registro `ISlice`.
- **RF-013**: La consulta y su coordinación DEBEN vivir en un handler que implemente `IHandler`, sin exponer directamente entidades EF Core.
- **RF-014**: El handler DEBE consultar una única propiedad por id y devolver el resultado mediante un response explícito separado del modelo persistente.
- **RF-015**: El mapping DEBE limitarse a transformar la proyección/entidad y la request actual en el contrato público; no DEBE contener acceso a datos ni reglas de consulta.
- **RF-016**: La consulta DEBE usar proyección sin seguimiento cuando la implementación existente lo permita, cargando solo los campos necesarios para el contrato.
- **RF-017**: El flujo DEBE propagar `CancellationToken` desde el endpoint hasta la consulta de persistencia y cualquier operación de mapping que admita cancelación.
- **RF-018**: El endpoint DEBE declarar metadatos públicos para HTTP 200, 404 y 500, y respuestas JSON con nombres camelCase conforme al contrato existente.
- **RF-019**: Los errores DEBEN usar `ProblemDetails` consistente con las specs previas, sin stack traces, secretos, rutas físicas ni entidades internas.
- **RF-020**: La solución NO DEBE introducir controllers, registros manuales, mapeadores automáticos, paginación, filtros ni una arquitectura paralela.
- **RF-021**: La prueba manual DEBE documentar `GET /api/properties/{id}` con imagen, sin imagen, id inexistente e id inválido en el archivo HTTP o quickstart de la iniciativa.
- **RF-022**: Todo código, contrato, prueba y cambio de archivo HTTP DEBE quedar trazado a una tarea específica en `tasks.md` antes de marcarse como completado.

### Entidades clave

- **Property**: entidad persistente existente que aporta los datos públicos de una propiedad.
- **GetPropertyByIdRequest**: identificador `Guid` recibido en el segmento de ruta y contexto de request necesario para construir la URL pública.
- **GetPropertyByIdResponse**: contrato público de una única propiedad, consistente con `PropertyListItem` de la spec 004 y sin paginación.
- **GetPropertyByIdSlice**: slice que registra `GET /api/properties/{id}` mediante `ISlice`.
- **GetPropertyByIdHandler**: handler que consulta una propiedad, gestiona ausencia y delega el mapping.
- **GetPropertyByIdMapping**: mapping explícito que construye `imageUrl` absoluta o devuelve el error interno correspondiente.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las consultas con `Guid` existente devuelve HTTP 200 y todos los nueve campos públicos definidos por la spec 004.
- **SC-002**: El 100% de las propiedades con imagen válida devuelve una `imageUrl` absoluta `http(s)` bajo `/assets/properties/`, construida con el esquema, host y puerto de la request actual.
- **SC-003**: El 100% de las propiedades sin imagen devuelve HTTP 200 con `imageUrl: null` y sin URL relativa, física o inventada.
- **SC-004**: El 100% de los ids con formato inválido y de los GUID válidos inexistentes devuelve HTTP 404; los GUID inexistentes incluyen `ProblemDetails`.
- **SC-005**: El 100% de las referencias de imagen inválidas detectadas en persistencia devuelve HTTP 500 con `ProblemDetails`, sin rutas físicas ni respuesta parcial.
- **SC-006**: El 100% de las respuestas exitosas no contiene metadatos de paginación ni expone entidades EF Core.
- **SC-007**: Las pruebas unitarias cubren consulta existente, sin imagen, esquema/host variable, id inválido, id inexistente, imagen inválida, cancelación y mapping explícito.
- **SC-008**: En una muestra funcional de al menos 100 consultas válidas, el percentil 95 de latencia será inferior a 250 ms en el entorno documentado en `quickstart.md`.
- **SC-009**: El endpoint se auto-descubre mediante `ISlice` y su handler mediante `IHandler`, sin registros manuales nuevos en `Program` ni registradores centrales.
- **SC-010**: Las solicitudes manuales documentadas para propiedad con imagen, sin imagen, id inexistente e id inválido devuelven la forma de contrato esperada contra el backend local.

## Suposiciones

- La iniciativa reutiliza la entidad `Property`, la persistencia, `ProblemDetails`, static files y los scanners existentes de las specs 001 a 007.
- `PropertyListItem` y el mapping de `004-properties-list-pagination` son la referencia funcional para nombres, campos, estado textual y validación de imagen.
- La ruta pública de assets es `/assets/properties/{fileName}` y los archivos válidos se sirven desde `wwwroot/assets/properties`.
- El esquema y host de la request actual son los valores públicos reconocidos por la aplicación; la configuración de proxy existente es responsable de proporcionar valores correctos cuando aplique.
- Una `ImageUrl` nula representa ausencia válida; una referencia no nula pero insegura representa inconsistencia interna y produce 500.
- El endpoint no requiere autenticación ni autorización nuevas.
- La consulta no modifica datos, archivos ni estado de la propiedad.
- La medición p95 se realizará con al menos 100 solicitudes válidas en el entorno local documentado, sin convertirla en una prueba de carga de producción.

## Compatibilidad y gobernanza

- Esta iniciativa depende explícitamente de `001-NetRentManager-solution-foundation`, `002-foundation-backend`, `003-properties-persistence-seeding`, `004-properties-list-pagination`, `005-swagger-ui`, `006-properties-create` y `007-properties-update`.
- Debe conservar la arquitectura Vertical Slice existente: slice, handler, mapping y validator por caso de uso, con auto-registro mediante `ISlice` e `IHandler`.
- Debe mantener Minimal APIs, .NET 10, EF Core, PostgreSQL, FluentValidation y ProblemDetails definidos por la constitución.
- Debe respetar el flujo Spec-Driven: esta spec precede a `plan.md`, `tasks.md` e implementación; ningún código o contrato puede crearse sin una tarea trazable.
- El estado inicial es `Borrador`; las transiciones posteriores deben ser ejecutadas por Speckit con trazabilidad de origen, destino, motivo y fecha ISO.

## Fuera de alcance

- Listado, paginación, filtros, búsqueda, ordenamiento configurable o exportación.
- Crear, actualizar o eliminar propiedades.
- Modificar entidades, migraciones, seeding o almacenamiento de imágenes.
- Devolver rutas relativas o físicas como contrato público.
- Cambios en frontend, contratos Refit, componentes Blazor, autenticación o autorización.
- Controllers, repositorios triviales, mappers automáticos o registros manuales fuera de los scanners existentes.
