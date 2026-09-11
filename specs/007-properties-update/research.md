# Investigación: Actualización de Propiedades

## Decisión 1: Reutilizar el modelo persistente existente

- **Decisión**: Reutilizar `Property`, `ImageUrl` nullable, la configuración existente y la migración de `006`; no crear migración ni columna de concurrencia.
- **Razonamiento**: La actualización modifica una entidad ya persistida y la spec exige compatibilidad con las iniciativas anteriores. `UpdatedAt` ya existe y puede registrar la modificación.
- **Alternativas consideradas**: Crear una entidad de edición separada, rechazado porque duplicaría el modelo; agregar `rowVersion`, rechazado por la decisión explícita de última escritura válida sin HTTP 409.

## Decisión 2: Contrato multipart y reemplazo completo

- **Decisión**: Usar request `[FromForm]` con todos los campos de negocio obligatorios y `IFormFile? image` opcional.
- **Razonamiento**: `PUT` queda definido como reemplazo completo; la ausencia de imagen tiene semántica especial de conservación de `ImageUrl`.
- **Alternativas consideradas**: PATCH parcial, rechazado porque no es el contrato aprobado; `IFormCollection`, rechazado porque desplaza el binding y la validación a parsing manual.

## Decisión 3: Orden de búsqueda y validación

- **Decisión**: Resolver la propiedad por id antes de cualquier escritura de archivo; validar campos y upload antes de mutar la entidad tracked o guardar cambios.
- **Razonamiento**: El id inexistente debe devolver 404 sin efectos y los errores de entrada deben conservar el estado anterior.
- **Alternativas consideradas**: Escribir primero la imagen, rechazado porque puede crear I/O innecesario para un id inexistente; mutar y luego validar, rechazado por riesgo de estado tracked parcial.

## Decisión 4: Validación y códigos 400/413/415

- **Decisión**: Mantener FluentValidation para campos de negocio y reglas simples del archivo; el handler verificará bytes reales. Se agregarán `UnsupportedMediaType` y `PayloadTooLarge` a `ErrorType` y a `ResultProblemDetailsMapper`.
- **Razonamiento**: El mapper actual solo conoce validación 400, not found y errores internos. Tipar 413/415 mantiene la semántica común y evita respuestas manuales en el slice.
- **Alternativas consideradas**: Devolver 400 para todo, rechazado por la spec; devolver `Results.Problem` directamente desde el endpoint, rechazado porque duplica la traducción de errores.

## Decisión 5: Staging y compensación de imágenes

- **Decisión**: Generar `Guid` con extensión normalizada, escribir con `CreateNew` y conservar la URL anterior. Si falla antes del commit, eliminar el archivo nuevo y restaurar la entidad tracked.
- **Razonamiento**: PostgreSQL y filesystem no comparten transacción; el archivo nuevo debe ser el único efecto compensable de la operación.
- **Alternativas consideradas**: Eliminar la imagen anterior primero, rechazado porque puede dejar una URL rota; transacción distribuida, descartada por complejidad y falta de soporte para filesystem.

## Decisión 6: Limpieza posterior al commit

- **Decisión**: Después de `SaveChangesAsync`, intentar eliminar la imagen anterior solo cuando la ruta pertenece al destino administrado. Si falla, registrar logging estructurado y mantener HTTP 200.
- **Razonamiento**: La propiedad ya apunta a una imagen nueva válida; revertir por un fallo de limpieza sería menos consistente que conservar la nueva referencia y reparar el huérfano después.
- **Alternativas consideradas**: Revertir la base de datos, rechazado porque puede crear una referencia rota si ya se publicó la nueva imagen; no limpiar nunca, rechazado por acumulación innecesaria.

## Decisión 7: Concurrencia

- **Decisión**: Aplicar última escritura válida confirmada, sin bloqueo ni control optimista.
- **Razonamiento**: Es la decisión aceptada y conserva el modelo actual sin migración, token de versión ni contrato 409.
- **Alternativas consideradas**: `rowVersion`/`updatedAt` con 409, rechazado por alcance; bloqueo durante I/O, rechazado por mantener recursos ocupados durante uploads.

## Decisión 8: Pruebas y rendimiento

- **Decisión**: Usar xUnit, EF Core InMemory, directorios temporales y dobles para I/O/persistencia. Medir p95 sobre 100 solicitudes funcionales y documentar el entorno.
- **Razonamiento**: Permite comprobar rollback, cancelación, colisiones y errores de status de forma determinista sin infraestructura externa no aprobada.
- **Alternativas consideradas**: Testcontainers para todas las pruebas, rechazado porque no está exigido; medición de producción, rechazada porque excede el objetivo funcional.
