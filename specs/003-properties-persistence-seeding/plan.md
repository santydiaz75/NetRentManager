# Plan de Implementación: Persistencia y Seeding de Propiedades

**Rama**: `003-properties-persistence-seeding` | **Fecha**: 2026-09-10 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/003-properties-persistence-seeding/spec.md`

## Resumen

Agregar el modelo persistente inicial de propiedades al proyecto existente
`NetRentManagerApi`, con configuraciones EF Core separadas, una única migración y
seeding idempotente disparado exclusivamente por `UseSeeding` y `UseAsyncSeeding`.
Los JSON y las imágenes se distribuirán mediante el manifiesto existente y las
imágenes se copiarán a `wwwroot/assets/properties`, mientras `ImageUrl` conservará
la ruta pública `/assets/properties/<archivo>`.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y SDK `10.0.400` fijado en `global.json`.

**Dependencias principales**: EF Core 10, Npgsql para PostgreSQL, ASP.NET Core
Minimal APIs, FluentValidation y las extensiones de DI ya usadas por la spec 002.
Las versiones de EF Core y Npgsql deben mantenerse alineadas con el target `net10.0`.

**Persistencia**: PostgreSQL mediante Npgsql; los datos fuente son JSON e imágenes
distribuidos como contenido del proyecto API.

**Pruebas**: xUnit; pruebas de modelo y migración sin servidor, y pruebas de seeding
con un proveedor relacional de prueba autorizado por la tarea. No se usa SQLite
cuando la prueba depende de comportamiento PostgreSQL; Testcontainers solo se usará
si queda disponible y trazado explícitamente en tasks.

**Plataforma objetivo**: ASP.NET Core sobre PostgreSQL, con runtime y publish del
proyecto `app/backend/src/NetRentManagerApi`.

**Tipo de proyecto**: Web API Minimal API con persistencia EF Core y slices futuros
organizados por feature.

**Objetivos de rendimiento**: el escaneo de archivos ocurre durante el seeding y no
por request; una ejecución repetida debe consultar por claves antes de insertar y
evitar copias de imagen innecesarias.

**Restricciones**: una sola migración, sin `HasData`, sin controllers, sin endpoints
de negocio en esta spec, sin seeder manual después de migrar, sin alterar
`HealthSlice`, sin modificar `global.json` y sin `dotnet ef database update` como
criterio de cierre.

**Escala/Alcance**: dos entidades persistentes, tres estados, diez propiedades,
diez imágenes y un manifiesto de seed version `1.0.0`.

## Comprobación de Constitución

*Gate: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec 003 es posterior a las specs 001 y 002 y extiende la misma
  solución sin crear un proyecto paralelo.
- **Aprobado**: las entidades y configuraciones siguen EF Core + Npgsql + PostgreSQL
  y las migraciones se mantienen bajo `Infrastructure/Persistence`.
- **Aprobado**: cada cambio persistente tendrá una tarea explícita; la migración se
  generará solo después de validar entidades, `DbSet` y configuraciones.
- **Aprobado**: `UseSeeding` y `UseAsyncSeeding` son los únicos puntos de entrada del
  seeder; `app.MigrateAsync()` se ejecuta antes de `app.Run()`.
- **Aprobado**: no se usan controllers, `HasData`, SQL manual ni endpoints de negocio
  fuera de `Features/Properties`.
- **Aprobado**: los documentos están en español, la spec declara estado canónico y
  el cierre exigirá tareas completas y evidencia en `quickstart.md`.

- La spec declara un estado canónico válido y el plan respeta la transición
  permitida para la fase actual.
- El plan incluye validación de `tasks.md` y evidencia en `quickstart.md` antes
  de permitir el estado `Implementada`.

## Estructura del proyecto

### Documentación de esta funcionalidad

```text
specs/003-properties-persistence-seeding/
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
├── Domain/Properties/
│   ├── Property.cs
│   └── PropertyStatus.cs
├── Features/Properties/              # reservado; sin endpoints en esta spec
├── Infrastructure/Persistence/
│   ├── AppDbContext.cs
│   ├── DatabaseSeeder.cs
│   ├── MigrationExtensions.cs
│   ├── Configurations/
│   │   ├── PropertyConfiguration.cs
│   │   └── PropertyStatusConfiguration.cs
│   └── Migrations/
├── wwwroot/assets/properties/         # destino público de imágenes
├── Program.cs
└── NetRentManagerApi.csproj

app/backend/tests/NetRentManagerApiTests/
├── Infrastructure/Persistence/
├── Domain/Properties/
└── TestData/

support/seed-data/
├── properties.json
├── properties-statuses.json
├── seed-manifest.json
└── images/properties/
```

**Decisión estructural**: el código persistente se incorpora al proyecto existente
`NetRentManagerApi`. `Domain/Properties` contiene solo entidades; persistencia,
configuraciones, migración y seeding viven bajo `Infrastructure/Persistence`.
Los endpoints de negocio quedan reservados para `Features/Properties` y no se crean.
No se crea `Shared` porque esta spec no demuestra reutilización entre dos slices.

## Diseño técnico

### Modelo y esquema

`PropertyStatus` será un enum cerrado con `Available`, `Rented` y `Maintenance`.
`Property` usará `Guid Id`, strings requeridos para título, descripción, dirección e
imagen, `decimal Price`, contadores enteros, `decimal AreaSquareMeters`, el enum de
estado y `DateTimeOffset` para `CreatedAt` y `UpdatedAt` nullable. La configuración
usará tablas `property_statuses` y `properties`, claves UUID, longitudes explícitas,
precisión monetaria `18,2`, precisión de área `10,2`, conversión string del enum,
índice único del código de estado y defaults UTC para creación.

Los estados se modelarán como catálogo persistente para que el seeder pueda validar
el JSON contra el conjunto permitido; `Property.Status` conservará el enum y la
relación se expresará mediante el valor textual almacenado según el contrato aprobado.

### Migración

La migración se llamará `AddPropertyManagementEntities` y será la única migración
creada por esta spec. Se generará después de compilar el modelo, con `dotnet ef
migrations add` y salida en `Infrastructure/Persistence/Migrations`. Se revisarán
`Up`, `Down` y el snapshot; cualquier operación destructiva detendrá el cierre.

### Seeding y arranque

`AppDbContext` aplicará `ApplyConfigurationsFromAssembly`. Su registro configurará
Npgsql, `UseSeeding` y `UseAsyncSeeding`, ambos delegando en `DatabaseSeeder` con la
misma lógica. `MigrationExtensions` solo aplicará `Database.MigrateAsync()` para
PostgreSQL; no invocará el seeder. `Program.cs` ejecutará `await app.MigrateAsync()`
antes de `app.Run()`.

`DatabaseSeeder` leerá el manifiesto, validará rutas y estados, copiará imágenes a
`wwwroot/assets/properties`, y usará el `Id` de cada propiedad como clave de
idempotencia. Estados y propiedades existentes se conservarán; los nuevos se
insertarán en una transacción lógica. La versión síncrona y asíncrona mantendrán el
mismo algoritmo, propagando `CancellationToken` en la ruta asíncrona.

### Assets y contrato público

El manifiesto existente es la única fuente de rutas. El proyecto incluirá JSON y
manifiesto como `None`/`Content` copiable a output y publish, y las imágenes de
`support/seed-data/images/properties` se copiarán a `wwwroot/assets/properties`.
El nombre de archivo del JSON se transforma en `ImageUrl` como
`/assets/properties/<archivo>`. El seeder rechazará rutas fuera de las fuentes
declaradas, archivos ausentes, duplicados de manifiesto y referencias a `support` en
la URL persistida.

### Pruebas

Se crearán unit tests para configuraciones, conversión enum-string, snapshot y
estructura de migración, lectura del manifiesto, sincronización de imágenes e
idempotencia. Para probar `UseAsyncSeeding` y `MigrateAsync` se usará una base
relacional de prueba solo si el proyecto puede disponer de PostgreSQL de prueba; si
requiere Testcontainers, la tarea lo autorizará explícitamente. No se usará SQLite
para afirmar comportamiento relacional PostgreSQL y no se usará `dotnet ef database
update` como criterio de cierre.

## Seguimiento de complejidad

No existen violaciones constitucionales que justificar.

| Violación | Necesidad | Alternativa simple descartada |
|-----------|-----------|-------------------------------|
| N/A | N/A | N/A |
