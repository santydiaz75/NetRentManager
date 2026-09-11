# Investigación: Alta de Propiedades

## Decisión 1: Nulabilidad de `ImageUrl`

- **Decisión**: Cambiar `Property.ImageUrl` a `string?`, configurar `image_url` como nullable en `PropertyConfiguration` y generar una migración EF Core que altere únicamente esa columna.
- **Razonamiento**: La ausencia de imagen es un estado válido fijado por la spec. Mantener una cadena vacía confundiría ausencia con una ruta y rompería el contrato `imageUrl: null`.
- **Alternativas consideradas**: Usar una cadena vacía, rechazado porque no representa `null`; crear una tabla separada de imágenes, rechazado porque excede el alcance y no es necesario para una imagen opcional.

## Decisión 2: Binding multipart

- **Decisión**: Registrar `POST /api/properties` con un request de formulario explícito y un `IFormFile?` opcional llamado `image`. El endpoint solo enlaza la entrada, resuelve el handler por `IHandler` y delega; la validación y lectura efectiva permanecen en el caso de uso.
- **Razonamiento**: Minimal APIs requieren declarar explícitamente la fuente de formulario para evitar inferir un body JSON. El campo de archivo debe ser nullable para que el flujo sin imagen sea válido.
- **Alternativas consideradas**: Recibir `IFormCollection` y parsear todos los campos manualmente, rechazado porque duplica binding y dificulta la validación; aceptar JSON con base64, rechazado porque no es multipart y aumenta memoria.

## Decisión 3: Validación de imagen

- **Decisión**: Validar archivo vacío, extensión y `ContentType` permitido como señales auxiliares, tamaño máximo de `5 * 1024 * 1024` bytes y magic bytes reales: PNG `89 50 4E 47`, JPEG `FF D8 FF`.
- **Razonamiento**: La extensión o el MIME declarado no prueban el contenido; la lectura de cabecera permite rechazar archivos incompatibles sin incorporar una librería de procesamiento de imágenes fuera del alcance.
- **Alternativas consideradas**: Confiar solo en extensión/MIME, rechazado por seguridad; agregar ImageSharp u otra librería, rechazado porque no se requiere conversión ni decodificación completa.

## Decisión 4: Nombre y almacenamiento de archivos

- **Decisión**: Generar un nombre con `Guid` y la extensión normalizada (`.png` o `.jpg`), escribir en la raíz runtime `wwwroot/assets/properties` que ya sirve static files y persistir solo `/assets/properties/{nombre}`.
- **Razonamiento**: Evita traversal, caracteres peligrosos y colisiones concurrentes. El nombre original solo se usa para determinar una extensión permitida, nunca como ruta física.
- **Alternativas consideradas**: Conservar el nombre original, rechazado por colisiones y traversal; usar hash de contenido, rechazado por complejidad innecesaria en este caso.

## Decisión 5: Persistencia y compensación

- **Decisión**: El handler valida primero, escribe el archivo con cancelación, crea `Property` con `ImageUrl` nullable y ejecuta `SaveChangesAsync`. Si falla persistencia, cancelación o una operación de almacenamiento después de crear el archivo, elimina el archivo creado y devuelve el error apropiado; los fallos inesperados se traducen a `Error.Internal` HTTP 500.
- **Razonamiento**: El sistema de archivos y PostgreSQL no comparten una transacción. La eliminación compensatoria evita archivos públicos huérfanos y la propiedad solo se agrega al contexto después de validar el archivo.
- **Alternativas consideradas**: Guardar primero la propiedad, rechazado porque deja referencias rotas si falla el archivo; usar una transacción distribuida, descartado por complejidad y falta de soporte para filesystem.

## Decisión 6: Response y status

- **Decisión**: Devolver HTTP 201 con `Results.Created`, `Location` `/api/properties/{id}` cuando corresponda, un contrato explícito y `status` textual (`Available`, `Rented`, `Maintenance`).
- **Razonamiento**: Alinea creación con el contrato persistente y evita exponer el valor numérico del enum. No se agrega un endpoint de detalle en esta iniciativa.
- **Alternativas consideradas**: HTTP 200, rechazado porque la operación crea un recurso; serializar el enum como número, rechazado por legibilidad e inconsistencia con la persistencia textual.

## Decisión 7: Compatibilidad del listado 004

- **Decisión**: Ajustar `ListPropertiesMapping` para que una `ImageUrl` nula produzca `PropertyListItem.ImageUrl = null` y solo marque como error interno las rutas no nulas inválidas.
- **Razonamiento**: La spec 006 define la ausencia de imagen como válida. El listado debe seguir devolviendo el elemento completo y no responder 500.
- **Alternativas consideradas**: Omitir el item o devolver 500, rechazado explícitamente por la aclaración aceptada.

## Decisión 8: Pruebas

- **Decisión**: Mantener pruebas unitarias xUnit con EF Core InMemory para handler/persistencia aislada, almacenamiento temporal para archivos y pruebas del mapping/validator. Cubrir PNG/JPG reales, límite exacto y excedido, contenido incompatible, ausencia de archivo, colisiones, cancelación y limpieza por fallo.
- **Razonamiento**: La constitución permite unit tests y la spec exige pruebas deterministas sin infraestructura externa. La migración se verificará por forma y nullable de la columna.
- **Alternativas consideradas**: Testcontainers para todo el flujo, rechazado por no estar exigido; pruebas manuales únicamente, rechazado porque no cubren limpieza ni cancelación de forma reproducible.
