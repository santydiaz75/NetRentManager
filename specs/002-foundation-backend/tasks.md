---

description: "Lista de tareas para la fundación interna del backend"
---

# Tareas: Fundación Interna del Backend

**Entrada**: Documentos de diseño de `specs/002-foundation-backend/`

**Prerequisitos**: `spec.md`, `plan.md`, `research.md`, `data-model.md`,
`contracts/http-contracts.md`

**Pruebas**: Se incluyen unit tests porque la spec los exige explícitamente. No se
crean pruebas de integración, `WebApplicationFactory` ni Testcontainers.

**Regla de trazabilidad**: Cada cambio de código y cada validación de esta
iniciativa debe corresponder a una tarea de este archivo.

## Fases y dependencias

- **Fase 1 - Preparación**: agrega las dependencias necesarias sin recrear la
  solución ni los proyectos existentes.
- **Fase 2 - Fundamentos**: crea los contratos transversales y la estructura común;
  bloquea todas las historias hasta completarse.
- **Fase 3 - US1**: habilita slices descubribles y migra `/health`.
- **Fase 4 - US2**: habilita auto-registro de handlers y validación automática.
- **Fase 5 - US3**: habilita `Result`, `ProblemDetails`, logging y composición final.
- **Fase 6 - Pulido**: ejecuta todas las validaciones, documenta evidencia y cierra
  la iniciativa.

---

## Fase 1: Preparación compartida

**Propósito**: preparar dependencias y estructura sin modificar `global.json`, la
solución existente ni introducir persistencia.

- [X] T001 Actualizar `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` para agregar la dependencia de FluentValidation compatible con .NET 10 y su integración de inyección de dependencias, conservando las referencias existentes.
- [X] T002 [P] Actualizar `app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` para referenciar las dependencias de prueba necesarias para ejecutar unit tests de la infraestructura, sin agregar paquetes de integración ni persistencia.
- [X] T003 [P] Crear la estructura de directorios `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints`, `app/backend/src/NetRentManagerApi/Infrastructure/Errors`, `app/backend/src/NetRentManagerApi/Infrastructure/Handlers`, `app/backend/src/NetRentManagerApi/Infrastructure/Validation` y `app/backend/tests/NetRentManagerApiTests/Infrastructure` sin crear features de negocio.

**Checkpoint**: los proyectos existentes restauran sus dependencias y la estructura
transversal está preparada.

---

## Fase 2: Fundamentos transversales bloqueantes

**Propósito**: definir contratos y utilidades compartidas antes de implementar
cualquier historia de usuario.

- [X] T004 [P] Crear el contrato `ISlice` con `AddEndpoint(IEndpointRouteBuilder app)` en `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/ISlice.cs`, manteniéndolo independiente de cualquier feature de negocio.
- [X] T005 [P] Crear el marker interface `IHandler` en `app/backend/src/NetRentManagerApi/Infrastructure/Handlers/IHandler.cs`, sin agregar un handler concreto.
- [X] T006 [P] Crear los contratos `Result<T>` y `Error` en `app/backend/src/NetRentManagerApi/Infrastructure/Errors/Result.cs`, `app/backend/src/NetRentManagerApi/Infrastructure/Errors/Error.cs` y sus tipos auxiliares necesarios, manteniéndolos independientes de ASP.NET Core.
- [X] T007 Crear las extensiones de registro base en `app/backend/src/NetRentManagerApi/Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs`, dejando un punto único para registrar slices, handlers, validators, ProblemDetails y logging sin registros individuales.
- [X] T008 [P] Crear las clases auxiliares de prueba en `app/backend/tests/NetRentManagerApiTests/TestTypes/`, reservando allí los slices, handlers, requests y validators usados por las pruebas y evitando que entren en producción.

**Checkpoint**: los contratos comunes y el punto de composición están definidos;
ninguna historia puede introducir una implementación paralela.

---

## Fase 3: Historia de Usuario 1 - Descubrir y mapear slices automáticamente (P1)

**Objetivo**: registrar slices por assembly scanning, mapearlos centralmente y
servir `/health` sin modificar `Program.cs` por endpoint.

**Prueba independiente**: las pruebas de `RegisterSlices` verifican descubrimiento e
idempotencia sobre el assembly de tests; las pruebas de salud verifican `GET /health`
y su resultado en memoria mediante el mecanismo de mapeo centralizado.

### Pruebas de US1

- [X] T009 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Endpoints/RegisterSlicesTests.cs` para comprobar que `RegisterSlices` descubre la clase `TestSlice` del assembly de tests bajo `ISlice`, no registra duplicados al invocarse dos veces y no descubre tipos de assemblies no solicitados.
- [X] T010 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Endpoints/HealthSliceTests.cs` para comprobar que el slice registra `GET /health`, que el mapeo centralizado lo incluye y que su resultado ejecutado sobre `DefaultHttpContext` devuelve HTTP 200 sin persistencia.

### Implementación de US1

- [X] T011 [US1] Implementar `RegisterSlices(IServiceCollection, Assembly)` en `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/SliceRegistrationExtensions.cs` usando reflection sobre el assembly recibido, clases públicas concretas que implementen `ISlice` y `TryAddEnumerable` con lifetime singleton.
- [X] T012 [US1] Implementar `MapSliceEndpoints(IEndpointRouteBuilder)` en `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/SliceEndpointMappingExtensions.cs` para crear el grupo de rutas, aplicar el filtro de validación global, resolver `IEnumerable<ISlice>` y llamar `AddEndpoint` sin registry manual.
- [X] T013 [US1] Implementar `HealthSlice` en `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/HealthSlice.cs` con `GET /health`, payload estable de salud y sin acceso a base de datos ni lógica de negocio.
- [X] T014 [US1] Integrar el registro y mapeo de slices en `app/backend/src/NetRentManagerApi/Program.cs` usando únicamente las extensiones centralizadas, sin registrar `/health` de forma individual.

**Checkpoint**: una clase `ISlice` añadida al assembly aprobado se registra y mapea
sin modificar `Program.cs`; `/health` queda disponible como sonda de infraestructura.

---

## Fase 4: Historia de Usuario 2 - Validación y auto-registro de handlers (P1)

**Objetivo**: registrar handlers y validators por assembly scanning y aplicar
validación automática con pass-through cuando no exista validator.

**Prueba independiente**: los tests ejecutan scanners sobre tipos definidos en el
assembly de tests y ejercen `ValidationFilterFactory` con validator válido, inválido
y ausente, verificando HTTP 400 y todos los errores.

### Pruebas de US2

- [X] T015 [P] [US2] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Handlers/RegisterHandlersTests.cs` para verificar que `RegisterHandlers` descubre `TestHandler` bajo `IHandler`, usa lifetime scoped y no registra duplicados al repetirse.
- [X] T016 [P] [US2] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Validation/ValidationFilterFactoryTests.cs` para verificar detección de `IValidator<T>`, ejecución de `ValidateAsync`, propagación de cancellation token, pass-through sin validator y que el delegate no se ejecuta cuando la validación falla.
- [X] T017 [P] [US2] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Validation/ValidationProblemDetailsTests.cs` para ejecutar el resultado inválido sobre `DefaultHttpContext`, comprobar HTTP 400, content type de Problem Details y conservación de errores agrupados por propiedad.

### Implementación de US2

- [X] T018 [US2] Implementar `RegisterHandlers(IServiceCollection, Assembly)` en `app/backend/src/NetRentManagerApi/Infrastructure/Handlers/HandlerRegistrationExtensions.cs` usando reflection sobre el assembly recibido, implementaciones concretas de `IHandler`, lifetime scoped y `TryAddEnumerable`, sin crear handlers de negocio.
- [X] T019 [US2] Implementar `RegisterValidators(IServiceCollection, Assembly)` en `app/backend/src/NetRentManagerApi/Infrastructure/Validation/ValidatorRegistrationExtensions.cs` mediante `AddValidatorsFromAssembly` con lifetime scoped, sin registros individuales.
- [X] T020 [US2] Implementar `ValidationFilterFactory` en `app/backend/src/NetRentManagerApi/Infrastructure/Validation/ValidationFilterFactory.cs` para inspeccionar parámetros validables, consultar `IServiceProviderIsService`, resolver el validator dentro del scope de request, propagar `CancellationToken` y devolver `ValidationProblemDetails` HTTP 400 o pass-through.
- [X] T021 [US2] Integrar `RegisterHandlers` y `RegisterValidators` en `app/backend/src/NetRentManagerApi/Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs` y aplicar la factory desde el mapeo centralizado sin tocar endpoints individuales.

**Checkpoint**: handlers y validators de assemblies aprobados se registran sin
configuración individual; la ausencia de validator no rompe el request.

---

## Fase 5: Historia de Usuario 3 - Errores uniformes y arranque mínimo (P1)

**Objetivo**: centralizar el mapeo de errores esperados, habilitar ProblemDetails y
mantener `Program.cs` como composición de infraestructura.

**Prueba independiente**: los tests verifican status code y payload de errores con y
sin detalles opcionales; el build y la inspección de `Program.cs` confirman que no
hay lógica de negocio ni registros manuales.

### Pruebas de US3

- [X] T022 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Errors/ResultProblemDetailsMappingTests.cs` para comprobar el mapeo centralizado de `NotFound` a 404, `Conflict` a 409, `Validation` a 400 y `Forbidden` a 403, conservando código, tipo y detalles seguros.
- [X] T023 [P] [US3] Ampliar `app/backend/tests/NetRentManagerApiTests/Infrastructure/Errors/ResultProblemDetailsMappingTests.cs` para verificar que los detalles opcionales nulos producen un `ProblemDetails` válido y que no se exponen excepciones internas o stack traces.
- [X] T024 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/ProgramCompositionTests.cs` para comprobar que la composición usa extensiones centralizadas y no contiene registros individuales, controllers, persistencia ni features de negocio.

### Implementación de US3

- [X] T025 [US3] Implementar el mapper centralizado `Result` a `ProblemDetails` en `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ResultProblemDetailsMapper.cs`, manteniendo el contrato de errores independiente de HTTP y los status codes definidos en `contracts/http-contracts.md`.
- [X] T026 [US3] Registrar `AddProblemDetails` y la configuración de errores esperados en `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ProblemDetailsServiceExtensions.cs`, sin exponer mensajes internos.
- [X] T027 [US3] Actualizar `app/backend/src/NetRentManagerApi/Program.cs` para limitarlo a configuración de servicios, ProblemDetails, logging, scanners y `MapSliceEndpoints`, sin lógica de negocio ni registros individuales.
- [X] T028 [US3] Añadir logging estructurado mediante `ILogger` en `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ProblemDetailsServiceExtensions.cs` y en los puntos de infraestructura donde se registren eventos, usando plantillas de logging y sin datos sensibles.

**Checkpoint**: los errores esperados tienen un contrato HTTP único y `Program.cs`
queda reducido a composición transversal.

---

## Fase 6: Pulido y validación transversal

**Propósito**: verificar la solución completa, registrar evidencia y comprobar las
restricciones negativas de la spec.

- [X] T029 Ejecutar y corregir los unit tests del backend con `dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore`, manteniendo todas las pruebas autorizadas en verde.
- [X] T030 [P] Ejecutar `dotnet build .\app\NetRentManager.sln --no-restore` y corregir cualquier error de compilación sin modificar `global.json` ni ampliar el alcance.
- [X] T031 [P] Inspeccionar `app/backend/src/NetRentManagerApi/` y confirmar que no existen entidades de dominio, `AppDbContext`, migraciones, seeders, conexiones de base de datos, controllers, handlers de negocio ni endpoints de producto.
- [X] T032 Registrar en `specs/002-foundation-backend/quickstart.md`, en `## Evidencia de validación`, la fecha `2026-09-10`, comandos ejecutados, resultados de build y tests, y la confirmación de restricciones negativas cumplidas.
- [X] T033 Confirmar que todas las tareas de `specs/002-foundation-backend/tasks.md` están marcadas `[X]`, que la spec permanece en `En implementación` hasta cumplir todos los gates y que existe evidencia suficiente para la transición final a `Implementada`.

**Checkpoint final**: build, tests, evidencia y revisión de alcance completados; solo
entonces puede ejecutarse la transición constitucional de cierre.

---

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Fase 1** no depende de otra fase y prepara referencias y directorios.
- **Fase 2** depende de T001-T003 y bloquea todas las historias.
- **US1** depende de T004, T007 y T008; sus tests T009-T010 deben existir antes de
  T011-T014.
- **US2** depende de T005, T007 y T008; sus tests T015-T017 deben existir antes de
  T018-T021.
- **US3** depende de T006-T007; sus tests T022-T024 deben existir antes de T025-T028.
- **Fase 6** depende de completar US1, US2 y US3.

### Dependencias entre historias

- **US1** y **US2** pueden implementarse en paralelo después de la Fase 2 porque
  trabajan en contratos y carpetas distintas, aunque ambas integran extensiones
  compartidas en T007/T021 y deben resolver ese mismo archivo secuencialmente.
- **US3** puede comenzar tras la Fase 2, pero T027 sobre `Program.cs` debe ejecutarse
  después de T014 y T021 para evitar conflictos de archivo.
- El MVP recomendado es Fase 1 + Fase 2 + US1; entrega discovery, mapeo y `/health`.

### Oportunidades paralelas

- T002-T003 pueden ejecutarse en paralelo con T001.
- T004-T006 y T008 pueden ejecutarse en paralelo después de la preparación.
- T009-T010 pueden ejecutarse en paralelo; T015-T017 y T022-T024 también.
- T011-T013 pueden ejecutarse en paralelo, y T018-T020 pueden ejecutarse en paralelo
  si T007 ya está terminado; las integraciones T014 y T021 son secuenciales.
- T022-T024 no modifican los mismos archivos y pueden ejecutarse en paralelo.
- T029-T031 son validaciones independientes, aunque T032 requiere sus resultados.

---

## Ejemplos de ejecución paralela

### US1

```text
T009 RegisterSlicesTests.cs
T010 HealthSliceTests.cs

Después de los tests:
T011 SliceRegistrationExtensions.cs
T012 SliceEndpointMappingExtensions.cs
T013 HealthSlice.cs
```

### US2

```text
T015 RegisterHandlersTests.cs
T016 ValidationFilterFactoryTests.cs
T017 ValidationProblemDetailsTests.cs

Después de los tests:
T018 HandlerRegistrationExtensions.cs
T019 ValidatorRegistrationExtensions.cs
T020 ValidationFilterFactory.cs
```

### US3

```text
T022-T023 ResultProblemDetailsMappingTests.cs
T024 ProgramCompositionTests.cs

Después de los tests:
T025 ResultProblemDetailsMapper.cs
T026 ProblemDetailsServiceExtensions.cs
T028 logging estructurado
```

---

## Estrategia de implementación

### MVP primero

1. Completar Fase 1 y Fase 2.
2. Completar US1 con pruebas de discovery, mapeo y `/health`.
3. Ejecutar tests y build del MVP.
4. Detenerse para validar que una futura incorporación de slice no requiere editar
   `Program.cs`.

### Entrega incremental

1. Agregar US2 y validar handlers y validators por scanning.
2. Agregar US3 y validar errores, ProblemDetails, logging y composición.
3. Ejecutar Fase 6 y completar evidencia en `quickstart.md`.
4. Cambiar la spec a `Implementada` únicamente mediante el flujo automático cuando
   no haya tareas pendientes y las validaciones estén documentadas.

### Criterios de finalización por historia

- **US1**: T009-T014 completadas; `RegisterSlices` es idempotente y `/health` se
  mapea sin registro individual.
- **US2**: T015-T021 completadas; handler y validator de prueba se descubren y el
  filtro cubre validación, pass-through y HTTP 400.
- **US3**: T022-T028 completadas; el mapper conserva contrato HTTP y `Program.cs`
  solo compone infraestructura.
- **Cierre**: T029-T033 completadas; `quickstart.md` contiene evidencia y no quedan
  tareas `[ ]`.
