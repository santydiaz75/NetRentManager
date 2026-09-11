# Plan de Implementación: Listado Paginado de Propiedades

**Rama**: `004-properties-list-pagination` | **Fecha**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/004-properties-list-pagination/spec.md`

## Resumen

Agregar el caso de uso `GET /api/properties` al proyecto `NetRentManagerApi` como
un slice auto-descubrible. El handler consultará `Property` mediante una
proyección `AsNoTracking`, ordenará por título e identificador, calculará los
metadatos y convertirá la ruta de imagen persistida en una URL absoluta basada en
la solicitud actual. La validación global rechazará páginas inválidas y tamaños
fuera de `1..100`; la respuesta usará `items` y contratos explícitos.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y SDK `10.0.400` fijado en `global.json`.

**Dependencias principales**: ASP.NET Core Minimal APIs, EF Core 10 preview ya
usado por el repositorio, Npgsql, FluentValidation, `ISlice`, `IHandler`,
`Result<T>` y el mapper centralizado de `ProblemDetails`.

**Persistencia**: PostgreSQL mediante `AppDbContext`; la entidad `Property` y sus
assets públicos provienen de `003-properties-persistence-seeding`.

**Pruebas**: xUnit con las dependencias existentes. Se añadirán pruebas unitarias
del validator, handler, mapping, paginación y errores. No se agrega una prueba de
integración ni un proveedor nuevo sin una tarea explícita.

**Plataforma objetivo**: ASP.NET Core sobre PostgreSQL, ejecutado localmente y
publicado desde `app/backend/src/NetRentManagerApi`.

**Tipo de proyecto**: Web API Minimal API con arquitectura Vertical Slice.

**Objetivos de rendimiento**: contar y obtener solo la página solicitada; usar
proyección SQL, `AsNoTracking`, `Skip`/`Take` y cancelación en todas las llamadas
asíncronas de EF Core.

**Restricciones**: no controllers, no cambios al esquema persistente, no
repositories triviales, no mappers automáticos, no registro manual de slices,
handlers o validators, `pageSize` máximo 100, y no exponer rutas físicas ni
detalles internos en `ProblemDetails`.

**Escala/Alcance**: listado de propiedades existentes; una consulta devuelve como
máximo 100 items y no incorpora filtros, búsqueda, orden configurable ni cambios
de frontend.

## Investigación y decisiones

Las decisiones técnicas están consolidadas en [research.md](research.md):

- `OrderBy(Title).ThenBy(Id)` para orden determinista.
- Proyección directa con `AsNoTracking` y dos operaciones EF cancelables.
- Contratos locales `ListPropertiesRequest`, `PropertyListItem` y
  `PagedPropertiesResponse`.
- Construcción de URLs con `Request.Scheme`, `Request.Host` y el nombre de archivo.
- Extensión mínima de `ErrorType` y `ResultProblemDetailsMapper` para representar
  la inconsistencia interna de imagen como HTTP 500.
- `UseStaticFiles` para que `/assets/properties/{fileName}` sea servible.

## Comprobación de Constitución

*Gate: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec está en `specs/004-properties-list-pagination`, declara el
  estado canónico `Borrador` y no se modifica su estado durante la planificación.
- **Aprobado**: el cambio usa la solución existente, Minimal APIs, Vertical Slice,
  EF Core, PostgreSQL y FluentValidation.
- **Aprobado**: el endpoint vivirá en `Features/Properties/ListProperties` y se
  descubrirá por `ISlice`; no se creará controller, registry manual ni jerarquía
  paralela.
- **Aprobado**: el handler accede directamente a `AppDbContext` para una lectura,
  sin repository trivial y sin introducir MediatR.
- **Aprobado**: las entidades no se devuelven directamente; el mapping es explícito
  y local al slice.
- **Aprobado**: cada modificación de código, prueba, contrato y archivo HTTP
  quedará trazada en `tasks.md` antes de marcarse completa.
- **Aprobado**: `quickstart.md` define la evidencia requerida; la spec no podrá
  pasar a `Implementada` con tareas pendientes o sin evidencia.

No existen violaciones constitucionales que justificar.

## Estructura del proyecto

### Documentación de la funcionalidad

```text
specs/004-properties-list-pagination/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── http-contracts.md
├── checklists/
│   └── requirements.md
└── tasks.md                 # Lo generará /speckit.tasks
```

### Código fuente

```text
app/backend/src/NetRentManagerApi/
├── Features/Properties/ListProperties/
│   ├── ListPropertiesSlice.cs
│   ├── ListPropertiesRequest.cs
│   ├── ListPropertiesRequestValidator.cs
│   ├── ListPropertiesHandler.cs
│   ├── ListPropertiesMapping.cs
│   ├── PropertyListItem.cs
│   └── PagedPropertiesResponse.cs
├── Infrastructure/Errors/
│   ├── ErrorType.cs
│   └── ResultProblemDetailsMapper.cs
├── Program.cs                         # solo habilitar static files si se requiere
├── NetRentManagerApi.http
└── wwwroot/assets/properties/

app/backend/tests/NetRentManagerApiTests/
├── Features/Properties/ListProperties/
│   ├── ListPropertiesRequestValidatorTests.cs
│   ├── ListPropertiesHandlerTests.cs
│   ├── ListPropertiesMappingTests.cs
│   └── ListPropertiesErrorTests.cs
└── Infrastructure/Errors/
    └── ResultProblemDetailsMapperTests.cs
```

**Decisión estructural**: el slice contiene el request, validator, handler,
contratos y mapping porque no existe reutilización demostrada con otro caso de
uso. La infraestructura compartida solo se modifica para el tipo de error interno
y su traducción centralizada, y `Program.cs` únicamente habilita la entrega de
assets públicos si la configuración actual no la proporciona.

## Diseño técnico

### Flujo del endpoint

1. `SliceEndpointMappingExtensions` aplica el filtro global de validación.
2. `ListPropertiesSlice` registra `GET /api/properties` y recibe el request,
   `ListPropertiesHandler`, `HttpContext` y `CancellationToken`.
3. `ListPropertiesRequestValidator` valida `page >= 1` y `1 <= pageSize <= 100`.
4. `ListPropertiesHandler` ejecuta `CountAsync(cancellationToken)` sobre la query
   y `ToListAsync(cancellationToken)` sobre la proyección ordenada y paginada.
5. `ListPropertiesMapping` construye cada `imageUrl` absoluta usando el request.
6. El endpoint convierte `Result<PagedPropertiesResponse>` con
   `ResultProblemDetailsMapper` y devuelve `200 OK` o el `ProblemDetails` apropiado.

### Paginación

`totalPages` será cero cuando `totalItems` sea cero; en otro caso será el techo de
`totalItems / pageSize`. `hasNext` será `page < totalPages` y `hasPrevious` será
`page > 1 && totalItems > 0`. Una página posterior a la última no es un error.

### Imagen y error interno

El mapping aceptará únicamente una ruta persistida cuyo nombre de archivo pueda
extraerse sin traversal, sin `support` y sin ruta física. La URL final se construye
con `UriBuilder` o composición controlada de `Request.Scheme`, `Request.Host` y
`/assets/properties/{fileName}`. Si falla la validación, el handler devuelve un
`Error` de tipo interno con código estable; el mapper central lo traduce a HTTP 500
sin devolver el valor problemático.

### Static files y proxy

La tarea de composición verificará que la API sirva `wwwroot` y añadirá
`app.UseStaticFiles()` si no existe otra configuración equivalente. La URL se basa
en `Request.Scheme` y `Request.Host`; la confianza en encabezados reenviados y la
configuración de proxies queda fuera de esta spec y corresponde al hosting existente.

## Fases de implementación previstas

1. Crear contratos, request, validator y pruebas de validación.
2. Crear handler, mapping explícito, paginación y pruebas de datos/metadatos.
3. Integrar slice `ISlice` y verificar auto-descubrimiento sin tocar registradores.
4. Añadir el error interno HTTP 500 y probar su traducción a `ProblemDetails`.
5. Habilitar static files si es necesario y actualizar `NetRentManagerApi.http`.
6. Ejecutar build, tests y escenarios de `quickstart.md`; registrar evidencia.

## Validación posterior al diseño

*Gate: re-evaluado tras crear los artefactos de investigación y diseño.*

- **Aprobado**: [research.md](research.md) resuelve las decisiones de orden,
  consulta, mapping, URL, validación, error interno y pruebas.
- **Aprobado**: [data-model.md](data-model.md) define request, item, respuesta,
  metadatos y reglas de inconsistencia sin cambiar persistencia.
- **Aprobado**: [contracts/http-contracts.md](contracts/http-contracts.md) fija
  solicitudes, respuesta 200, 400 y 500.
- **Aprobado**: [quickstart.md](quickstart.md) contiene comandos y escenarios
  ejecutables, incluidos los dos requests manuales obligatorios.
- **Aprobado**: no quedan decisiones sin resolver en el plan ni en los artefactos
  de diseño.

## Complejidad

No hay violaciones constitucionales ni complejidad excepcional que justificar.
