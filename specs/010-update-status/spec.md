# Especificación de la Funcionalidad: Actualización de Estado de Propiedad

**Rama de la funcionalidad**: `010-update-status`

**Creado**: 2026-09-11

**Estado**: Implementada

**Trazabilidad de estado**:

- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar prerrequisitos y checklist | Fecha: 2026-09-11
- `En implementación` -> `Implementada` | Motivo: 31 tareas completadas, build correcto, 134 pruebas correctas, OpenAPI validado con Redocly/NSwag y evidencia registrada en quickstart.md | Fecha: 2026-09-11

**Entrada**: Descripción de usuario: "Crear una nueva spec llamada 010-update-status para el backend de NetRentManager API, con un endpoint público para actualizar únicamente el estado de una propiedad existente identificada por su id."

## Clarifications

### Session 2026-09-11

- Q: ¿Qué debe ocurrir si el cuerpo incluye propiedades JSON adicionales como `title`, `price`, `imageUrl` o `image`? → A: Rechazar la solicitud con `400 Bad Request`.
- Q: ¿Cómo debe tratarse un `status` enviado con mayúsculas o minúsculas diferentes? → A: Aceptar sin distinguir mayúsculas/minúsculas y normalizar al valor canónico.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Actualizar el estado de una propiedad (Prioridad: P1)

Como consumidor público de NetRentManager API, necesito actualizar únicamente el estado de una propiedad existente mediante su id, para reflejar si está disponible, alquilada o en mantenimiento sin modificar el resto de sus datos.

**Por qué esta prioridad**: El cambio de estado es el único objetivo de la iniciativa y habilita una operación pequeña, explícita y segura para el ciclo de vida de una propiedad.

**Prueba independiente**: Persistir una propiedad con todos sus datos, enviar una solicitud válida a `PATCH /api/properties/{id}/status` y comprobar que la respuesta es `200 OK`, que devuelve la propiedad actualizada y que solo cambia `status`.

**Escenarios de aceptación**:

1. **Dado** un id existente y una propiedad persistida, **cuando** envío `PATCH /api/properties/{id}/status` con un cuerpo que contiene `status: "Available"`, **entonces** recibo `200 OK`, la respuesta devuelve la propiedad actualizada y su estado es `Available`.
2. **Dado** un id existente y una propiedad persistida, **cuando** envío el estado `Rented` o `Maintenance`, **entonces** recibo `200 OK` y se persiste exactamente el nuevo estado textual.
3. **Dado** una propiedad con título, descripción, dirección, precio, dimensiones e `imageUrl`, **cuando** actualizo su estado, **entonces** todos esos valores permanecen intactos y únicamente cambia `status`.

### Historia de Usuario 2 - Rechazar solicitudes inválidas (Prioridad: P1)

Como consumidor de la API, necesito recibir errores claros cuando el estado falta, no es válido o la propiedad no existe, para poder corregir la solicitud sin producir cambios parciales.

**Por qué esta prioridad**: La validación de entrada y la respuesta de inexistencia protegen la integridad de los datos y son parte del contrato mínimo del endpoint.

**Prueba independiente**: Enviar solicitudes sin `status`, con un valor no permitido y con un id inexistente, comprobando respectivamente `400`, `400` y `404`, todos con el formato de errores vigente.

**Escenarios de aceptación**:

1. **Dado** un id existente, **cuando** envío un cuerpo vacío o sin la propiedad `status`, **entonces** recibo `400 Bad Request` con `ProblemDetails` o `ValidationProblemDetails` y la propiedad no cambia.
2. **Dado** un id existente, **cuando** envío un estado diferente de `Available`, `Rented` o `Maintenance`, **entonces** recibo `400 Bad Request` con un detalle identificable y la propiedad no cambia.
3. **Dado** un id inexistente, **cuando** envío un estado válido, **entonces** recibo `404 Not Found` con `ProblemDetails` y no se crea ni modifica ningún registro.
4. **Dado** una solicitud con un cuerpo incompatible con JSON o con un tipo de contenido no admitido, **cuando** la envío, **entonces** recibo `400 Bad Request` sin modificar la propiedad.

### Historia de Usuario 3 - Consumir el contrato y probarlo manualmente (Prioridad: P2)

Como desarrollador o integrador, necesito que el endpoint esté documentado y tenga un ejemplo ejecutable en el archivo `.http`, para verificar el contrato sin consultar código interno.

**Por qué esta prioridad**: La documentación y el ejemplo reducen errores de integración, aunque dependen de que el caso de uso principal ya esté definido.

**Prueba independiente**: Revisar el documento OpenAPI generado y ejecutar el ejemplo del archivo `.http` con un id y un estado válidos, comprobando método, ruta, cuerpo y respuesta `200`.

**Escenarios de aceptación**:

1. **Dado** el backend compilado, **cuando** consulto el contrato OpenAPI, **entonces** aparece `PATCH /api/properties/{id}/status` con el parámetro `id`, el cuerpo requerido `status` y las respuestas `200`, `400` y `404`.
2. **Dado** el archivo HTTP del proyecto, **cuando** ejecuto el ejemplo de actualización de estado, **entonces** la solicitud usa `PATCH`, la ruta contiene un id de propiedad y el cuerpo solo contiene `status`.
3. **Dado** el endpoint documentado, **cuando** reviso su contrato, **entonces** no aparecen campos de actualización de título, descripción, dirección, precio, dimensiones, `imageUrl` ni upload de imagen.

## Casos límite

- Si `id` no tiene formato GUID válido, la solicitud debe responder `400 Bad Request` con un detalle de validación y no debe ejecutar una actualización.
- Si `status` está presente con valor `null`, vacío o solo espacios, debe responder `400 Bad Request`.
- Si `status` usa mayúsculas o minúsculas diferentes, debe aceptarse y normalizarse a uno de los valores canónicos `Available`, `Rented` o `Maintenance`.
- Si se envían propiedades adicionales como `title`, `price`, `imageUrl` o `image`, la solicitud debe responder `400 Bad Request` y no debe actualizar ningún campo ni introducir upload de archivos.
- Si se envían dos actualizaciones válidas consecutivas para el mismo id, la última confirmada debe dejar el estado correspondiente a la última solicitud aceptada.
- Si la propiedad ya tiene el estado solicitado, la operación debe ser idempotente y devolver `200 OK` sin modificar otros datos.
- Si la persistencia falla durante la actualización, debe responder `500 Internal Server Error` con `ProblemDetails` sin exponer detalles internos.
- Si el cliente cancela la solicitud, la cancelación debe propagarse y no debe quedar una actualización parcial confirmada.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El backend DEBE exponer un endpoint público `PATCH /api/properties/{id}/status` para actualizar el estado de una propiedad existente.
- **RF-002**: El endpoint DEBE recibir un cuerpo JSON con la propiedad obligatoria `status`.
- **RF-003**: `status` DEBE aceptar `Available`, `Rented` y `Maintenance` sin distinguir mayúsculas/minúsculas, y DEBE normalizarse antes de persistir al valor canónico correspondiente.
- **RF-004**: Si `status` falta, es `null`, vacío, no puede convertirse al tipo esperado o no pertenece a los valores permitidos, el endpoint DEBE responder `400 Bad Request` con `ProblemDetails` o `ValidationProblemDetails`.
- **RF-005**: El endpoint DEBE buscar la propiedad por `id` antes de confirmar la actualización.
- **RF-006**: Si no existe una propiedad con el `id` solicitado, el endpoint DEBE responder `404 Not Found` con `ProblemDetails`.
- **RF-007**: Una actualización válida DEBE responder `200 OK` y devolver un contrato explícito de la propiedad actualizada.
- **RF-008**: La respuesta exitosa DEBE incluir como mínimo `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- **RF-009**: La operación DEBE modificar únicamente `status`; `title`, `description`, `address`, `price`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl` DEBEN conservar sus valores persistidos anteriores.
- **RF-010**: El endpoint NO DEBE aceptar ni procesar subida de imagen, multipart/form-data, `image` ni cambios de `imageUrl`.
- **RF-010a**: Si el cuerpo JSON incluye propiedades distintas de `status`, el endpoint DEBE responder `400 Bad Request` y NO DEBE aplicar la actualización.
- **RF-011**: El endpoint DEBE ser público y NO DEBE introducir autenticación, autorización ni permisos nuevos.
- **RF-012**: El caso de uso DEBE implementarse como un slice auto-descubrible bajo `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus`, siguiendo Minimal API y Vertical Slice.
- **RF-013**: El handler DEBE implementar `IHandler` y el slice DEBE implementar `ISlice`, usando el auto-registro existente sin registros manuales en `Program.cs`.
- **RF-014**: La entrada DEBE validarse mediante un validator junto al slice y mediante el mecanismo existente de FluentValidation, sin duplicar reglas manualmente en el endpoint.
- **RF-015**: La respuesta y los errores DEBEN conservar el formato ProblemDetails y las convenciones de estados HTTP vigentes del backend.
- **RF-016**: El flujo DEBE propagar `CancellationToken` hasta la operación de persistencia.
- **RF-017**: Si la persistencia falla inesperadamente, el endpoint DEBE responder `500 Internal Server Error` con `ProblemDetails` sin exponer stack traces, secretos ni detalles internos.
- **RF-018**: El endpoint DEBE quedar incluido en el documento OpenAPI generado, describiendo el parámetro `id`, el cuerpo JSON solo con `status` y las respuestas `200`, `400` y `404`.
- **RF-019**: DEBE existir al menos un ejemplo de solicitud válida en `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` usando `PATCH /api/properties/{id}/status` y un cuerpo que contenga únicamente `status`.
- **RF-020**: La documentación, el contrato y el ejemplo HTTP NO DEBEN incluir campos editables distintos de `status`, imágenes ni upload de archivos.
- **RF-021**: Todo código, configuración, contrato, prueba, ejemplo HTTP y documentación DEBE quedar trazado a una tarea específica de `tasks.md` antes de marcarse como completado.

### Entidades clave

- **UpdatePropertyStatusRequest**: cuerpo JSON que contiene únicamente el nuevo estado solicitado.
- **UpdatePropertyStatusResponse**: contrato explícito de la propiedad actualizada, incluyendo sus datos conservados e `imageUrl` nullable.
- **Property**: entidad persistente existente, buscada por id y modificada exclusivamente en su estado.
- **UpdatePropertyStatusSlice**: slice responsable de registrar `PATCH /api/properties/{id}/status` mediante `ISlice`.
- **UpdatePropertyStatusHandler**: handler responsable de validar el estado, buscar la propiedad, modificar únicamente `status`, persistir y devolver la respuesta mediante `IHandler`.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de las solicitudes válidas con `Available`, `Rented` o `Maintenance` a un id existente responde `200 OK` y devuelve la propiedad actualizada.
- **SC-002**: El 100% de las actualizaciones válidas conserva sin cambios los campos distintos de `status`, incluido `imageUrl`.
- **SC-003**: El 100% de los ids inexistentes responde `404 Not Found` con `ProblemDetails` y sin cambios persistidos.
- **SC-004**: El 100% de los cuerpos sin `status`, con `status` nulo, vacío o inválido responde `400 Bad Request` con un detalle identificable.
- **SC-005**: El 100% de las solicitudes que incluyen `image`, `imageUrl` u otros campos no pertenecientes al caso de uso no modifica esos datos ni almacena archivos.
- **SC-006**: El contrato OpenAPI generado contiene exactamente la operación `PATCH /api/properties/{id}/status`, el cuerpo requerido solo con `status` y las respuestas mínimas `200`, `400` y `404`.
- **SC-007**: El ejemplo HTTP documentado se puede ejecutar contra el backend local con un id válido y devuelve `200 OK` sin enviar otros campos.
- **SC-008**: Las pruebas unitarias verifican actualización correcta, conservación de datos, validación del estado, id inexistente, errores ProblemDetails, cancelación y ausencia de upload.
- **SC-009**: El endpoint se auto-descubre mediante `ISlice` y el handler mediante `IHandler`, sin cambios manuales en los registradores centrales.
- **SC-010**: Una repetición del mismo estado sobre la misma propiedad conserva el mismo resultado observable y no modifica los demás campos persistidos.

## Suposiciones

- La iniciativa se implementará en `app/backend/src/NetRentManagerApi` y reutilizará la entidad `Property`, persistencia, validator global, handler registry, mapping y ProblemDetails existentes.
- La ruta canónica será exactamente `PATCH /api/properties/{id}/status`.
- El cuerpo JSON tendrá exactamente un campo de negocio admitido: `status`.
- Los estados válidos son los definidos por la persistencia y las specs existentes: `Available`, `Rented` y `Maintenance`; la entrada se compara sin distinguir mayúsculas/minúsculas y se persiste normalizada.
- La respuesta reutilizará la forma de contrato explícito de propiedad existente, incluyendo `imageUrl` nullable.
- Las solicitudes con propiedades JSON adicionales se rechazarán con `400 Bad Request`; el contrato de entrada solo admite `status`.
- El ejemplo HTTP usará un GUID de propiedad existente del seeding o un marcador documentado que el desarrollador deba sustituir.
- La autenticación y autorización no forman parte de esta iniciativa porque el endpoint se define como público.
- No se requiere migración de base de datos porque el campo `status` ya existe y solo se modifica su valor.

## Compatibilidad y gobernanza

- Esta iniciativa depende de `001-NetRentManager-solution-foundation`, `002-foundation-backend`, `003-properties-persistence-seeding`, `004-properties-list-pagination`, `006-properties-create`, `007-properties-update`, `008-properties-get-by-id` y `009-open-api`.
- Debe respetar .NET 10, ASP.NET Core Minimal APIs, EF Core, PostgreSQL, FluentValidation, ProblemDetails y Vertical Slice definidos por la constitución.
- Debe mantener la semántica de todos los endpoints existentes y no ampliar el alcance de actualización a otros campos.
- Debe respetar el flujo Spec-Driven: esta spec precede a `plan.md`, `tasks.md` e implementación; ningún código puede realizarse sin una tarea trazable.
- El estado inicial es `Borrador`; las transiciones posteriores deben ser realizadas por el flujo Speckit con trazabilidad de origen, destino, motivo y fecha ISO.

## Fuera de alcance

- Actualizar `title`, `description`, `address`, `price`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` o `imageUrl`.
- Subir, reemplazar, eliminar o procesar imágenes.
- Recibir `multipart/form-data` o archivos.
- Crear, eliminar, listar o consultar propiedades mediante nuevos endpoints.
- Agregar autenticación, autorización, control de concurrencia, `rowVersion`, `updatedAt`, respuesta `409` o bloqueos.
- Cambiar la semántica o los contratos de endpoints existentes.
- Cambiar persistencia, migraciones, seeding o el modelo de estados existente.
- Cambiar frontend, Refit, componentes Blazor o navegación de la aplicación.
