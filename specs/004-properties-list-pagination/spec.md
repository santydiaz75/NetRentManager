# Especificación de la Funcionalidad: Listado Paginado de Propiedades

**Rama de la funcionalidad**: `004-properties-list-pagination`

**Creado**: 2026-09-11

**Estado**: Implementada

**Trazabilidad de estado**:

- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar prerrequisitos y checklist | Fecha: 2026-09-11
- `En implementación` -> `Implementada` | Motivo: todas las tareas completadas, build y 56 pruebas correctas, validación HTTP y evidencia registrada en `quickstart.md` | Fecha: 2026-09-11

**Entrada**: Descripción de usuario: "Definir e implementar un endpoint Minimal API basado en ISlice para devolver la lista paginada de propiedades, incluyendo la imagen correspondiente de cada propiedad mediante URL absoluta pública."

## Clarificaciones

### Sesión 2026-09-11

- Q: ¿Cuál debe ser el valor máximo permitido para `pageSize`? → A: `100` como máximo.
- Q: ¿Cómo debe llamarse la propiedad de la respuesta que contiene la colección de propiedades? → A: `items`.
- Q: ¿Qué debe hacer el endpoint si encuentra una propiedad persistida sin `ImageUrl` válida? → A: Devolver `500` con `ProblemDetails`.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Consultar el primer listado de propiedades (Prioridad: P1)

Como consumidor del backend, necesito consultar las propiedades en páginas ordenadas de forma estable, para mostrar un listado predecible sin cargar todos los registros a la vez.

**Por qué esta prioridad**: El listado paginado es el caso de uso principal y habilita el consumo de propiedades existentes por clientes actuales y futuros.

**Prueba independiente**: Con propiedades semilla existentes, consultar el endpoint sin parámetros y comprobar que devuelve la primera página, hasta seis propiedades y el orden ascendente por título.

**Escenarios de aceptación**:

1. **Dado** un conjunto de propiedades semilla, **cuando** consulto `GET /api/properties` sin parámetros, **entonces** recibo una respuesta exitosa con `page` igual a 1, `pageSize` igual a 6 y como máximo seis elementos ordenados por `title` ascendente.
2. **Dado** un conjunto de propiedades ordenables por título, **cuando** consulto la primera página, **entonces** cada elemento contiene `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.

### Historia de Usuario 2 - Navegar entre páginas (Prioridad: P1)

Como consumidor del backend, necesito solicitar una página y un tamaño concretos, para recorrer el catálogo y conocer si existen páginas anteriores o siguientes.

**Por qué esta prioridad**: Sin metadatos consistentes, un cliente no puede construir navegación fiable ni saber cuándo finaliza el listado.

**Prueba independiente**: Consultar una segunda página con tamaño seis sobre un conjunto de propiedades suficiente y comparar sus elementos y metadatos con el total conocido.

**Escenarios de aceptación**:

1. **Dado** que existen más de seis propiedades, **cuando** consulto `/api/properties?page=2&pageSize=6`, **entonces** recibo la segunda página con hasta seis elementos, sin repetir elementos de la primera página y ordenada por título ascendente.
2. **Dado** cualquier resultado de listado, **cuando** reviso la respuesta, **entonces** contiene una colección `items` y los metadatos `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` y `hasPrevious`, cuyos valores corresponden al conjunto consultado.
3. **Dado** un conjunto vacío o una página posterior a la última, **cuando** consulto el listado, **entonces** recibo una respuesta exitosa con una lista vacía y metadatos coherentes, sin error de rango.

### Historia de Usuario 3 - Obtener imágenes públicas y errores claros (Prioridad: P1)

Como consumidor del backend, necesito una URL absoluta y servible para la imagen de cada propiedad y mensajes de validación claros, para renderizar el listado y corregir solicitudes inválidas.

**Por qué esta prioridad**: Una imagen relativa o una ruta interna no es utilizable fuera del contexto del servidor y una paginación inválida debe fallar de forma explícita.

**Prueba independiente**: Consultar propiedades con imágenes persistidas y enviar valores inválidos para `page` y `pageSize`, comprobando respectivamente URLs absolutas públicas y una respuesta 400 con detalle de validación.

**Escenarios de aceptación**:

1. **Dado** un host local `http://localhost:5023` y propiedades con imagen persistida, **cuando** consulto el listado, **entonces** cada `imageUrl` tiene el formato `http://localhost:5023/assets/properties/{fileName}` y no contiene rutas físicas ni es relativa.
2. **Dado** una solicitud con `page` menor o igual que cero, **cuando** consulto el endpoint, **entonces** recibo estado 400 con `ProblemDetails` o `ValidationProblemDetails` que identifica el parámetro inválido.
3. **Dado** una solicitud con `pageSize` menor o igual que cero, **cuando** consulto el endpoint, **entonces** recibo estado 400 con `ProblemDetails` o `ValidationProblemDetails` que identifica el parámetro inválido.
4. **Dado** una solicitud cancelada, **cuando** se está procesando la consulta, **entonces** la cancelación se propaga y no se continúa procesando innecesariamente la lectura.

### Casos límite

- Si `page` o `pageSize` no puede convertirse al tipo esperado, la respuesta debe ser 400 con un detalle de validación.
- Si solo se proporciona uno de los parámetros, el parámetro ausente conserva su valor predeterminado: `page = 1` o `pageSize = 6`.
- Si `pageSize` es mayor que `100`, debe rechazarse con 400 en lugar de producir una respuesta desproporcionada.
- Si no existen propiedades, la respuesta debe conservar la forma del contrato y devolver `totalItems = 0`, `totalPages = 0`, `hasNext = false` y `hasPrevious = false`.
- Si una propiedad tiene una imagen persistida, el nombre de archivo debe conservarse en la URL pública sin exponer la ruta física de almacenamiento.
- Si una propiedad persistida no tiene una `ImageUrl` válida o no puede transformarse en una URL pública absoluta, el endpoint debe devolver `500` con `ProblemDetails` y no una respuesta parcial.
- Si dos propiedades tienen el mismo título, el orden debe seguir siendo determinista para que la paginación no cambie entre consultas equivalentes.
- Si el host de la solicitud está detrás de una configuración de proxy, la URL pública debe construirse usando el esquema y host que la aplicación reconoce para la solicitud actual.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El backend DEBE exponer un endpoint `GET /api/properties` para listar propiedades persistidas.
- **RF-002**: El endpoint DEBE formar parte de un slice auto-descubrible que implemente `ISlice` y vivir bajo `Features/Properties/ListProperties` dentro del proyecto existente `app/backend/src/NetRentManagerApi`.
- **RF-003**: El endpoint DEBE aceptar los parámetros de consulta `page` y `pageSize`.
- **RF-004**: Cuando no se envíen parámetros, el endpoint DEBE usar `page = 1` y `pageSize = 6`.
- **RF-005**: El endpoint DEBE devolver las propiedades ordenadas por `title` ascendente; en caso de títulos iguales, DEBE aplicar un desempate estable definido durante la planificación.
- **RF-006**: El endpoint DEBE devolver únicamente contratos explícitos de respuesta y NO DEBE exponer directamente entidades persistentes.
- **RF-007**: Cada elemento de respuesta DEBE incluir como mínimo `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- **RF-008**: La respuesta DEBE incluir la colección `items` y los metadatos `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` y `hasPrevious`.
- **RF-009**: `totalItems` DEBE representar el total de propiedades que cumplen la consulta antes de aplicar página y tamaño; `totalPages` DEBE calcularse a partir de ese total y `pageSize`.
- **RF-010**: El valor de `imageUrl` DEBE ser una URL absoluta pública servible por la API con el formato `http(s)://{host}/assets/properties/{fileName}`.
- **RF-011**: El endpoint NO DEBE devolver rutas físicas internas, rutas bajo `support` ni URLs relativas en `imageUrl`.
- **RF-012**: La URL absoluta de imagen DEBE construirse con el esquema y host de la solicitud actual durante el mapping de la respuesta.
- **RF-012a**: Si una propiedad no tiene una `ImageUrl` válida o no puede producir una URL pública absoluta, el endpoint DEBE devolver `500` con `ProblemDetails` y NO DEBE devolver el elemento con una imagen nula, omitirlo o continuar con una respuesta parcial.
- **RF-013**: `page` y `pageSize` DEBEN validarse antes de ejecutar la consulta; los valores menores o iguales que cero y los valores no convertibles DEBEN producir estado 400 con `ProblemDetails` o `ValidationProblemDetails`.
- **RF-014**: El flujo completo DEBE propagar `CancellationToken` desde el endpoint hasta la lectura de datos y el mapping que admita cancelación.
- **RF-015**: La lectura DEBE usar una consulta proyectada y sin seguimiento de entidades, evitando cargar entidades completas cuando el contrato no las necesita.
- **RF-016**: El endpoint DEBE mantener la lógica del caso de uso en su handler y el endpoint debe limitarse a recibir la solicitud, delegar y devolver el resultado.
- **RF-017**: La solución DEBE conservar Minimal APIs, el auto-descubrimiento de `ISlice`, la validación existente y el manejo de `ProblemDetails`, sin introducir controllers ni una jerarquía paralela de endpoints.
- **RF-018**: La prueba manual DEBE incluir solicitudes para `GET /api/properties` y `GET /api/properties?page=1&pageSize=6` en `app/backend/src/NetRentManagerApi/NetRentManagerApi.http`.
- **RF-019**: Todo código, contrato, validación, prueba y cambio de archivo HTTP DEBE quedar trazado a una tarea específica en `tasks.md` antes de marcarse como completado.

### Entidades clave

- **Property**: propiedad persistida que aporta identidad, información descriptiva, precio, dimensiones, estado y referencia de imagen.
- **PropertyListItem**: contrato de salida de un elemento del listado, separado del modelo persistente.
- **PagedPropertiesResponse**: contrato que contiene la colección `items` de propiedades y los metadatos de paginación.
- **ListPropertiesRequest**: parámetros de consulta validados para seleccionar página y tamaño.
- **ListPropertiesSlice**: slice responsable de registrar la ruta del caso de uso y delegar la ejecución.
- **ListPropertiesHandler**: componente responsable de consultar, proyectar, paginar y construir el resultado del caso de uso.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **CE-001**: El 100% de las consultas sin parámetros devuelve `page = 1`, `pageSize = 6` y nunca más de seis propiedades, ordenadas por título ascendente.
- **CE-002**: El 100% de las consultas válidas con `page` y `pageSize` devuelve metadatos matemáticamente consistentes con el total de propiedades y la página solicitada.
- **CE-003**: El 100% de los elementos devueltos con imagen contiene una URL absoluta `http(s)` bajo `/assets/properties/`, sin rutas físicas ni prefijo `support`.
- **CE-004**: El 100% de las solicitudes con `page <= 0`, `pageSize <= 0` o valores no convertibles recibe estado HTTP 400 y un detalle identificable del parámetro inválido.
- **CE-005**: Una consulta válida conserva el mismo orden y contenido de página cuando se repite con el mismo conjunto de datos, incluidos títulos duplicados.
- **CE-006**: Las pruebas del caso de uso verifican cancelación propagada y no dejan consultas activas después de cancelar la solicitud.
- **CE-007**: Las dos solicitudes documentadas en el archivo HTTP se pueden ejecutar contra el backend local y devuelven la forma de contrato definida.
- **CE-008**: No se agregan controllers ni endpoints de propiedades fuera de `Features/Properties/ListProperties`, y el slice queda registrado por el auto-descubrimiento existente.

## Suposiciones

- El proyecto real de backend es `app/backend/src/NetRentManagerApi`; la mención a `NetRentManagerApi` se interpreta como una referencia genérica y no autoriza crear un proyecto paralelo.
- La persistencia de propiedades y el campo `ImageUrl` ya están disponibles conforme a `003-properties-persistence-seeding`.
- Las imágenes públicas se sirven desde `wwwroot/assets/properties` mediante la ruta `/assets/properties/{fileName}`.
- El host y el esquema públicos se obtienen de la solicitud actual y la infraestructura de proxy ya existente será la responsable de proporcionar los valores correctos cuando corresponda.
- El límite máximo de `pageSize` es `100`, incluido en la validación funcional y en las pruebas del endpoint.
- El listado no filtra por estado, texto, precio ni otros criterios; esos casos quedan fuera de esta iniciativa.
- La respuesta puede contener una lista vacía para páginas sin elementos, manteniendo los metadatos del contrato.

## Fuera de alcance

- Crear, actualizar, eliminar o cambiar el estado de propiedades.
- Filtros, búsqueda, ordenamiento configurable, agrupaciones o exportación.
- Cambios en el frontend, contratos Refit o componentes Blazor.
- Autenticación, autorización y permisos nuevos.
- Crear un proyecto o namespace paralelo llamado `NetRentManagerApi`.
- Modificar la persistencia, el seeding o el formato de las imágenes definido por la spec 003.
- Usar controllers, mapeadores automáticos no requeridos o una ruta de endpoints distinta del patrón `ISlice`.
