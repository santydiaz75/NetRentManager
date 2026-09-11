# Investigación: Consulta de Propiedad por Id

## Decisión 1: Restricción de ruta para el identificador

- **Decisión**: Registrar la ruta como `GET /api/properties/{id:guid}`. Un segmento no convertible a `Guid` no coincide y devuelve 404; un GUID válido inexistente devuelve 404 mediante `Error.NotFound`.
- **Razonamiento**: Es consistente con la semántica pública de la spec y evita inventar un contrato 400 para un recurso cuya ruta no es válida.
- **Alternativas consideradas**: Recibir `string` y parsear en el handler, rechazado porque duplica routing y mezcla validación de ruta con lógica de consulta; devolver 400, rechazado por la aclaración aceptada.

## Decisión 2: Contrato unitario compatible con 004

- **Decisión**: Crear `GetPropertyByIdResponse` con los mismos nueve campos públicos de `PropertyListItem` y sin `PagedPropertiesResponse`.
- **Razonamiento**: Permite que clientes reutilicen sus modelos públicos sin recibir metadatos irrelevantes de paginación.
- **Alternativas consideradas**: Devolver `Property`, rechazado por exponer entidades EF; reutilizar el wrapper paginado, rechazado porque agrega `items` y metadatos fuera del contrato.

## Decisión 3: Consulta proyectada sin seguimiento

- **Decisión**: Consultar `AppDbContext.Properties.AsNoTracking()` mediante una proyección interna y `SingleOrDefaultAsync` con `CancellationToken`.
- **Razonamiento**: La operación es de solo lectura y necesita únicamente los campos públicos; evita materializar y rastrear una entidad completa.
- **Alternativas consideradas**: `FindAsync` seguido de mapping, rechazado porque rastrea la entidad y carga más datos; reutilizar el listado con `pageSize=1`, rechazado por semántica y contrato incorrectos.

## Decisión 4: Construcción y validación de imageUrl

- **Decisión**: Reutilizar el criterio de 004: una `ImageUrl` nula produce `null`; una ruta no nula debe comenzar bajo `/assets/properties/`, tener un nombre seguro y convertirse en URL absoluta con esquema y host de la request.
- **Razonamiento**: Conserva la interoperabilidad externa y evita exponer filesystem, `support`, traversal o URLs persistidas no controladas.
- **Alternativas consideradas**: devolver la URL relativa persistida, rechazado porque clientes externos no pueden resolverla; aceptar URI absoluta persistida, rechazado porque permitiría host o esquema controlados por datos.

## Decisión 5: Error y cancelación

- **Decisión**: Usar `Error.NotFound` para ausencia, `Error.Internal` para imagen inconsistente y `ResultProblemDetailsMapper` para ProblemDetails. Propagar cancelación sin convertirla en error de negocio.
- **Razonamiento**: Mantiene el contrato común de las specs anteriores y evita filtrar detalles internos.
- **Alternativas consideradas**: respuesta manual en el slice, rechazada por duplicación; devolver `imageUrl` omitida, rechazada porque produciría respuesta parcial.

## Decisión 6: Pruebas y rendimiento

- **Decisión**: Usar xUnit, EF Core InMemory, `DefaultHttpContext` con `http`/`https`, hosts con puerto y pruebas focalizadas de mapping, handler, slice, errores y cancelación. Medir p95 sobre 100 consultas válidas.
- **Razonamiento**: Cubre el comportamiento de URL y cancelación sin infraestructura externa, y verifica el objetivo de lectura ligera.
- **Alternativas consideradas**: pruebas de integración externas obligatorias, rechazadas porque no están aprobadas; medir solo el listado, rechazado porque no prueba el endpoint unitario.
