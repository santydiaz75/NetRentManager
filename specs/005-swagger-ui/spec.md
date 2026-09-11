# Especificación de la Funcionalidad: Swagger UI para el Backend

**Rama de la funcionalidad**: `005-swagger-ui`

**Creado**: 2026-09-11

**Estado**: Implementada

**Trazabilidad de estado**:

- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar prerrequisitos y checklist | Fecha: 2026-09-11
- `En implementación` -> `Implementada` | Motivo: 30 tareas completadas, build correcto, 122 pruebas correctas, smoke Playwright 2/2 y validación HTTP Development/Production registrada en `quickstart.md` | Fecha: 2026-09-11

**Entrada**: Descripción de usuario: "Crear la especificación 005-swagger-ui para añadir Swagger UI al proyecto de backend."

## Clarifications

### Session 2026-09-11

- Q: ¿En qué entornos debe estar disponible Swagger UI? → A: Exclusivamente en `Development`.
- Q: ¿Qué biblioteca debe servir la interfaz Swagger UI? → A: `Swashbuckle.AspNetCore`, conectado al documento OpenAPI v1 existente.
- Q: ¿Qué respuesta debe devolver `/swagger` fuera de `Development`? → A: `404 Not Found`, sin contenido de Swagger UI.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Explorar el contrato HTTP en desarrollo (Prioridad: P1)

Como desarrollador del backend, necesito abrir una interfaz Swagger UI durante el desarrollo, para consultar las operaciones HTTP disponibles y sus contratos sin leer manualmente el documento OpenAPI.

**Por qué esta prioridad**: La exploración visual del contrato es el valor principal de la funcionalidad y reduce el tiempo necesario para entender y probar la API.

**Prueba independiente**: Ejecutar el backend en el entorno de desarrollo, abrir la ruta documentada de Swagger UI y comprobar que la interfaz carga con el título de la API y las operaciones públicas existentes.

**Escenarios de aceptación**:

1. **Dado** el backend ejecutándose en el entorno de desarrollo, **cuando** accedo a `/swagger`, **entonces** recibo una interfaz HTML de Swagger UI funcional o una redirección equivalente a su página principal.
2. **Dado** que el documento OpenAPI v1 está disponible, **cuando** carga Swagger UI, **entonces** muestra las operaciones documentadas de salud, listado, detalle, creación y actualización de propiedades.
3. **Dado** un endpoint documentado con parámetros, cuerpo o respuestas de error, **cuando** lo inspecciono en la interfaz, **entonces** puedo identificar sus entradas, respuestas y códigos HTTP sin consultar otra documentación.

### Historia de Usuario 2 - Probar operaciones desde la interfaz (Prioridad: P1)

Como desarrollador, necesito ejecutar solicitudes de prueba desde Swagger UI, para verificar rápidamente el comportamiento de los endpoints con datos controlados y revisar sus respuestas.

**Por qué esta prioridad**: La interfaz debe servir tanto para consultar como para validar manualmente el contrato durante el desarrollo.

**Prueba independiente**: Desde Swagger UI, ejecutar una consulta de salud y una operación de lectura de propiedades, y comprobar que la respuesta mostrada coincide con la respuesta del backend.

**Escenarios de aceptación**:

1. **Dado** un endpoint ejecutable sin autenticación, **cuando** selecciono "Try it out" y envío una solicitud válida, **entonces** Swagger UI muestra el método, la URL, el código HTTP, los encabezados relevantes y el cuerpo de respuesta.
2. **Dado** una solicitud inválida, **cuando** la ejecuto desde Swagger UI, **entonces** la interfaz muestra el código de error y el contrato de `ProblemDetails` devuelto por el backend.
3. **Dado** un endpoint de creación o actualización que recibe multipart/form-data, **cuando** lo inspecciono, **entonces** la interfaz permite identificar y proporcionar los campos y el archivo admitido por el contrato.

### Historia de Usuario 3 - Mantener la UI fuera de producción (Prioridad: P1)

Como responsable de la aplicación, necesito que Swagger UI solo esté disponible en desarrollo, para no ampliar innecesariamente la superficie pública de documentación en entornos productivos.

**Por qué esta prioridad**: La UI es una herramienta operativa para desarrolladores y no debe publicarse accidentalmente junto con el backend productivo.

**Prueba independiente**: Ejecutar el backend con configuración de producción y solicitar la ruta de Swagger UI y sus recursos, comprobando que no se sirven como una interfaz funcional.

**Escenarios de aceptación**:

1. **Dado** el backend ejecutándose fuera de `Development`, **cuando** solicito `/swagger`, **entonces** recibo `404 Not Found` y no se sirve contenido de la interfaz.
2. **Dado** el backend ejecutándose en producción, **cuando** solicito recursos JavaScript, CSS o documentos auxiliares exclusivos de Swagger UI, **entonces** tampoco se sirven desde las rutas de documentación.
3. **Dado** un entorno de desarrollo, **cuando** Swagger UI carga el contrato, **entonces** utiliza únicamente el documento OpenAPI v1 publicado por el backend y no una definición duplicada o editada manualmente.

## Casos límite

- Si el documento OpenAPI v1 no existe, no puede leerse o devuelve un formato inválido, Swagger UI debe mostrar un error de carga identificable y no una pantalla vacía que parezca funcional.
- Si el documento OpenAPI v1 no contiene operaciones, la interfaz debe cargar el error o advertencia correspondiente sin inventar endpoints.
- Si se solicita una ruta de Swagger UI con una variante de barra final, el comportamiento debe ser consistente y no crear una segunda configuración independiente.
- Si una operación devuelve `400`, `404`, `413`, `415` o `500`, Swagger UI debe mostrar el código y el cuerpo de error real sin reemplazarlo por un éxito visual.
- Si una operación requiere un archivo, la interfaz debe conservar la posibilidad de seleccionar el archivo y mostrar los campos del formulario declarados en el contrato.
- Si el entorno no es desarrollo, ningún recurso exclusivo de Swagger UI debe quedar accesible por una ruta alternativa.
- Si el documento OpenAPI cambia, la interfaz debe reflejar el cambio después de reiniciar o recargar el backend sin mantener una copia paralela del contrato.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El backend DEBE ofrecer una interfaz Swagger UI accesible en la ruta `/swagger` cuando se ejecute en el entorno de desarrollo.
- **RF-002**: Swagger UI DEBE mostrar una página de inicio identificable como documentación interactiva de NetRentManagerApi.
- **RF-003**: Swagger UI DEBE cargar el documento OpenAPI público versionado como `v1` desde la ubicación canónica definida por la iniciativa `009-open-api`, sin duplicar ni editar manualmente sus operaciones.
- **RF-003a**: La interfaz DEBE servirse mediante `Swashbuckle.AspNetCore` y su configuración DEBE apuntar al documento OpenAPI v1 existente, sin generar un contrato paralelo.
- **RF-004**: La interfaz DEBE mostrar el 100% de las operaciones HTTP presentes en el documento OpenAPI v1 disponible para el entorno.
- **RF-005**: Para cada operación, la interfaz DEBE mostrar como mínimo método, ruta, parámetros, cuerpo de solicitud cuando aplique, respuestas exitosas y respuestas de error documentadas.
- **RF-006**: La interfaz DEBE permitir ejecutar solicitudes desde la operación seleccionada mediante la acción equivalente a `Try it out`.
- **RF-007**: Las solicitudes ejecutadas desde Swagger UI DEBEN conservar el método, la ruta, los parámetros, los encabezados, el cuerpo y el tipo de contenido definidos por el documento OpenAPI.
- **RF-008**: La interfaz DEBE representar correctamente solicitudes `multipart/form-data`, incluidos los campos de propiedad y el archivo de imagen cuando estén declarados por el contrato.
- **RF-009**: La interfaz DEBE mostrar el código HTTP, los encabezados relevantes y el cuerpo real de la respuesta recibida después de ejecutar una solicitud.
- **RF-010**: Las respuestas de error del backend, incluidos `ProblemDetails` y `HttpValidationProblemDetails`, DEBEN visualizarse como respuestas de error y no como resultados exitosos.
- **RF-011**: Swagger UI y todos sus recursos exclusivos DEBEN estar disponibles únicamente cuando `ASPNETCORE_ENVIRONMENT=Development`.
- **RF-012**: Fuera de `Development`, la ruta `/swagger`, sus recursos estáticos y cualquier documento auxiliar de la interfaz DEBEN devolver `404 Not Found` y NO DEBEN servir contenido de Swagger UI.
- **RF-013**: El acceso a Swagger UI NO DEBE introducir autenticación, autorización ni cambios en los permisos de los endpoints existentes.
- **RF-014**: La configuración DEBE permitir identificar un título, una descripción y la versión del documento mostrado al usuario, manteniéndolos coherentes con el contrato OpenAPI v1.
- **RF-015**: Si el documento OpenAPI no está disponible o no es válido, Swagger UI DEBE mostrar un mensaje de error de carga identificable y no ocultar silenciosamente el fallo.
- **RF-016**: La interfaz DEBE exponer una sola fuente de contrato para v1 y NO DEBE registrar documentos, versiones o rutas Swagger UI duplicados.
- **RF-017**: La implementación DEBE conservar ASP.NET Core Minimal APIs, Vertical Slice, ProblemDetails y la estructura del backend existente; NO DEBE introducir controllers ni un proyecto paralelo.
- **RF-018**: Todo código, configuración, recurso, prueba y cambio de documentación DEBE quedar trazado a una tarea específica en `tasks.md` antes de marcarse como completado.

### Entidades clave

- **Swagger UI**: interfaz web interactiva que permite explorar y ejecutar las operaciones descritas por el contrato OpenAPI.
- **OpenAPI v1**: documento que define las rutas, entradas, respuestas y schemas que Swagger UI debe mostrar.
- **Operation**: operación HTTP documentada, identificada por su método y ruta.
- **Development Environment**: entorno en el que la UI está disponible para tareas de desarrollo y validación manual.
- **Production Environment**: entorno en el que la UI y sus recursos exclusivos no deben estar publicados.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **CE-001**: En el 100% de los arranques en desarrollo, `/swagger` carga una interfaz funcional o una redirección equivalente y no devuelve una página de error inesperada.
- **CE-002**: El 100% de las operaciones presentes en OpenAPI v1 aparece en Swagger UI con método y ruta coincidentes.
- **CE-003**: Un desarrollador puede ejecutar una consulta de salud y una consulta de propiedades desde la interfaz en menos de 2 minutos, sin construir manualmente las solicitudes fuera de Swagger UI.
- **CE-004**: El 100% de las respuestas de prueba, incluidas respuestas 2xx y errores 4xx/5xx, se muestra con su código HTTP y cuerpo real.
- **CE-005**: El 100% de las operaciones multipart documentadas permite identificar sus campos y seleccionar el archivo correspondiente desde la interfaz.
- **CE-006**: En el 100% de las validaciones de producción, `/swagger` y sus recursos exclusivos no sirven una UI funcional ni un documento auxiliar duplicado.
- **CE-007**: Cuando el documento OpenAPI está ausente o es inválido, el 100% de las cargas de Swagger UI muestra un error identificable para el desarrollador.
- **CE-008**: La solución no agrega controllers, endpoints de negocio duplicados ni una segunda fuente de verdad del contrato.

## Suposiciones

- La iniciativa `009-open-api` define y publica el documento OpenAPI v1 consumido por Swagger UI; si su planificación aún no está implementada, el orden de implementación resolverá primero el contrato requerido.
- La ruta canónica de la interfaz será `/swagger` y la ruta de la página principal podrá normalizarse mediante una redirección equivalente.
- La API actual es pública y no requiere credenciales para ejecutar las operaciones existentes desde la interfaz.
- La interfaz se habilitará exclusivamente mediante la configuración estándar `ASPNETCORE_ENVIRONMENT=Development`; cualquier otro valor de entorno se considera no autorizado para Swagger UI.
- Los navegadores de desarrollo tienen acceso al backend y permiten ejecutar solicitudes HTTP desde el mismo origen de la aplicación.
- Swagger UI se considera una herramienta de desarrollo y no se requiere personalización visual avanzada, branding adicional ni soporte de múltiples documentos en esta versión.
- `Swashbuckle.AspNetCore` se utilizará para servir los recursos de Swagger UI; la fuente de operaciones seguirá siendo exclusivamente el documento OpenAPI v1 de `009-open-api`.
- La validación fuera de `Development` se realizará mediante solicitudes HTTP y comprobará `404 Not Found` tanto para la página como para sus recursos exclusivos.

## Fuera de alcance

- Crear o modificar endpoints de negocio, contratos de propiedades, persistencia, seeding o reglas de validación.
- Generar el documento OpenAPI v1, que pertenece a `009-open-api`.
- Publicar Swagger UI, ReDoc, Scalar u otra interfaz de documentación en producción.
- Añadir autenticación, autorización, usuarios, roles o permisos específicos para Swagger UI.
- Crear múltiples versiones del contrato o permitir seleccionar documentos OpenAPI alternativos.
- Cambiar el frontend Blazor, los clientes Refit o la navegación de la aplicación.
- Personalizar profundamente el tema visual, agregar branding comercial o construir una página de documentación independiente.
