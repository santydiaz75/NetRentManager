# Plan de Implementación: Consulta de Propiedad por Id

**Rama**: `008-properties-get-by-id` | **Fecha**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/008-properties-get-by-id/spec.md`

## Resumen

Agregar `GET /api/properties/{id}` como slice Vertical Slice para consultar una
propiedad individual. El handler buscará una proyección `AsNoTracking` por
`Guid`, devolverá 404 cuando no exista y delegará al mapping la construcción del
contrato público. Cuando `ImageUrl` sea válida, el mapping construirá una URL
absoluta con `Request.Scheme`, `Request.Host` y `/assets/properties/{fileName}`;
cuando sea `null`, conservará `imageUrl: null`. Una referencia persistida insegura
producirá HTTP 500 mediante `ProblemDetails`.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y SDK `10.0.400` fijado en `global.json`.

**Dependencias principales**: ASP.NET Core Minimal APIs, EF Core 10, Npgsql,
FluentValidation, `ISlice`, `IHandler`, `Result<T>` y
`ResultProblemDetailsMapper` existentes.

**Persistencia**: PostgreSQL mediante `AppDbContext`; lectura proyectada y sin
seguimiento desde `Properties`.

**Imágenes**: se reutiliza la ruta pública `/assets/properties/` y la validación
existente del mapping de 004. No se modifican archivos, entidades, migraciones ni
seeding.

**Pruebas**: xUnit, EF Core InMemory, `DefaultHttpContext` con esquemas/hosts
variables y pruebas de validator, handler, mapping, slice, errores y cancelación.

**Plataforma objetivo**: API ASP.NET Core ejecutable desde
`app/backend/src/NetRentManagerApi`.

**Tipo de proyecto**: Web API Minimal API organizada por features con Vertical Slice.

**Objetivos de rendimiento**: p95 inferior a 250 ms en 100 consultas válidas;
seleccionar una sola proyección con `AsNoTracking` y propagar cancelación a EF Core.

**Restricciones**: sin paginación, filtros, cambios de persistencia, frontend,
controllers, registros manuales, entidades EF en respuestas ni URLs relativas.

**Escala/Alcance**: una propiedad por solicitud y una respuesta JSON con nueve
campos públicos, sin metadatos de listado.

## Comprobación de Constitución

*Gate: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec está en `specs/008-properties-get-by-id`, declara `Borrador`
  y el plan no modifica su estado.
- **Aprobado**: se mantienen .NET 10, Minimal APIs, EF Core, PostgreSQL,
  FluentValidation y ProblemDetails.
- **Aprobado**: el caso de uso vive en `Features/Properties/GetPropertyById` y se
  registra por auto-descubrimiento de `ISlice`, `IHandler` y validators.
- **Aprobado**: el handler consulta; el endpoint delega; el mapping transforma y
  construye la URL pública sin acceso a datos.
- **Aprobado**: la respuesta reutiliza los campos de 004 sin copiar metadatos de
  paginación ni exponer `Property` directamente.
- **Aprobado**: no se modifica el esquema persistente ni se crea migración.
- **Aprobado**: `quickstart.md` define evidencia y la spec no podrá cerrarse con
  tareas pendientes o sin validación registrada.

No existen violaciones constitucionales que justificar.

## Investigación y decisiones

Las decisiones están consolidadas en [research.md](research.md):

- usar `Guid` como restricción de ruta; tanto formato inválido como recurso ausente
  terminan en 404;
- reutilizar los nueve campos públicos de `PropertyListItem` de 004 en un contrato
  unitario separado;
- proyectar con `AsNoTracking` y `SingleOrDefaultAsync` cancelable;
- construir la URL absoluta desde la request actual y validar el nombre de archivo;
- conservar `imageUrl: null` para ausencia válida y devolver 500 para inconsistencia;
- mantener el error mediante el mapper común, sin respuestas manuales.

## Estructura del proyecto

### Documentación de la funcionalidad

```text
specs/008-properties-get-by-id/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── backend-properties-get-by-id.openapi.yaml
├── checklists/
│   └── requirements.md
└── tasks.md                 # Lo generará /speckit.tasks
```

### Código fuente

```text
app/backend/src/NetRentManagerApi/
├── Features/Properties/GetPropertyById/
│   ├── GetPropertyByIdSlice.cs
│   ├── GetPropertyByIdRequest.cs
│   ├── GetPropertyByIdRequestValidator.cs
│   ├── GetPropertyByIdHandler.cs
│   ├── GetPropertyByIdMapping.cs
│   └── GetPropertyByIdResponse.cs
├── Infrastructure/Errors/
│   └── ResultProblemDetailsMapper.cs
└── NetRentManagerApi.http

app/backend/tests/NetRentManagerApiTests/
├── Features/Properties/GetPropertyById/
│   ├── GetPropertyByIdRequestValidatorTests.cs
│   ├── GetPropertyByIdHandlerTests.cs
│   ├── GetPropertyByIdMappingTests.cs
│   ├── GetPropertyByIdSliceTests.cs
│   └── GetPropertyByIdErrorTests.cs
└── Infrastructure/Errors/
    └── ResultProblemDetailsMapperTests.cs
```

**Decisión estructural**: el slice contiene sus requests, response, validator,
handler y mapping. La infraestructura compartida solo se reutiliza; no se crean
registradores, migraciones ni adaptadores específicos fuera del caso de uso.

## Diseño técnico

### Flujo de consulta

1. `GetPropertyByIdSlice` registra `GET /api/properties/{id:guid}` y declara
   respuestas 200, 404 y 500.
2. El endpoint enlaza el `Guid`, obtiene el handler auto-registrado, pasa
   `HttpRequest` y `CancellationToken`, y convierte el `Result` con el mapper común.
3. `GetPropertyByIdHandler` ejecuta una proyección `AsNoTracking` con
   `SingleOrDefaultAsync(id, cancellationToken)`.
4. Si no hay fila, devuelve `Error.NotFound`; si existe, llama al mapping explícito.
5. `GetPropertyByIdMapping` copia los nueve campos públicos y convierte `status` a
   texto. Si `ImageUrl` es `null`, devuelve `imageUrl: null`.
6. Para una imagen no nula, valida ruta pública, nombre de archivo, ausencia de
   traversal, `support`, URI absoluta y ruta física; después construye la URL con
   esquema y host de la request.

### Contrato de errores

- Segmento no GUID: HTTP 404 por restricción de ruta.
- GUID sin entidad: HTTP 404 con `ProblemDetails` y código estable.
- Imagen persistida inconsistente: HTTP 500 con `ProblemDetails`, sin ruta ni stack.
- Cancelación: propagar `OperationCanceledException` y el token hasta EF Core.

### Compatibilidad con 004

El response unitario comparte los nombres y tipos de `PropertyListItem`: `id`,
`title`, `description`, `address`, `price`, `status`, `bedroomCount`,
`bathroomCount`, `areaSquareMeters` e `imageUrl`. No incluye `items` ni ningún
metadato de `PagedPropertiesResponse`.

## Fases de implementación previstas

1. Crear contratos, request, validator y pruebas de validación/ruta.
2. Crear proyección, handler, mapping y pruebas de 200/404/500.
3. Registrar el slice, auto-descubrimiento y metadatos OpenAPI.
4. Añadir ejemplos HTTP para imagen, ausencia de imagen, id inexistente e inválido.
5. Ejecutar build, tests, validación manual y medición p95; registrar evidencia.

## Validación posterior al diseño

*Gate: re-evaluado tras crear los artefactos de investigación y diseño.*

- **Aprobado**: [research.md](research.md) resuelve ruta, proyección, mapping,
  URL absoluta, errores, cancelación y pruebas.
- **Aprobado**: [data-model.md](data-model.md) define la proyección y el response
  sin cambiar persistencia.
- **Aprobado**: [backend-properties-get-by-id.openapi.yaml](contracts/backend-properties-get-by-id.openapi.yaml)
  fija ruta, parámetros, response 200 y errores 404/500.
- **Aprobado**: [quickstart.md](quickstart.md) contiene escenarios manuales y p95.
- **Aprobado**: no quedan decisiones `NEEDS CLARIFICATION` ni placeholders.

## Complejidad

No hay violaciones constitucionales ni complejidad excepcional que justificar.
