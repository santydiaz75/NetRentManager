# Guía de Validación: Persistencia y Seeding de Propiedades

## Prerrequisitos

- .NET SDK definido por `global.json` (`10.0.400`).
- PostgreSQL disponible para las pruebas relacionales de seeding, o el proveedor de
  prueba aprobado explícitamente por las tareas.
- PowerShell desde la raíz del repositorio.
- La solución existente en `app/NetRentManager.sln`.

## Preparación

Restaurar y compilar antes de generar la migración:

```powershell
dotnet restore .\app\NetRentManager.sln
dotnet build .\app\NetRentManager.sln --no-restore
```

La migración solo debe generarse después de que entidades, `DbSet` y configuraciones
estén completas y el proyecto compile:

```powershell
dotnet ef migrations add AddPropertyManagementEntities `
  --project .\app\backend\src\NetRentManagerApi `
  --startup-project .\app\backend\src\NetRentManagerApi `
  --output-dir Infrastructure/Persistence/Migrations
```

No se requiere `dotnet ef database update` para cerrar la iniciativa. La aplicación
debe aplicar migraciones al iniciar mediante `await app.MigrateAsync()`.

## Validación de pruebas

Ejecutar las pruebas del backend:

```powershell
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
```

La suite debe cubrir configuración de entidades, conversión enum-string, migración,
lectura del manifiesto, copia de imágenes, seeding síncrono/asíncrono e idempotencia.

## Escenarios de validación

1. Construir el modelo y comprobar tablas, claves, restricciones, precisiones,
   defaults y conversiones.
2. Revisar que exista una única migración `AddPropertyManagementEntities` y que
   `Up`, `Down` y el snapshot sean coherentes.
3. Ejecutar `MigrateAsync` sobre una base sin esquema y comprobar que el seeding se
   dispara mediante `UseAsyncSeeding`.
4. Ejecutar el mismo seeding dos veces y comprobar que no aumenta el número de
   estados, propiedades o imágenes.
5. Ejecutar el camino síncrono y asíncrono sobre la misma fuente y comparar el
   resultado observable.
6. Validar `seed-manifest.json`, los dos JSON, las diez imágenes y el destino público.
7. Confirmar que cada `ImageUrl` usa `/assets/properties/<archivo>` y que ninguna
   contiene `support`.
8. Confirmar que `Program.cs` llama `await app.MigrateAsync()` antes de `app.Run()`
   y que no invoca manualmente el seeder.
9. Confirmar que `HealthSlice` conserva el comportamiento de la spec 002.

## Evidencia de validación

Fecha: 2026-09-10

Proveedor de prueba usado:

- Validación de modelo/migración y seeding en pruebas automatizadas con EF Core InMemory para el fallback no relacional autorizado por plan/instrucciones.
- No se ejecutaron pruebas de integración contra PostgreSQL real en este run porque el flujo actual no provisionó instancia dedicada ni Docker/Testcontainers para este entorno.

Comandos ejecutados:

```powershell
dotnet restore .\app\NetRentManager.sln
dotnet build .\app\backend\src\NetRentManagerApi\NetRentManagerApi.csproj --no-restore
dotnet ef migrations add AddPropertyManagementEntities --project .\app\backend\src\NetRentManagerApi --startup-project .\app\backend\src\NetRentManagerApi --output-dir Infrastructure/Persistence/Migrations
dotnet build .\app\NetRentManager.sln --no-restore
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
dotnet publish .\app\backend\src\NetRentManagerApi\NetRentManagerApi.csproj -c Debug --no-restore -o .\artifacts\publish\NetRentManagerApi
```

Resultados observados:

- `dotnet build .\app\NetRentManager.sln --no-restore`: correcto.
- `dotnet test ...NetRentManagerApiTests.csproj --no-restore`: 37 pruebas totales, 37 correctas, 0 errores.
- Migración única generada: `AddPropertyManagementEntities` con revisión manual de `Up`, `Down` y `AppDbContextModelSnapshot`.
- Verificación publish de assets: `statuses=True properties=True manifest=True images=10` en `artifacts/publish/NetRentManagerApi/support/seed-data`.
- Pruebas de idempotencia del seed: validada doble ejecución para `Seed` y doble ejecución para `SeedAsync` sin duplicados (`DatabaseSeederTests`).
- Verificación de `UseAsyncSeeding` con ruta de `MigrateAsync` y ejecución repetida sin migraciones pendientes (`UseAsyncSeedingTests`).
- Verificación de `ImageUrl`: todas las propiedades semilla usan `/assets/properties/<archivo>` y no contienen `support` (`ImageUrlTests`).
- Verificación de restricciones negativas: sin `controllers`, sin `HasData`, sin SQL manual en migraciones/configuración, sin migraciones funcionales adicionales y con `HealthSlice` preservado.
