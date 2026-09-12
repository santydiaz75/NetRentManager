---

description: "Task list for feature implementation"
---

# Tasks: Secret Manager para credenciales de conexión

**Input**: Documentos de diseño de `specs/011-secret-manager-credentials/`

**Prerrequisitos**: plan.md (requerido), spec.md (requerido para historias de usuario), research.md, data-model.md, quickstart.md

**Pruebas**: Se incluyen tareas de prueba porque `plan.md` (sección "Diseño técnico > Validación") las exige explícitamente para esta iniciativa.

**Organización**: Las tareas están agrupadas por historia de usuario para permitir implementación y prueba independiente de cada una.

## Formato: `[ID] [P?] [Story] Descripción`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencias)
- **[Story]**: Historia de usuario a la que pertenece (US1, US2, US3)
- Cada tarea incluye la ruta exacta de archivo

## Phase 1: Setup (Infraestructura compartida)

**Propósito**: Preparar la convención de `.gitignore` usada por las historias siguientes.

- [X] T001 Agregar el patrón `appsettings.*.Local.json` al `.gitignore` de la raíz del repositorio (`.gitignore`), junto al ya existente `appsettings.Local.json`, para cubrir archivos locales de overrides por entorno (ej. `appsettings.Development.Local.json`) que nunca deben trackearse en Git.

**Checkpoint**: El `.gitignore` cubre el patrón de overrides local antes de continuar con las historias de usuario.

---

## Phase 2: User Story 1 - Eliminar credenciales en texto plano de los archivos versionados (Priority: P1) 🎯 MVP

**Objetivo de la historia**: `appsettings.json` y `appsettings.Development.json` ya no contienen usuario ni contraseña de la base de datos en texto plano.

**Prueba independiente**: Inspeccionar el contenido de ambos archivos y confirmar que `ConnectionStrings:DefaultConnection` ya no incluye `Username` ni `Password`.

- [X] T002 [P] [US1] Quitar `Username` y `Password` de `ConnectionStrings:DefaultConnection` en `app/backend/src/NetRentManagerApi/appsettings.json`, dejando solo `Host`, `Database`, `SSL Mode` y `Channel Binding`.
- [X] T003 [P] [US1] Quitar `Username` y `Password` de `ConnectionStrings:DefaultConnection` en `app/backend/src/NetRentManagerApi/appsettings.Development.json`, dejando solo `Host`, `Database`, `SSL Mode` y `Channel Binding`.

**Checkpoint**: Ningún archivo de configuración versionado contiene credenciales en texto plano (SC-001 verificable de forma aislada; la aplicación aún no arrancará hasta completar la Historia 2).

---

## Phase 3: User Story 2 - Leer credenciales de forma segura en desarrollo mediante Secret Manager (Priority: P2)

**Objetivo de la historia**: La aplicación se conecta a la base de datos en desarrollo combinando la cadena de conexión no sensible con el usuario y la contraseña provistos por Secret Manager, y falla con un mensaje claro si faltan.

**Prueba independiente**: Configurar credenciales con `dotnet user-secrets` y arrancar la aplicación; luego quitar una credencial y verificar que falla con un mensaje explícito.

### Pruebas para esta historia

- [X] T004 [US2] Escribir pruebas unitarias en `app/backend/tests/NetRentManagerApiTests/Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensionsTests.cs` que verifiquen: (a) el ensamblado correcto de la cadena de conexión combinando la base de `ConnectionStrings:DefaultConnection` con `DatabaseCredentials:Username`/`DatabaseCredentials:Password` mediante `NpgsqlConnectionStringBuilder`; (b) que se lanza `InvalidOperationException` con mensaje claro cuando falta la cadena base; (c) que se lanza `InvalidOperationException` con mensaje claro cuando falta el usuario; (d) que se lanza `InvalidOperationException` con mensaje claro cuando falta la contraseña.

### Implementación para esta historia

- [X] T005 [US2] Modificar `AddInfrastructure` en `app/backend/src/NetRentManagerApi/Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs` para leer `DatabaseCredentials:Username` y `DatabaseCredentials:Password` desde `IConfiguration`, combinarlos con la cadena base mediante `NpgsqlConnectionStringBuilder`, y lanzar `InvalidOperationException` con un mensaje claro indicando qué falta (cadena base, usuario o contraseña) antes de registrar `AppDbContext`.
- [X] T006 [US2] Ejecutar la sección "Configurar Secret Manager en desarrollo" de `specs/011-secret-manager-credentials/quickstart.md` en un entorno local y confirmar que la aplicación arranca y se conecta correctamente a la base de datos con las credenciales provistas por Secret Manager.
- [X] T007 [US2] Ejecutar la sección "Validar el fallo explícito sin credenciales" de `specs/011-secret-manager-credentials/quickstart.md`, confirmar que la aplicación falla al iniciar con un mensaje claro al faltar una credencial, y restaurar la credencial eliminada.

**Checkpoint**: En este punto, las Historias 1 y 2 combinadas entregan un MVP funcional: sin credenciales en texto plano y aplicación operativa en desarrollo vía Secret Manager (SC-002, SC-003).

---

## Phase 4: User Story 3 - Documentar el manejo de credenciales en producción (Priority: P3)

**Objetivo de la historia**: El equipo cuenta con documentación clara sobre cómo proveer las credenciales en producción, sin depender de Secret Manager.

**Prueba independiente**: Revisar la documentación del proyecto y confirmar que explica ambas opciones (variables de entorno y Azure Key Vault) para producción.

- [X] T008 [US3] Confirmar que la sección "Configurar credenciales en producción" de `specs/011-secret-manager-credentials/quickstart.md` documenta ambas opciones (`DatabaseCredentials__Username`/`DatabaseCredentials__Password` por variables de entorno, y un proveedor de configuración respaldado por Azure Key Vault) sin priorizar una sobre la otra, y ajustar la redacción si falta claridad tras completar T005.

**Checkpoint**: La documentación de producción es consistente con la implementación final de `AddInfrastructure` (SC-004 y FR-006 verificables).

---

## Dependencies & Execution Order

### Orden entre fases

- **Setup (Phase 1)**: Sin dependencias. T001 puede ejecutarse en cualquier momento.
- **User Story 1 (Phase 2)**: Sin dependencias de otras historias. Puede iniciarse inmediatamente.
- **User Story 2 (Phase 3)**: Depende de que Historia 1 haya quitado las credenciales de los archivos (T002, T003), ya que T005 asume que `ConnectionStrings:DefaultConnection` ya no contiene `Username`/`Password`.
- **User Story 3 (Phase 4)**: Depende de que Historia 2 (T005) haya definido la clave de configuración final `DatabaseCredentials:Username`/`Password`, para poder confirmar que la documentación de producción es consistente.

### Dependencias dentro de cada historia

- Historia 1: T002 y T003 son independientes entre sí (archivos distintos).
- Historia 2: T004 (pruebas) antes de T005 (implementación); T006 y T007 dependen de T005 completada.
- Historia 3: T008 depende de T005 completada.

### Oportunidades de paralelización

- T002 y T003 pueden ejecutarse en paralelo (`[P]`, archivos distintos).
- T001 (Setup) puede ejecutarse en paralelo con la Historia 1, ya que no comparten archivos.

## Implementation Strategy

### Entrega de MVP (Historias 1 y 2)

1. Completar Phase 1 (Setup: `.gitignore`).
2. Completar Phase 2 (User Story 1: quitar credenciales de `appsettings.json`/`appsettings.Development.json`).
3. Completar Phase 3 (User Story 2: ensamblado con Secret Manager + pruebas + validación de quickstart).
4. **PARAR y validar**: en este punto la aplicación funciona en desarrollo sin credenciales en texto plano — MVP entregable.

### Entrega incremental

1. Agregar Phase 4 (User Story 3: confirmar documentación de producción) una vez validado el MVP.
2. Cada fase deja la aplicación en un estado funcional y verificable de forma independiente.
