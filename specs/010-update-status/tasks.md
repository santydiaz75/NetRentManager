---

description: "Lista de tareas para implementar la actualización de estado de una propiedad"
---

# Tareas: Actualización de Estado de Propiedad

**Entrada**: Documentos de diseño de `specs/010-update-status/`

**Prerrequisitos**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/http-contracts.md` y `quickstart.md`

**Organización**: Las tareas están agrupadas por historia de usuario para permitir implementación y validación independientes.

## Dependencias y orden de ejecución

- `009-open-api` debe haber generado el documento base antes de validar el drift del nuevo endpoint.
- La Fase 1 confirma rutas, dependencias y contrato existente.
- La Fase 2 prepara la estructura de pruebas y bloquea las historias.
- US1 y US2 son el núcleo P1; US2 depende del contrato de US1, pero sus pruebas de validator pueden prepararse en paralelo.
- US3 depende de la implementación del endpoint para validar OpenAPI y el ejemplo `.http`.
- La Fase final depende de todas las historias y de la evidencia de `quickstart.md`.

## Fase 1: Preparación

**Propósito**: Confirmar la base existente sin modificar persistencia ni endpoints anteriores.

- [X] T001 [P] Confirmar en `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` que el proyecto usa .NET 10, generación OpenAPI en build y las dependencias existentes de EF Core, FluentValidation y ProblemDetails.
- [X] T002 [P] Confirmar en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Configurations/PropertyConfiguration.cs` y `app/backend/src/NetRentManagerApi/Domain/Properties/PropertyStatus.cs` que `Status` usa el enum cerrado `Available`, `Rented`, `Maintenance` y no requiere migración.
- [X] T003 [P] Revisar `specs/010-update-status/contracts/http-contracts.md` y fijar en las pruebas la ruta `PATCH /api/properties/{id}/status`, el cuerpo JSON estricto y las respuestas 200/400/404/500.
- [X] T004 [P] Confirmar en `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/SliceRegistrationExtensions.cs` y `app/backend/src/NetRentManagerApi/Infrastructure/Handlers/HandlerRegistrationExtensions.cs` los mecanismos de auto-descubrimiento de `ISlice`, `IHandler` y validators.

## Fase 2: Fundamentos

**Propósito**: Preparar la estructura del slice y las pruebas del caso de uso.

- [X] T005 [P] Crear el directorio `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus/` siguiendo la convención de slices existente.
- [X] T006 [P] Crear el directorio `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdatePropertyStatus/` y preparar fixtures xUnit con EF Core InMemory.
- [X] T007 [P] Revisar `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ResultProblemDetailsMapper.cs` para reutilizar los mapeos vigentes de 400, 404 y 500 sin crear errores paralelos.

**Punto de control**: La estructura está lista y las pruebas pueden ejecutarse sin modificar el modelo de base de datos.

## Fase 3: Historia de Usuario 1 - Actualizar el estado de una propiedad (Prioridad: P1) 🎯 MVP

**Objetivo**: Implementar el flujo válido que cambia únicamente `Status` y devuelve la propiedad completa conservando todos los demás datos.

**Prueba independiente**: Preparar una propiedad con `imageUrl` y datos conocidos, enviar un estado válido en distintas capitalizaciones y verificar `200 OK`, valor canónico y conservación de campos.

### Pruebas de la Historia de Usuario 1

- [X] T008 [P] [US1] Crear en `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusHandlerTests.cs` pruebas de actualización válida para `Available`, `Rented` y `Maintenance`, comprobando que solo cambia `Status` y que `imageUrl` permanece intacta.
- [X] T009 [P] [US1] Crear en `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusMappingTests.cs` pruebas del mapping hacia el response completo con `status` textual e `imageUrl` nullable.

### Implementación de la Historia de Usuario 1

- [X] T010 [P] [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusRequest.cs` con el campo obligatorio `status` y sin propiedades de otros campos ni archivos.
- [X] T011 [P] [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusResponse.cs` con `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`.
- [X] T012 [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusMapping.cs` para transformar explícitamente la entidad actualizada al response, conservando todos los campos no modificados.
- [X] T013 [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusHandler.cs` con búsqueda por id, normalización case-insensitive al enum canónico, mutación exclusiva de `Property.Status`, `SaveChangesAsync(cancellationToken)` y respuesta `Result`.
- [X] T014 [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusSlice.cs` con `PATCH /api/properties/{id}/status`, tags y metadatos de respuestas OpenAPI, usando auto-descubrimiento de `ISlice` e `IHandler`.
- [X] T015 [US1] Ejecutar las pruebas de handler y mapping en `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdatePropertyStatus/` y confirmar que el flujo válido devuelve 200 sin alterar otros campos.

**Punto de control**: US1 permite actualizar únicamente el estado y devuelve la propiedad completa con datos conservados.

## Fase 4: Historia de Usuario 2 - Rechazar solicitudes inválidas (Prioridad: P1)

**Objetivo**: Proteger el contrato con validación estricta, respuestas ProblemDetails y ausencia de cambios parciales.

**Prueba independiente**: Ejecutar solicitudes con estado ausente, nulo, vacío, inválido, propiedades adicionales, id inexistente, GUID inválido y persistencia fallida.

### Pruebas de la Historia de Usuario 2

- [X] T016 [P] [US2] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusRequestValidatorTests.cs` con casos de estado ausente, nulo, vacío, inválido, capitalización alternativa y propiedades adicionales rechazadas.
- [X] T017 [P] [US2] Añadir en `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusHandlerTests.cs` pruebas de id inexistente 404, persistencia fallida 500, cancelación propagada e idempotencia del mismo estado.
- [X] T018 [P] [US2] Añadir en `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusContractTests.cs` pruebas unitarias sobre el binding/contrato para GUID inválido, body JSON incompatible, ausencia de `status`, campos adicionales y códigos 400/404/500, sin `WebApplicationFactory` ni infraestructura HTTP externa.

### Implementación de la Historia de Usuario 2

- [X] T019 [US2] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusRequestValidator.cs` con reglas de campo obligatorio, estado permitido case-insensitive y rechazo de propiedades JSON adicionales con 400.
- [X] T020 [US2] Completar en `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusHandler.cs` el manejo de entidad inexistente, cancelación y excepciones inesperadas mediante `ProblemDetails`, sin modificar la entidad si la validación falla.
- [X] T021 [US2] Completar en `app/backend/src/NetRentManagerApi/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusSlice.cs` los metadatos `.ProducesProblem` para 400, 404 y 500, sin aceptar multipart ni campos de actualización general.
- [X] T022 [US2] Ejecutar las pruebas de validator, handler y contrato de US2 y verificar que los errores no alteran `Status`, `imageUrl` ni los demás campos.

**Punto de control**: Las solicitudes inválidas producen 400/404/500 coherentes y nunca actualizan parcialmente la propiedad.

## Fase 5: Historia de Usuario 3 - Consumir el contrato y probarlo manualmente (Prioridad: P2)

**Objetivo**: Exponer el contrato OpenAPI correcto y un ejemplo `.http` ejecutable sin campos adicionales ni archivos.

**Prueba independiente**: Compilar el backend, inspeccionar el documento OpenAPI y ejecutar el ejemplo PATCH del archivo HTTP con un id válido.

### Pruebas de la Historia de Usuario 3

- [X] T023 [P] [US3] Añadir en `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusContractTests.cs` una prueba unitaria que lea el JSON OpenAPI generado y compruebe exactamente `PATCH /api/properties/{id}/status`, body solo con `status` y respuestas 200/400/404/500.
- [X] T024 [P] [US3] Añadir en `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdatePropertyStatus/UpdatePropertyStatusContractTests.cs` una prueba unitaria que confirme que el contrato OpenAPI no contiene `title`, `price`, `imageUrl`, `image` ni multipart como entrada.

### Implementación de la Historia de Usuario 3

- [X] T025 [US3] Añadir en `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` un ejemplo válido de `PATCH /api/properties/{id}/status` con `Content-Type: application/json` y un cuerpo que contenga únicamente `status`.
- [X] T026 [US3] Ejecutar las pruebas OpenAPI y el ejemplo manual documentado en `app/backend/src/NetRentManagerApi/NetRentManagerApi.http`, registrando la evidencia en `specs/010-update-status/quickstart.md`.

**Punto de control**: El endpoint está documentado, el contrato es estricto y el ejemplo HTTP es ejecutable.

## Fase 6: Pulido y preocupaciones transversales

**Propósito**: Cerrar validación, regeneración del contrato y trazabilidad.

- [X] T027 [P] Ejecutar `dotnet build app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj`, comprobar que no aparece ningún archivo nuevo bajo `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Migrations/` y verificar que no se crean archivos nuevos bajo `app/backend/src/NetRentManagerApi/wwwroot/assets/properties/`.
- [X] T028 [P] Ejecutar `dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` y registrar el resultado final en `specs/010-update-status/quickstart.md`.
- [X] T029 Regenerar y validar contrato OpenAPI v1: ejecutar `support/scripts/generate-openapi-v1.ps1` y versionar `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json`.
- [X] T030 [P] Revisar `specs/010-update-status/checklists/requirements.md` y confirmar que cada requisito queda cubierto por una tarea completada, sin modificar los marcadores del checklist durante la implementación.
- [X] T031 Confirmar en `specs/010-update-status/tasks.md` que todas las tareas completadas están marcadas `[X]` y que no queda ninguna tarea sin trazabilidad antes de cerrar la spec.
- [X] T032 Ajustar `.github/workflows/openapi-contract.yml` para ejecutar las pruebas con `OpenApiGenerateDocumentsOnBuild=false`, evitando que el build de tests sobrescriba el contrato después de su regeneración y validación.

## Dependencias y oportunidades de paralelización

### Dependencias por fase

- Fase 1 no depende de otras fases.
- Fase 2 depende de Fase 1 y bloquea las historias.
- US1 depende de Fase 2.
- US2 depende de US1 para reutilizar request, response, mapping y slice, aunque T016 puede prepararse en paralelo con US1.
- US3 depende de US1 y US2 para validar el contrato final y el ejemplo `.http`.
- Fase 6 depende de todas las historias y de la evidencia actualizada.

### Oportunidades paralelas

- T001-T004 pueden ejecutarse en paralelo porque inspeccionan contratos y registros distintos.
- T005-T007 pueden ejecutarse en paralelo porque crean estructura o revisan infraestructura diferente.
- T008-T012 pueden ejecutarse en paralelo cuando no haya edición simultánea del mismo archivo.
- T016-T018 pueden prepararse en paralelo, coordinando T017 con el archivo del handler.
- T023-T024 comparten el mismo archivo de contrato y conviene consolidarlas.
- T027, T030 pueden ejecutarse en paralelo; T028 y T029 deben ejecutarse después de la implementación completa.

## Ejemplos de ejecución paralela

### Historia de Usuario 1

```text
T008: pruebas del handler en UpdatePropertyStatusHandlerTests.cs
T009: pruebas del mapping en UpdatePropertyStatusMappingTests.cs
T010-T012: request, response y mapping en archivos distintos
```

### Historia de Usuario 2

```text
T016: validator tests
T017: handler error tests
T018: contract/error tests
```

### Historia de Usuario 3

```text
T023-T024: pruebas del documento OpenAPI en UpdatePropertyStatusContractTests.cs
T025: ejemplo manual en NetRentManagerApi.http
```

## Estrategia de implementación

### MVP: Historias de Usuario 1 y 2

1. Completar Fase 1 y Fase 2.
2. Implementar US1 y US2.
3. Ejecutar pruebas de éxito, conservación, validación, 404 y 500.
4. Detenerse para validar que el endpoint cambia solo `Status`.

### Entrega incremental

1. US1: actualización válida y conservación de datos.
2. US2: errores, cancelación, idempotencia y rechazo de campos adicionales.
3. US3: contrato OpenAPI y ejemplo manual.
4. Fase 6: build, suite, regeneración de OpenAPI y cierre trazable.

## Criterios de prueba independientes

- **US1**: Una propiedad existente cambia solo `Status`, normaliza la capitalización y conserva `imageUrl` y demás campos.
- **US2**: Los casos inválidos producen 400/404/500 según corresponda y no cambian datos.
- **US3**: OpenAPI y `.http` describen y ejecutan exactamente el PATCH con solo `status`.

## Notas de trazabilidad

- No se crean entidades, migraciones ni cambios de esquema.
- No se agregan imágenes, multipart ni cambios a endpoints existentes.
- Cada cambio de código, prueba, contrato y ejemplo queda asociado a una tarea concreta.

