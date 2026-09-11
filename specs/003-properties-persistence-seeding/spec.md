# Especificación de la Funcionalidad: Persistencia y Seeding de Propiedades

**Rama de la funcionalidad**: `003-properties-persistence-seeding`

**Creado**: 2026-09-10

**Estado**: Implementada

**Trazabilidad de estado**:

- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar prerrequisitos y checklist | Fecha: 2026-09-10
- `En implementación` -> `Implementada` | Motivo: cierre con `tasks.md` sin pendientes y `quickstart.md` con evidencia de validación completa | Fecha: 2026-09-10

**Entrada**: Descripción de usuario: "Definir e implementar el modelo persistente inicial de propiedades y estados de propiedad, incluyendo migración EF Core, seeding idempotente automático y manejo de archivos externos para que queden consumibles por el proceso de inserción y por futuras consultas de propiedades con imagen."

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Persistir propiedades y estados (Prioridad: P1)

Como equipo de desarrollo, necesito un modelo persistente de propiedades con estados controlados, restricciones consistentes y una migración versionada, para que las futuras funcionalidades trabajen sobre una base de datos estable.

**Por qué esta prioridad**: Sin el modelo y el esquema inicial no pueden existir casos de uso confiables para crear, consultar o cambiar el estado de una propiedad.

**Prueba independiente**: Se construye el modelo EF Core, se inspecciona la configuración de entidades y se verifica que la migración única representa las tablas, columnas, restricciones, precisiones y conversión de estado definidas.

**Escenarios de aceptación**:

1. **Dado** el modelo persistente configurado, **cuando** se construye el modelo de EF Core, **entonces** existen `Property` y `PropertyStatus` con sus propiedades requeridas y no hay configuración inline en `OnModelCreating`.
2. **Dado** una propiedad con estado `Available`, `Rented` o `Maintenance`, **cuando** se persiste, **entonces** el estado se almacena como su valor de texto y se recupera con el mismo valor semántico.
3. **Dado** el cambio funcional completo de esta iniciativa, **cuando** se revisan las migraciones, **entonces** existe una sola migración coherente con `Up`, `Down` y snapshot revisados.

### Historia de Usuario 2 - Ejecutar seeding idempotente y migraciones al iniciar (Prioridad: P1)

Como responsable de operación, necesito que la aplicación aplique migraciones y ejecute el seeding inicial automáticamente al iniciar, para disponer de datos base sin invocaciones manuales y sin duplicarlos en reinicios.

**Por qué esta prioridad**: El arranque repetible garantiza que todos los entornos obtengan el esquema y los datos iniciales de forma controlada.

**Prueba independiente**: Se ejecuta el seeding síncrono y asíncrono sobre una base de datos de prueba, se repite el proceso y se verifica que las cantidades y claves permanecen estables; además se confirma que `UseAsyncSeeding` se invoca durante el arranque aunque no existan migraciones pendientes.

**Escenarios de aceptación**:

1. **Dado** un entorno sin esquema actualizado, **cuando** inicia la aplicación, **entonces** se ejecuta `app.MigrateAsync()` antes de `app.Run` y el esquema queda actualizado.
2. **Dado** un entorno recién migrado, **cuando** se ejecuta el seeding asíncrono, **entonces** se cargan los estados y propiedades definidos en los archivos fuente.
3. **Dado** un entorno que ya contiene los datos semilla, **cuando** la aplicación reinicia y vuelve a ejecutar el seeding, **entonces** no se crean duplicados ni se alteran registros existentes fuera de las reglas definidas.
4. **Dado** un proveedor que ejecuta el proceso de inicialización sin migraciones pendientes, **cuando** se inicia la aplicación, **entonces** `UseAsyncSeeding` sigue ejecutando la ruta asíncrona de seeding.

### Historia de Usuario 3 - Incorporar archivos externos e imágenes servibles (Prioridad: P1)

Como consumidor de futuras consultas de propiedades, necesito que los datos JSON y las imágenes de soporte estén disponibles en runtime y publish, para que cada propiedad conserve una `ImageUrl` pública utilizable sin exponer rutas físicas internas.

**Por qué esta prioridad**: Una propiedad sin una referencia de imagen servible no cumple el contrato de datos inicial que necesitan las consultas futuras.

**Prueba independiente**: Se leen `properties.json`, `properties-statuses.json` y `seed-manifest.json`, se sincronizan las imágenes al destino final del proyecto API y se verifica que cada `ImageUrl` persistida apunte a una ruta pública final, nunca a `support`.

**Escenarios de aceptación**:

1. **Dado** el proyecto API compilado o publicado, **cuando** el seeding necesita los archivos fuente, **entonces** los JSON y el manifiesto están disponibles en runtime.
2. **Dado** una imagen declarada por el manifiesto, **cuando** se prepara el destino público, **entonces** el archivo existe en la ubicación final consumible por la API.
3. **Dado** una propiedad semilla con imagen, **cuando** se persiste, **entonces** `ImageUrl` contiene la ruta pública final y no una ruta física de `support`.
4. **Dado** un manifiesto con una entrada inválida, duplicada o inexistente, **cuando** se procesa, **entonces** el seeding falla con un diagnóstico claro sin insertar una referencia inconsistente.

### Casos límite

- Si un estado del JSON no coincide con los estados permitidos, el seeding debe fallar de forma explícita antes de crear una propiedad inválida.
- Si falta un archivo JSON, el manifiesto o una imagen declarada, el arranque debe reportar la causa y no persistir referencias incompletas.
- Si los archivos fuente contienen un identificador ya existente, el seeding debe aplicar la regla idempotente definida y no insertar duplicados.
- Si dos propiedades referencian la misma imagen, la sincronización no debe crear copias inconsistentes y ambas deben conservar una ruta pública válida.
- Si una propiedad tiene `UpdatedAt` ausente, el modelo debe aplicar el comportamiento predeterminado definido para el primer registro.
- Si se ejecuta el seeding síncrono y asíncrono sobre el mismo conjunto, ambos deben producir el mismo resultado observable.
- Si el destino público de imágenes no existe, el proceso debe crearlo o fallar con un diagnóstico accionable antes de persistir `ImageUrl`.
- Si la aplicación arranca sin conexión a PostgreSQL, debe devolver un error de inicialización claro y no simular una base de datos disponible.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El backend DEBE definir `PropertyStatus` con exactamente los valores `Available`, `Rented` y `Maintenance`.
- **RF-002**: El backend DEBE definir `Property` con `Id`, `Title`, `Description`, `Address`, `Price`, `Status`, `BedroomCount`, `BathroomCount`, `AreaSquareMeters`, `ImageUrl`, `CreatedAt` y `UpdatedAt`.
- **RF-003**: El estado de `Property` DEBE almacenarse como texto mediante conversión EF Core y recuperarse como el valor semántico correspondiente.
- **RF-004**: Cada entidad persistente DEBE tener una configuración separada que implemente `IEntityTypeConfiguration<T>` bajo `Infrastructure/Persistence/Configurations`.
- **RF-005**: `AppDbContext` DEBE exponer los `DbSet` necesarios y descubrir las configuraciones mediante `ApplyConfigurationsFromAssembly`, sin mapping inline en `OnModelCreating`.
- **RF-006**: Las configuraciones DEBEN definir restricciones, longitudes, precisión, defaults e índices necesarios para el modelo funcional, de forma coherente con el esquema aprobado en el plan.
- **RF-007**: La iniciativa DEBE generar una sola migración EF Core para el cambio completo de propiedades y estados, revisando `Up`, `Down` y snapshot antes del cierre.
- **RF-008**: El contexto DEBE configurar `UseSeeding` y `UseAsyncSeeding` con comportamientos síncrono y asíncrono equivalentes.
- **RF-009**: `DatabaseSeeder` DEBE ser idempotente y evitar duplicar estados, propiedades o referencias de imágenes cuando se ejecute repetidamente.
- **RF-010**: El proceso de arranque DEBE ejecutar `app.MigrateAsync()` siempre antes de `app.Run`, sin condicionarlo a la existencia aparente de migraciones pendientes.
- **RF-011**: El flujo DEBE ejecutar la ruta de `UseAsyncSeeding` incluso cuando no haya migraciones pendientes y no DEBE invocar manualmente el seeder desde `MigrationExtensions` ni como fallback paralelo.
- **RF-012**: Los archivos `properties.json`, `properties-statuses.json`, la carpeta `properties` y `support/seed-data/seed-manifest.json` DEBEN integrarse al proyecto API para estar disponibles durante build, publish y runtime.
- **RF-013**: El seeding DEBE resolver las fuentes y destinos mediante `seed-manifest.json`, sin rutas físicas hardcodeadas dispersas en el código.
- **RF-014**: Las imágenes DEBEN sincronizarse a una ubicación final pública consumible por la API y conservarse disponibles en runtime y publish.
- **RF-015**: `ImageUrl` DEBE persistir la ruta pública final servida por la API y NO DEBE contener rutas físicas bajo `support`.
- **RF-016**: El flujo DEBE validar lectura de JSON, manifiesto, integridad de estados, copia de imágenes e idempotencia antes de insertar referencias de propiedades.
- **RF-017**: La solución DEBE conservar Minimal APIs, Vertical Slice Architecture y el patrón de slice existente, incluyendo `HealthSlice`, sin cambios no requeridos por esta spec.
- **RF-018**: Todo endpoint futuro de negocio relacionado con propiedades DEBE vivir en `Features/Properties/<caso-de-uso>`; no se permite una jerarquía paralela de endpoints de negocio.
- **RF-019**: `Shared` dentro de `Features/Properties` SOLO DEBE crearse si existe reutilización real entre al menos dos slices de la misma feature.
- **RF-020**: La iniciativa NO DEBE usar controllers, `HasData`, invocaciones manuales del seeder como fallback, múltiples migraciones para el mismo cambio funcional ni `dotnet ef database update` como criterio obligatorio de cierre.
- **RF-021**: Todo cambio de persistencia, migración, seeder, configuración, asset o archivo de soporte DEBE quedar trazado a una tarea específica en `tasks.md` antes de marcarse como completado.

### Entidades clave

- **PropertyStatus**: catálogo cerrado de estados de disponibilidad operativa de una propiedad.
- **Property**: propiedad persistente con información descriptiva, económica, dimensional, de estado, imagen y auditoría temporal.
- **AppDbContext**: contexto de persistencia que expone propiedades y estados y aplica configuraciones separadas.
- **DatabaseSeeder**: componente responsable de cargar estados, propiedades e imágenes de forma idempotente en versiones síncrona y asíncrona.
- **SeedManifest**: manifiesto que relaciona fuentes JSON, imágenes y destinos públicos finales.
- **Migration**: versión única del esquema correspondiente al cambio funcional completo.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **CE-001**: El modelo `Property` y el catálogo `PropertyStatus` compilan y pasan el 100% de las pruebas de configuración y conversión enum-string.
- **CE-002**: Existe exactamente una migración para el cambio funcional y sus operaciones `Up`, `Down` y snapshot representan el esquema aprobado.
- **CE-003**: El 100% de las ejecuciones repetidas del seeding produce el mismo número de estados, propiedades e imágenes válidas, sin duplicados.
- **CE-004**: El arranque ejecuta `app.MigrateAsync()` antes de `app.Run` y la ruta asíncrona de seeding se verifica tanto con migraciones pendientes como sin ellas.
- **CE-005**: El 100% de los JSON, el manifiesto y las imágenes declaradas están disponibles en runtime y publish según la prueba de assets.
- **CE-006**: El 100% de las propiedades semilla con imagen conserva una `ImageUrl` pública final y ninguna contiene rutas bajo `support`.
- **CE-007**: Los errores de fuente, manifiesto, estado o imagen faltante detienen la inserción inconsistente y exponen una causa diagnosticable.
- **CE-008**: `HealthSlice` mantiene su comportamiento existente y no requiere cambios fuera de los necesarios para registrar persistencia y seeding.
- **CE-009**: No se agregan controllers, `HasData`, endpoints de negocio fuera de `Features`, múltiples migraciones funcionalmente duplicadas ni una base de datos alternativa.

## Suposiciones

- El proyecto real de esta solución es `app/backend/src/NetRentManagerApi`; la referencia del prompt a `NetRentManagerApi` se interpreta como un nombre genérico y no autoriza crear un segundo proyecto o namespace paralelo.
- La spec 002 ya dejó disponible el patrón `ISlice`, `AddInfrastructure` y la composición de `Program.cs`; esta iniciativa los reutiliza y evita modificar `HealthSlice` salvo necesidad técnica documentada.
- PostgreSQL y Npgsql son los proveedores objetivo definidos por la constitución y se configurarán mediante la configuración existente del proyecto.
- Los archivos `support/seed-data/properties.json`, `support/seed-data/properties-statuses.json`, `support/seed-data/seed-manifest.json` y `support/seed-data/images/` son las fuentes iniciales canónicas.
- La definición exacta de longitudes, precisión, defaults, índices, formato del manifiesto y ruta pública final se concretará en `plan.md` sin ampliar este alcance.
- La migración se generará después de completar entidades, `DbSet` y configuraciones, y antes de finalizar la implementación.
- La prueba de seeding usará un proveedor relacional de prueba autorizado por el plan; no se sustituirá PostgreSQL por SQLite si se comprueba comportamiento específico de PostgreSQL.
- Las futuras operaciones de creación, actualización, consulta y cambio de estado solo quedan estructuralmente reservadas en esta iniciativa; sus endpoints funcionales no se implementan aquí.

## Fuera de alcance

- Endpoints completos de crear, actualizar, consultar, listar o cambiar estado de propiedades.
- Autenticación, autorización, usuarios, contratos, rentas o cualquier feature de producto no relacionada con el modelo persistente inicial.
- Cambios al frontend o a la API de comunicación Refit.
- Controllers o una jerarquía de endpoints de negocio fuera de `Features`.
- Datos semilla adicionales que no estén en los JSON y manifiesto canónicos.
- Creación de un proyecto, solución o namespace paralelo llamado `NetRentManagerApi`.
