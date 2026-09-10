# Plan de Implementación: Fundación Interna del Backend

**Rama**: `002-foundation-backend` | **Fecha**: 2026-09-10 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/002-foundation-backend/spec.md`

## Resumen

Crear la infraestructura transversal mínima del backend existente para que endpoints
Minimal API, handlers y validators se descubran mediante assembly scanning, sin
registros manuales en `Program.cs`. La implementación usará contratos explícitos,
reflection limitada al assembly recibido, DI de ASP.NET Core, FluentValidation y
ProblemDetails. El endpoint `/health` será el único slice funcional de esta spec.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10; `global.json` fija SDK `10.0.400`.

**Dependencias principales**: ASP.NET Core Minimal APIs, FluentValidation,
Microsoft.AspNetCore.OpenApi y las abstracciones DI integradas de .NET.

**Persistencia**: N/A para esta spec; no se crea `AppDbContext`, conexión ni migración.

**Pruebas**: xUnit como unit tests; `DefaultHttpContext` y builders en memoria para
verificar resultados HTTP sin `WebApplicationFactory`.

**Plataforma objetivo**: ASP.NET Core sobre el runtime definido por `global.json`.

**Tipo de proyecto**: Web API Minimal API dentro de una solución fullstack existente.

**Objetivos de rendimiento**: el descubrimiento ocurre al iniciar; la validación se
ejecuta una vez por request validable y no debe escanear assemblies por request.

**Restricciones**: no controllers, no persistencia, no features de negocio, no
modificación de `global.json`, no registros individuales y solo unit tests.

**Escala/Alcance**: un proyecto API existente, un assembly de producción escaneado,
un assembly de tests usado para demostrar descubrimiento y cinco grupos de pruebas.

## Comprobación de Constitución

*Gate: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec está en `specs/`, mantiene el flujo Spec-Driven y no crea
  una solución paralela.
- **Aprobado**: el backend usa Minimal APIs, Vertical Slice Architecture y deja
  `Program.cs` como composición.
- **Aprobado**: no se introducen entidades, persistencia ni controllers.
- **Aprobado**: las pruebas son unitarias; no se autoriza `WebApplicationFactory` ni
  Testcontainers porque no son necesarios para el alcance aprobado.
- **Aprobado**: los documentos generados están en español y cada cambio futuro
  deberá trazarse a `tasks.md`.
- **Aprobado**: el cierre deberá conservar evidencia en `quickstart.md` y no podrá
  cambiar la spec a `Implementada` mientras existan tareas pendientes.

- La spec declara un estado canónico válido y el plan respeta la transición
  permitida para la fase actual.
- El plan incluye validación de `tasks.md` y evidencia en `quickstart.md` antes
  de permitir el estado `Implementada`.

## Estructura del proyecto

### Documentación de esta funcionalidad

```text
specs/002-foundation-backend/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md             # Lo generará /speckit.tasks
```

### Código fuente

```text
app/backend/src/NetRentManagerApi/
├── Infrastructure/
│   ├── Endpoints/
│   ├── Errors/
│   ├── Handlers/
│   └── Validation/
├── Features/                         # reservado; sin features en esta spec
└── Program.cs

app/backend/tests/NetRentManagerApiTests/
├── Infrastructure/
│   ├── Endpoints/
│   ├── Errors/
│   └── Validation/
└── TestTypes/                        # slices, handlers y validators de prueba
```

**Decisión estructural**: se conserva la solución existente y se agrega una
organización transversal dentro de `Infrastructure`. Las futuras features vivirán
en `Features/<área>/<caso-de-uso>`; esta spec no crea ninguna. Los tests se agrupan
por capacidad transversal y sus tipos auxiliares quedan exclusivamente en el
proyecto de pruebas.

## Diseño técnico

### Descubrimiento y DI

- `ISlice` expone `void AddEndpoint(IEndpointRouteBuilder app)`.
- `RegisterSlices(IServiceCollection, Assembly)` inspecciona únicamente el assembly
  recibido, filtra clases públicas, concretas y no abstractas, y registra cada tipo
  como `Singleton<ISlice>` mediante `TryAddEnumerable`.
- `MapSliceEndpoints(IEndpointRouteBuilder)` crea un grupo, aplica el filtro global,
  resuelve `IEnumerable<ISlice>` y llama a cada `AddEndpoint`.
- `IHandler` es marker interface; `RegisterHandlers` registra implementaciones
  concretas como scoped con `TryAddEnumerable`, sin handlers de negocio.
- `AddValidatorsFromAssembly` registra validators scoped desde el assembly indicado.

### Validación

`ValidationFilterFactory` inspecciona la firma del handler, excluye parámetros de
infraestructura y comprueba la existencia de `IValidator<T>` con
`IServiceProviderIsService`. Si existe, resuelve el validator desde el scope de la
petición y ejecuta `ValidateAsync` con el cancellation token. Un resultado inválido
devuelve `ValidationProblemDetails` HTTP 400 agrupado por propiedad; sin validator
se ejecuta el delegate sin error.

### Errores

`Result<T>` y `Error` permanecen independientes de HTTP. Un mapper centralizado
convierte `NotFound` a 404, `Conflict` a 409, `Validation` a 400 y `Forbidden` a
403 mediante `ProblemDetails`; los detalles opcionales ausentes producen un payload
válido. `AddProblemDetails` cubre errores HTTP inesperados sin exponer excepciones.

### Salud y composición

`HealthSlice` registra `GET /health` y devuelve un resultado estable de salud sin
base de datos. `Program.cs` solo registra OpenAPI, ProblemDetails, scanners y el
mapeo centralizado; no registra slices ni endpoints individuales.

## Estrategia de pruebas

Las pruebas usarán xUnit y `ServiceCollection` para verificar scanners e idempotencia.
La factory de validación se ejercerá con contextos de invocación y delegates falsos;
los resultados se ejecutarán sobre `DefaultHttpContext` y se deserializarán como
ProblemDetails estructurados. La salud verificará la ruta registrada y el resultado
HTTP del slice en memoria. No se levanta un host real ni se usa base de datos.

## Seguimiento de complejidad

No existen violaciones constitucionales que justificar.

| Violación | Necesidad | Alternativa simple descartada |
|-----------|-----------|-------------------------------|
| N/A | N/A | N/A |
