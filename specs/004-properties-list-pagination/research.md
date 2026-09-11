# Investigación: Listado Paginado de Propiedades

## Decisión 1: Paginación y orden estable

- **Decisión**: Usar `page = 1`, `pageSize = 6`, validar `1 <= page` y `1 <= pageSize <= 100`, y ordenar por `Title` ascendente seguido de `Id` ascendente.
- **Razonamiento**: Los valores por defecto y el límite de 100 están fijados por la spec. `Id` aporta un desempate determinista para que una misma página no cambie cuando existen títulos iguales.
- **Alternativas consideradas**: Ordenar solo por título, rechazado porque no garantiza estabilidad; permitir cualquier tamaño positivo, rechazado porque puede producir consultas desproporcionadas.

## Decisión 2: Consulta de lectura

- **Decisión**: Ejecutar `CountAsync` sobre la consulta filtrada y luego una proyección `Select` con `AsNoTracking`, `OrderBy`, `ThenBy`, `Skip` y `Take`, materializada con `ToListAsync(cancellationToken)`.
- **Razonamiento**: Mantiene el acceso directo a `AppDbContext` permitido por la arquitectura, evita cargar entidades completas y propaga cancelación en las operaciones de EF Core.
- **Alternativas consideradas**: Cargar todas las entidades y paginar en memoria, rechazado por consumo innecesario; crear un repository, rechazado porque solo envolvería una consulta EF Core.

## Decisión 3: Contratos y mapping

- **Decisión**: Mantener `ListPropertiesRequest`, `PropertyListItem` y `PagedPropertiesResponse` dentro del slice, con mapping explícito. La respuesta usa la propiedad `items`.
- **Razonamiento**: Evita exponer `Property`, respeta Vertical Slice y fija el contrato JSON sin introducir un mapper automático.
- **Alternativas consideradas**: Reutilizar la entidad persistente, rechazado por mezclar dominio y contrato HTTP; crear `Shared`, rechazado porque todavía no existe reutilización entre dos slices.

## Decisión 4: URL absoluta y assets

- **Decisión**: Validar que `Property.ImageUrl` sea una ruta pública relativa con nombre de archivo, combinar `Request.Scheme`, `Request.Host` y `/assets/properties/{fileName}` durante el mapping, y habilitar static files para `wwwroot` antes de mapear los slices.
- **Razonamiento**: La persistencia de la spec 003 conserva rutas como `/assets/properties/1.png`, mientras el contrato HTTP exige una URL absoluta. `UseStaticFiles` hace que la URL generada sea servible por la propia API.
- **Alternativas consideradas**: Persistir la URL absoluta, rechazado porque acoplaría datos a un host; devolver la ruta relativa, rechazado explícitamente por la spec; usar una URL fija de configuración, rechazado porque el requisito exige el request actual.

## Decisión 5: Inconsistencia de imagen como error 500

- **Decisión**: Añadir `ErrorType.Internal` o equivalente dentro del modelo de errores compartido y mapearlo centralmente a `ProblemDetails` con HTTP 500 y código estable, sin exponer detalles internos. El handler devolverá `Result<PagedPropertiesResponse>` con ese error cuando una fila no pueda producir una URL pública válida.
- **Razonamiento**: La spec exige `500` y `ProblemDetails`; el mapper actual solo conoce 400, 403, 404 y 409. Extender el tipo y el mapper conserva el patrón `Result` y evita usar excepciones como control de flujo.
- **Alternativas consideradas**: Lanzar `InvalidOperationException`, rechazado porque mezcla un error esperado de integridad de datos con excepciones inesperadas; mapearlo como validación 400, rechazado porque el fallo pertenece al estado interno persistido, no a la entrada del cliente.

## Decisión 6: Validación y auto-descubrimiento

- **Decisión**: Crear un `AbstractValidator<ListPropertiesRequest>` junto al slice. El endpoint implementará `ISlice`, resolverá el handler por DI y no requerirá cambios en `Program.cs`, registradores ni filtros individuales.
- **Razonamiento**: La infraestructura existente escanea automáticamente slices, handlers y validators, y el filtro global ya devuelve `ValidationProblemDetails` 400.
- **Alternativas consideradas**: Validar manualmente dentro del endpoint, rechazado por duplicar la infraestructura; registrar tipos de forma manual, rechazado por romper el patrón de auto-descubrimiento.

## Decisión 7: Pruebas

- **Decisión**: Añadir pruebas unitarias del validator, handler/mapping, cálculo de metadatos y traducción del error 500; actualizar el archivo `.http` con las dos solicitudes manuales requeridas. Usar EF Core InMemory solo para el comportamiento de consulta aislado y mantener las pruebas específicas de PostgreSQL fuera de este alcance.
- **Razonamiento**: La constitución permite unit tests y la spec exige validar el caso de uso sin requerir una nueva prueba de integración. La consulta proyectada y la construcción de URL se pueden verificar con datos controlados.
- **Alternativas consideradas**: Testcontainers/PostgreSQL para todo el slice, rechazado porque aumentaría la infraestructura sin estar exigido por la spec; pruebas HTTP de extremo a extremo como única cobertura, rechazado porque ocultan la causa de errores de mapping y validación.
