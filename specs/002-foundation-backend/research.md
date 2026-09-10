# Investigación: Fundación Interna del Backend

## Decisión: assembly scanning explícito e idempotente

`RegisterSlices` y `RegisterHandlers` recibirán el `Assembly` a inspeccionar. Se
filtrarán clases públicas, concretas y no abstractas mediante `IsAssignableFrom`.
Los registros usarán `TryAddEnumerable` para que repetir la llamada no agregue
implementaciones duplicadas.

**Razonamiento**: limita el alcance del descubrimiento, evita registrar tipos de
assemblies no autorizados y satisface el requisito de no usar registries manuales ni
convenciones basadas en nombres.

**Alternativas consideradas**: escanear todos los assemblies cargados se descartó
por impredecible; registrar tipos explícitamente en `Program.cs` se descartó por
acoplamiento; usar MediatR o Scrutor se descartó porque la spec no lo requiere.

## Decisión: slices stateless y handlers scoped

`ISlice` se registrará como singleton porque solo define mapeo y no conserva estado
de request. Los handlers futuros se registrarán como scoped para permitir dependencias
de petición sin capturarlas durante el arranque.

**Razonamiento**: respeta los lifetimes indicados por las guías locales y separa la
composición de endpoints de la ejecución de casos de uso.

**Alternativas consideradas**: hacer todos los slices scoped añade resolución por
request sin necesidad; singleton para handlers puede capturar servicios scoped.

## Decisión: registro oficial de FluentValidation

Se usará `AddValidatorsFromAssembly` con lifetime scoped. La
`ValidationFilterFactory` localizará `IValidator<T>` mediante la firma del handler,
`IServiceProviderIsService` y el scope de la petición.

**Razonamiento**: usa el mecanismo oficial, evita scanners duplicados y permite
pass-through cuando no existe validator.

**Alternativas consideradas**: validar dentro de cada endpoint se descartó por
repetición; un middleware global se descartó porque no conoce de forma fiable el tipo
concreto enlazado al parámetro del handler.

## Decisión: errores esperados como Result y ProblemDetails

`Result<T>` y `Error` serán contratos independientes de ASP.NET Core. Un mapper
centralizará el status code y el payload HTTP. `ValidationProblemDetails` se reservará
para errores de entrada y `ProblemDetails` para otros errores esperados.

**Razonamiento**: mantiene una frontera clara entre aplicación y transporte y evita
excepciones como control de flujo.

**Alternativas consideradas**: lanzar excepciones para 404 o 409 se descartó por
mezclar errores esperados con fallos inesperados; respuestas anónimas se descartaron
por falta de contrato estable.

## Decisión: unit tests sin host real

Se probarán scanners con `ServiceCollection`, filtros con contextos de invocación y
resultados HTTP con `DefaultHttpContext` en memoria. `/health` se verificará mediante
el registro de la ruta y la ejecución de su resultado, sin `WebApplicationFactory`.

**Razonamiento**: la constitución permite por defecto solo unit tests y la spec no
aprueba pruebas de integración. La estrategia cubre el comportamiento observable sin
crear una dependencia de host de pruebas.

**Alternativas consideradas**: `WebApplicationFactory` daría una prueba HTTP más
completa, pero sería integración y requeriría aprobación explícita en `tasks.md`.

## Decisión: no persistencia ni entidades

No se agregan EF Core, Npgsql, `AppDbContext`, migraciones, seeders ni conexión de
base de datos en esta iniciativa.

**Razonamiento**: la spec define una fundación transversal previa a cualquier feature
de producto y excluye explícitamente persistencia.
