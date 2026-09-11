# Investigación: Persistencia y Seeding de Propiedades

## Decisión: reutilizar `NetRentManagerApi`

La implementación se realizará en `app/backend/src/NetRentManagerApi`, que es el
proyecto real creado por las specs anteriores. La referencia del prompt a `NetRentManagerApi`
se trata como nombre genérico y no se creará una solución, proyecto o namespace
paralelo.

**Alternativas consideradas**: crear `NetRentManagerApi` se descartó por violar la solución
única y por duplicar el backend existente.

## Decisión: catálogo persistente y enum en la entidad

`PropertyStatus` será un enum cerrado con tres valores y tendrá catálogo persistente
para validar los datos fuente. `Property.Status` seguirá siendo enum en dominio y se
almacenará mediante conversión textual de EF Core.

**Razonamiento**: el dominio conserva seguridad de tipos y la base de datos queda
legible; el catálogo permite validar estados descritos por JSON antes de insertar.

**Alternativas consideradas**: guardar el estado como entero se descartó por
opacidad; usar strings directamente en la entidad se descartó por perder el conjunto
cerrado del dominio.

## Decisión: precisión y restricciones explícitas

El precio se almacenará como `numeric(18,2)` y el área como `numeric(10,2)`. Las
cadenas tendrán longitudes explícitas, las claves serán UUID, la creación tendrá
default UTC y `UpdatedAt` será nullable para representar el primer registro sin
actualización.

**Razonamiento**: evita precisión implícita, limita datos inválidos y deja el esquema
estable para consultas futuras.

## Decisión: una migración funcional única

Se generará `AddPropertyManagementEntities` después de validar entidades, `DbSet` y
configuraciones. Se revisarán `Up`, `Down` y snapshot antes de cerrar la tarea.

**Razonamiento**: cumple la regla de una migración por cambio funcional coherente y
permite revisar el esquema completo en una unidad.

## Decisión: seeding nativo EF Core e idempotente

`DatabaseSeeder.Seed` y `SeedAsync` compartirán la misma secuencia conceptual. El
registro del contexto usará `UseSeeding` y `UseAsyncSeeding`; `MigrationExtensions`
solo aplicará `Database.MigrateAsync()`, y `Program.cs` ejecutará `await
app.MigrateAsync()` antes de `app.Run()`.

La idempotencia se basará en los identificadores de estados y propiedades. La ruta
asíncrona propagará `CancellationToken` a lectura, consultas, guardado y copias de
archivos.

**Alternativas consideradas**: invocar el seeder manualmente después de migrar se
descartó porque duplica la ruta nativa; `HasData` se descartó porque el seed requiere
lectura de archivos, validación y sincronización de imágenes.

## Decisión: manifiesto como fuente única de rutas

El archivo `support/seed-data/seed-manifest.json` define fuentes, destino público y
prefijo de URL. El seeder resolverá todos los archivos desde ese contrato y rechazará
rutas ausentes, duplicadas o fuera del directorio permitido.

**Razonamiento**: evita rutas físicas dispersas y permite cambiar la distribución de
assets sin modificar la lógica de dominio.

## Decisión: assets públicos bajo `wwwroot/assets/properties`

Las imágenes fuente de `support/seed-data/images/properties` se copiarán al destino
`wwwroot/assets/properties`. `ImageUrl` persistirá únicamente `/assets/properties/<archivo>`.

**Razonamiento**: `wwwroot` es la ubicación pública estándar del proyecto web y la
URL queda desacoplada de rutas físicas del repositorio.

## Decisión: pruebas relacionales trazadas

Las pruebas de modelo y archivos serán unitarias. Las pruebas de `UseAsyncSeeding`,
`MigrateAsync` e idempotencia requerirán un proveedor relacional; si se usa
Testcontainers para PostgreSQL deberá existir una tarea explícita y el entorno deberá
estar disponible. SQLite no se usará para afirmar comportamiento específico de
PostgreSQL.

**Riesgo**: el entorno de ejecución puede no disponer de PostgreSQL o Docker. En ese
caso la tarea de pruebas debe reportar el bloqueo y no sustituir silenciosamente el
proveedor por una base no relacional.
