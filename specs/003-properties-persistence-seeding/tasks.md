---

description: "Lista de tareas para persistencia y seeding de propiedades"
---

# Tareas: Persistencia y Seeding de Propiedades

**Entrada**: Documentos de diseño de `specs/003-properties-persistence-seeding/`

**Prerequisitos**: `spec.md`, `plan.md`, `research.md`, `data-model.md`,
`contracts/persistence-contracts.md`, `quickstart.md`

**Pruebas**: Se incluyen pruebas de modelo, migración, seeding y assets porque la
spec las exige explícitamente. Las pruebas que requieran PostgreSQL o un proveedor
relacional deben estar trazadas y no pueden sustituirse silenciosamente por SQLite.

**Regla de trazabilidad**: Cada entidad, configuración, migración, seeder, asset,
archivo de soporte, cambio de arranque y validación debe corresponder a una tarea de
este archivo antes de marcarse como completado.

## Fases y dependencias

- **Fase 1 - Preparación**: agrega EF Core/Npgsql y prepara el proyecto sin alterar
  `global.json` ni crear otro proyecto.
- **Fase 2 - Fundamentos persistentes**: crea entidades, contexto y configuraciones;
  bloquea la migración y el seeding.
- **Fase 3 - US1**: valida el modelo y genera/revisa la única migración.
- **Fase 4 - US2**: implementa seeding nativo, migración automática y arranque.
- **Fase 5 - US3**: integra manifiesto, JSON, imágenes y `ImageUrl` público.
- **Fase 6 - Pulido**: ejecuta build, tests, revisión de restricciones y evidencia.

---

## Fase 1: Preparación compartida

**Propósito**: preparar dependencias y rutas de contenido del proyecto existente.

- [X] T001 Actualizar `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` para agregar EF Core Design, EF Core PostgreSQL/Npgsql y las dependencias runtime alineadas con .NET 10, sin modificar `global.json` ni eliminar referencias de la spec 002.
- [X] T002 [P] Actualizar `app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` con las dependencias necesarias para probar el modelo, migración y seeding relacional, sin usar SQLite para afirmar comportamiento específico de PostgreSQL.
- [X] T003 [P] Crear los directorios trazados `app/backend/src/NetRentManagerApi/Domain/Properties`, `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Configurations`, `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Migrations`, `app/backend/src/NetRentManagerApi/wwwroot/assets/properties` y `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence`.

**Checkpoint**: el proyecto restaura EF Core/Npgsql y las rutas de persistencia y
assets están preparadas.

---

## Fase 2: Fundamentos persistentes bloqueantes

**Propósito**: crear el modelo y su configuración antes de generar migraciones o
insertar datos.

- [X] T004 [P] [US1] Crear `PropertyStatus` en `app/backend/src/NetRentManagerApi/Domain/Properties/PropertyStatus.cs` con exactamente `Available`, `Rented` y `Maintenance`.
- [X] T005 [P] [US1] Crear `Property` en `app/backend/src/NetRentManagerApi/Domain/Properties/Property.cs` con identidad, datos descriptivos, precio, estado, dimensiones, `ImageUrl` y timestamps según `data-model.md`.
- [X] T006 [US1] Crear `AppDbContext` en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/AppDbContext.cs` con los `DbSet` requeridos y `ApplyConfigurationsFromAssembly`, sin mapping inline.
- [X] T007 [P] [US1] Crear `PropertyConfiguration` en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Configurations/PropertyConfiguration.cs` con tabla, clave, restricciones, longitudes, precisiones, defaults, conversión enum-string e índices definidos en `data-model.md`.
- [X] T008 [P] [US1] Crear `PropertyStatusConfiguration` en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Configurations/PropertyStatusConfiguration.cs` con catálogo textual, clave única, descripción requerida y restricciones coherentes con el contrato.
- [X] T009 [US1] Registrar `AppDbContext` con Npgsql, connection string y descubrimiento de configuraciones en `app/backend/src/NetRentManagerApi/Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs`, sin alterar el registro ni comportamiento de `HealthSlice`.

**Checkpoint**: el modelo compila, el contexto descubre configuraciones por assembly
y todavía no existe ninguna migración generada antes de validar el modelo.

---

## Fase 3: Historia de Usuario 1 - Persistir propiedades y estados (P1)

**Objetivo**: entregar el esquema persistente completo en una única migración revisada.

**Prueba independiente**: construir el modelo, verificar conversión enum-string y
revisar la migración y el snapshot contra `persistence-contracts.md`.

### Pruebas de US1

- [X] T010 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/PropertyModelTests.cs` para verificar tablas, claves, nulabilidad, longitudes, precisiones, defaults, restricciones e índices de `Property` y `PropertyStatus`.
- [X] T011 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/PropertyStatusConversionTests.cs` para comprobar que `Available`, `Rented` y `Maintenance` se almacenan como texto y se recuperan como enum.
- [X] T012 [US1] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/MigrationShapeTests.cs` para comprobar que existe una única migración funcional, incluye tablas/columnas esperadas y conserva operaciones revisables `Up` y `Down`.

### Implementación de US1

- [X] T013 [US1] Compilar `app/backend/src/NetRentManagerApi` y validar entidades, `DbSet` y configuraciones antes de generar la migración, registrando el resultado en la secuencia de tareas.
- [X] T014 [US1] Generar la única migración `AddPropertyManagementEntities` en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Migrations/` mediante `dotnet ef migrations add` con proyecto y startup project `NetRentManagerApi`.
- [X] T015 [US1] Revisar `Up`, `Down` y el snapshot de `AddPropertyManagementEntities` en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Migrations/`, confirmando tablas, tipos, nulabilidad, claves, índices, conversión textual y ausencia de operaciones destructivas.

**Checkpoint**: existe exactamente una migración coherente y revisada para todo el
cambio de propiedades y estados; no se usa `dotnet ef database update` como gate.

---

## Fase 4: Historia de Usuario 2 - Seeding idempotente y arranque (P1)

**Objetivo**: aplicar migraciones y cargar datos iniciales mediante los hooks nativos
de EF Core, sin fallback ni invocación manual del seeder.

**Prueba independiente**: ejecutar el arranque y el seeding síncrono/asíncrono sobre
un proveedor relacional autorizado, repetirlo y comparar conteos, claves y URLs.

### Pruebas de US2

- [X] T016 [P] [US2] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/DatabaseSeederTests.cs` para verificar idempotencia por identificador, estados existentes, propiedades existentes y equivalencia observable entre `Seed` y `SeedAsync`.
- [X] T017 [P] [US2] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/UseAsyncSeedingTests.cs` para verificar que `UseAsyncSeeding` se ejecuta mediante `MigrateAsync` aunque no haya migraciones pendientes y propaga cancelación.
- [X] T018 [US2] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/MigrationStartupTests.cs` para comprobar que `Program.cs` invoca `await app.MigrateAsync()` antes de `app.Run()` y que `MigrationExtensions` no invoca manualmente `DatabaseSeeder`.

### Implementación de US2

- [X] T019 [US2] Crear `DatabaseSeeder` en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/DatabaseSeeder.cs` con métodos `Seed` y `SeedAsync` equivalentes, idempotentes y con validación previa de fuentes, estados, propiedades e imágenes.
- [X] T020 [US2] Crear `MigrationExtensions` en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/MigrationExtensions.cs` para aplicar `Database.MigrateAsync()` a PostgreSQL, usar fallback solo para proveedores no relacionales autorizados y no llamar manualmente al seeder.
- [X] T021 [US2] Configurar `UseSeeding` y `UseAsyncSeeding` durante el registro de `AppDbContext` en `app/backend/src/NetRentManagerApi/Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs`, delegando ambos en `DatabaseSeeder` con el mismo algoritmo.
- [X] T022 [US2] Actualizar `app/backend/src/NetRentManagerApi/Program.cs` para ejecutar `await app.MigrateAsync()` siempre antes de `app.Run()`, manteniendo `HealthSlice` y el mapeo de slices sin cambios no requeridos.

**Checkpoint**: el arranque aplica migraciones y EF Core dispara el seeding; repetir
la operación no duplica registros y no existe una ruta manual paralela.

---

## Fase 5: Historia de Usuario 3 - Assets e imágenes públicas (P1)

**Objetivo**: hacer consumibles en runtime y publish los JSON, manifiesto e imágenes,
y persistir solo URLs públicas finales.

**Prueba independiente**: leer el manifiesto real, sincronizar las diez imágenes,
comprobar su destino y validar que cada propiedad usa `/assets/properties/<archivo>`.

### Pruebas de US3

- [X] T023 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/SeedManifestTests.cs` para validar versión, campos obligatorios, rutas fuente, destino público y rechazo de rutas ausentes, duplicadas o fuera del manifiesto.
- [X] T024 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/SeedAssetsTests.cs` para comprobar que los JSON, manifiesto y diez imágenes quedan disponibles en output/publish y que el destino contiene las copias esperadas.
- [X] T025 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/ImageUrlTests.cs` para comprobar que ninguna `ImageUrl` contiene `support` y que todas usan `/assets/properties/<archivo>` con archivo existente.

### Implementación de US3

- [X] T026 [US3] Crear los modelos de manifiesto y lectura validada en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/SeedManifest.cs` y `SeedDataReader.cs`, resolviendo todas las rutas desde `seed-manifest.json`.
- [X] T027 [US3] Integrar `support/seed-data/properties.json`, `support/seed-data/properties-statuses.json`, `support/seed-data/seed-manifest.json` y `support/seed-data/images/properties/` en `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` como contenido copiable a output y publish.
- [X] T028 [US3] Implementar la sincronización de imágenes en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/SeedAssetSynchronizer.cs`, creando `wwwroot/assets/properties`, validando archivos declarados y construyendo URLs desde `publicImageBasePath` sin rutas físicas.
- [X] T029 [US3] Integrar `SeedDataReader` y `SeedAssetSynchronizer` en `DatabaseSeeder` sin duplicar rutas hardcodeadas, validando estados e imágenes antes de insertar propiedades.

**Checkpoint**: los assets están disponibles en runtime/publish, las diez imágenes
son accesibles bajo `wwwroot/assets/properties` y `ImageUrl` solo contiene rutas públicas.

---

## Fase 6: Pulido y validación transversal

**Propósito**: revisar migración, probar restricciones negativas, documentar evidencia y
cerrar solo cuando el estado constitucional lo permita.

- [X] T030 Ejecutar `dotnet build .\app\NetRentManager.sln --no-restore` y corregir errores sin modificar `global.json`, `HealthSlice` ni ampliar el alcance.
- [X] T031 Ejecutar `dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore` con el proveedor relacional autorizado y registrar conteos de pruebas, seeding síncrono/asíncrono e idempotencia.
- [X] T032 [P] Publicar o preparar output del proyecto API y verificar que JSON, manifiesto e imágenes están presentes en sus destinos finales sin archivos bajo `support` usados como `ImageUrl`.
- [X] T033 [P] Inspeccionar `app/backend/src/NetRentManagerApi/` para confirmar que no existen controllers, `HasData`, SQL manual, endpoints de negocio fuera de `Features`, múltiples migraciones funcionales ni base de datos alternativa.
- [X] T034 Registrar en `specs/003-properties-persistence-seeding/quickstart.md`, en `## Evidencia de validación`, fecha, comandos, proveedor de prueba, build/tests, revisión de migración, dos ejecuciones del seed y verificación de assets.
- [X] T035 Confirmar que todas las tareas de `specs/003-properties-persistence-seeding/tasks.md` están marcadas `[X]`, que no quedan tareas pendientes y que la spec permanece en `En implementación` hasta cumplir todas las condiciones de cierre.

**Checkpoint final**: la migración única está revisada, el seeding es idempotente,
los assets son públicos y la evidencia permite la transición automática a
`Implementada`.

---

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Fase 1** no depende de otra fase.
- **Fase 2** depende de T001-T003 y bloquea migración y seeding.
- **US1** depende de T004-T009; T014 depende de T013 y T015 depende de T014.
- **US2** depende de la migración revisada T015 y de los contratos del contexto.
- **US3** puede preparar modelos de lectura en paralelo con US2, pero T029 debe
  integrarse después de T019 y T026-T028.
- **Fase 6** depende de completar US1, US2 y US3.

### Dependencias de historias

- **US1** es el MVP técnico: modelo, configuraciones y migración única.
- **US2** depende de US1 porque el seeder requiere tablas y `DbSet` disponibles.
- **US3** puede preparar validaciones de assets después de T001, pero su integración
  final depende de `DatabaseSeeder` y del modelo de US1.

### Oportunidades paralelas

- T002-T003 pueden ejecutarse en paralelo con T001.
- T004-T005 y T007-T008 pueden ejecutarse en paralelo después de preparar rutas.
- T010-T012 pueden escribirse en paralelo y deben existir antes de cerrar US1.
- T016-T018 pueden escribirse en paralelo; T023-T025 también.
- T019-T020 pueden implementarse en paralelo si el contrato del contexto está cerrado;
  T021-T022 deben actualizar el mismo punto de composición secuencialmente.
- T026-T028 pueden ejecutarse en paralelo en archivos distintos; T029 integra el
  resultado en el seeder.
- T030-T033 son validaciones independientes; T034 depende de sus resultados.

---

## Ejemplos de ejecución paralela

### US1

```text
T010 PropertyModelTests.cs
T011 PropertyStatusConversionTests.cs
T012 MigrationShapeTests.cs

Después de validar el modelo:
T013 compilación del proyecto
T014 migración única
T015 revisión de Up, Down y snapshot
```

### US2

```text
T016 DatabaseSeederTests.cs
T017 UseAsyncSeedingTests.cs
T018 MigrationStartupTests.cs

Después de cerrar el contrato del contexto:
T019 DatabaseSeeder.cs
T020 MigrationExtensions.cs
```

### US3

```text
T023 SeedManifestTests.cs
T024 SeedAssetsTests.cs
T025 ImageUrlTests.cs

En paralelo:
T026 SeedManifest.cs y SeedDataReader.cs
T027 NetRentManagerApi.csproj
T028 SeedAssetSynchronizer.cs
```

---

## Estrategia de implementación

### MVP primero

1. Completar Fase 1 y Fase 2.
2. Completar US1 y generar/revisar `AddPropertyManagementEntities`.
3. Ejecutar build y pruebas de modelo/migración.
4. Detenerse para validar que el esquema persistente es coherente antes de introducir
   seeding o assets.

### Entrega incremental

1. Agregar US2 y comprobar `MigrateAsync`, hooks nativos e idempotencia.
2. Agregar US3 y comprobar manifiesto, imágenes, output/publish e `ImageUrl`.
3. Ejecutar Fase 6 y documentar evidencia real en `quickstart.md`.
4. Cambiar la spec a `Implementada` solo mediante el flujo automático cuando no haya
   tareas pendientes y todas las validaciones estén documentadas.

### Criterios de finalización por historia

- **US1**: T004-T015 completadas; entidades, configuraciones y una única migración revisada.
- **US2**: T016-T022 completadas; ambos hooks de seeding, arranque e idempotencia verificados.
- **US3**: T023-T029 completadas; manifiesto, assets, destino público y URLs consistentes.
- **Cierre**: T030-T035 completadas; evidencia completa, restricciones negativas confirmadas y sin tareas `[ ]`.
