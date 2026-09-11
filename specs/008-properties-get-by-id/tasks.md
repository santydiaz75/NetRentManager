---

description: "Tareas para la consulta de propiedad por id"
---

# Tareas: Consulta de Propiedad por Id

**Entrada**: Documentos de diseño de `specs/008-properties-get-by-id/`

**Prerequisitos**: `spec.md`, `plan.md`, `research.md`, `data-model.md`,
`contracts/backend-properties-get-by-id.openapi.yaml`, `quickstart.md`

**Pruebas**: Se incluyen pruebas unitarias porque la spec exige cubrir consulta
existente, URL absoluta, propiedad sin imagen, ids inválidos/inexistentes,
imágenes persistidas inconsistentes, cancelación, mapping y auto-descubrimiento.

**Regla de trazabilidad**: Todo código, contrato OpenAPI, prueba, cambio de archivo
HTTP y evidencia debe corresponder a una tarea de este archivo antes de marcarse
como completado.

## Fases y dependencias

- **Fase 1 - Preparación**: confirma scanners, persistencia y patrón público de 004.
- **Fase 2 - Fundamentos del contrato**: prepara proyección, response y OpenAPI.
- **Fase 3 - US1 P1**: consulta una propiedad existente con HTTP 200 y contrato completo; es el MVP.
- **Fase 4 - US2 P2**: cubre `imageUrl: null` y variaciones de esquema/host.
- **Fase 5 - US3 P3**: cubre 404, 500, cancelación y seguridad de URLs.
- **Fase 6 - Regresión y cierre**: integra archivo HTTP, build, tests, p95 y evidencia.

---

## Fase 1: Preparación compartida

**Propósito**: anclar la implementación a la arquitectura y contratos existentes,
sin crear migraciones ni registros manuales.

- [X] T001 [P] Revisar `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/ISlice.cs`, `app/backend/src/NetRentManagerApi/Infrastructure/Handlers/IHandler.cs` y los registradores existentes para confirmar auto-descubrimiento sin cambios manuales.
- [X] T002 [P] Revisar `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/PropertyListItem.cs` y `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesMapping.cs` para reutilizar los nueve campos públicos y el criterio de URL absoluta de 004.
- [X] T003 [P] Revisar `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/AppDbContext.cs`, `app/backend/src/NetRentManagerApi/Domain/Properties/Property.cs` y `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Configurations/PropertyConfiguration.cs` para confirmar que la consulta no requiere migración ni cambios de persistencia.
- [X] T004 [P] Revisar `app/backend/src/NetRentManagerApi/Program.cs` y `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ResultProblemDetailsMapper.cs` para confirmar `UseStaticFiles`, ProblemDetails y propagación de cancelación existentes.

**Checkpoint**: los contratos de 004, el modelo persistente y los scanners están
confirmados antes de crear el slice.

---

## Fase 2: Fundamentos del contrato y la proyección

**Propósito**: preparar contratos explícitos, proyección interna y documentación
OpenAPI antes de conectar la consulta.

- [X] T005 [P] Crear `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdRequest.cs` con el `Guid Id` recibido por la ruta restringida `{id:guid}`; un segmento no convertible debe resolverse como HTTP 404 por routing.
- [X] T006 [P] Crear `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdResponse.cs` con exactamente `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters` e `imageUrl`, sin `items`, `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext` ni `hasPrevious`.
- [X] T007 [P] Crear `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdRequestValidator.cs` con las reglas de binding del `Guid` sin inventar validación 400 para ids inválidos, porque el contrato exige HTTP 404 por restricción de ruta.
- [X] T008 [P] Actualizar `specs/008-properties-get-by-id/contracts/backend-properties-get-by-id.openapi.yaml` para mantener la ruta `/api/properties/{id}`, parámetro `id` obligatorio con `format: uuid`, response 200, ProblemDetails 404/500 y el schema de los nueve campos públicos.
- [X] T009 Crear `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs` con una proyección interna de los nueve campos, `status` textual y una salida `imageUrl` nullable; no incluir acceso a datos ni lógica de consulta.

**Checkpoint**: los contratos, schema OpenAPI y tipos de entrada/salida expresan
el alcance unitario sin paginación.

---

## Fase 3: Historia de Usuario 1 - Consultar una propiedad existente (P1) - MVP

**Objetivo**: devolver HTTP 200 con el contrato público completo de 004 para un
`Guid` existente, sin exponer entidades EF ni metadatos de paginación.

**Prueba independiente**: persistir una propiedad con imagen válida, ejecutar el
handler/endpoint con una request `https://api.example.test:8443` y comprobar los
nueve campos, `status` textual y `imageUrl` absoluta bajo
`https://api.example.test:8443/assets/properties/{fileName}`.

### Pruebas de US1

- [X] T010 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdMappingTests.cs` para verificar los nueve campos, `status` como texto, ausencia de metadatos de paginación y URL absoluta con esquema `https` y host con puerto.
- [X] T011 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdHandlerTests.cs` con EF Core InMemory para comprobar proyección de una propiedad existente, `AsNoTracking`, respuesta explícita y HTTP lógico de éxito.
- [X] T012 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdSliceTests.cs` para comprobar que el slice es público, implementa `ISlice`, el handler implementa `IHandler` y la ruta declarada es `GET /api/properties/{id:guid}`.

### Implementación de US1

- [X] T013 [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs` con una proyección `AsNoTracking` de `Property` y `SingleOrDefaultAsync(id, cancellationToken)`, devolviendo el contrato explícito sin exponer la entidad EF.
- [X] T014 [US1] Completar `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs` para copiar exactamente los nueve campos públicos de 004, convertir `PropertyStatus` a texto y construir `imageUrl` como URL absoluta con `Request.Scheme`, `Request.Host` y `/assets/properties/{fileName}`.
- [X] T015 [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs` con `GET /api/properties/{id:guid}`, `Produces` para HTTP 200/404/500, response JSON explícito, resolución del handler auto-registrado y delegación mediante `ResultProblemDetailsMapper`.
- [X] T016 [US1] Integrar `GetPropertyByIdRequest`, `GetPropertyByIdHandler`, `GetPropertyByIdMapping` y `GetPropertyByIdResponse` sin añadir registros manuales en `app/backend/src/NetRentManagerApi/Program.cs` ni exponer `Property` directamente.
- [X] T017 [US1] Ejecutar las pruebas de `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/` correspondientes a US1 y corregir únicamente archivos trazados por esta fase.

**Checkpoint**: el MVP devuelve una propiedad existente con HTTP 200, contrato
completo y URL absoluta pública, sin paginación.

---

## Fase 4: Historia de Usuario 2 - Consultar propiedad sin imagen (P2)

**Objetivo**: aceptar propiedades válidas con `ImageUrl == null` y conservar
`imageUrl: null` sin generar URLs relativas, físicas o inventadas.

**Prueba independiente**: consultar una propiedad sin imagen con requests `http`
y `https`, hosts distintos y un host con puerto; comprobar HTTP 200, los nueve
campos y `imageUrl: null`.

### Pruebas de US2

- [X] T018 [P] [US2] Ampliar `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdMappingTests.cs` para cubrir `ImageUrl == null`, esquemas `http`/`https`, hosts variables y host con puerto sin construir una URL para la imagen.
- [X] T019 [P] [US2] Ampliar `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdHandlerTests.cs` para comprobar que una propiedad persistida sin imagen devuelve HTTP lógico 200 y todos los campos públicos con `imageUrl: null`.
- [X] T020 [US2] Confirmar mediante `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdMappingTests.cs` que la respuesta no serializa `items` ni metadata de paginación.

### Implementación de US2

- [X] T021 [US2] Completar `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs` para devolver `imageUrl: null` cuando `ImageUrl` sea nula, manteniendo HTTP 200 y sin cambiar esquema, host ni datos persistidos.
- [X] T022 [US2] Ejecutar las pruebas focalizadas de `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/` para US1 y US2 y confirmar que la URL absoluta de imágenes existentes no se rompe al admitir propiedades sin imagen.

**Checkpoint**: las propiedades con y sin imagen tienen respuestas 200 válidas,
compatibles con 004 y sin metadata de paginación.

---

## Fase 5: Historia de Usuario 3 - Ids inválidos y datos inconsistentes (P3)

**Objetivo**: devolver 404 para ids inválidos o ausentes, 500 para referencias de
imagen inseguras y propagar cancelación sin filtrar detalles internos.

**Prueba independiente**: consultar segmento no GUID, GUID inexistente, imagen con
ruta física/`support`/traversal y request cancelada; comprobar 404/500, ausencia de
rutas internas y propagación del token.

### Pruebas de US3

- [X] T023 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdErrorTests.cs` para GUID válido inexistente con `Error.NotFound`, imagen `support`, ruta física, URI absoluta, traversal, nombre vacío y HTTP lógico 500 sin revelar la ruta.
- [X] T024 [P] [US3] Añadir a `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdSliceTests.cs` la comprobación de la restricción `{id:guid}` y documentar que el segmento `no-es-guid` responde HTTP 404 por routing.
- [X] T025 [P] [US3] Añadir a `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdHandlerTests.cs` una prueba de cancelación que verifique propagación de `OperationCanceledException` y uso del `CancellationToken` en la consulta.
- [X] T026 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/GetPropertyByIdRequestValidatorTests.cs` para confirmar que el caso de uso no transforma ids inválidos en HTTP 400 y que la responsabilidad corresponde a la restricción de ruta.

### Implementación de US3

- [X] T027 [US3] Completar `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdHandler.cs` para devolver `Error.NotFound("properties.not_found", ...)` cuando `SingleOrDefaultAsync` no encuentre el GUID y mantener el resultado sin datos de otras propiedades.
- [X] T028 [US3] Completar `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdMapping.cs` para rechazar rutas no nulas que no comiencen por `/assets/properties/`, contengan `support`, `..`, URI absoluta, ruta física o nombre inseguro, devolviendo `Error.Internal` sin respuesta parcial.
- [X] T029 [US3] Completar `app/backend/src/NetRentManagerApi/Features/Properties/GetPropertyById/GetPropertyByIdSlice.cs` para propagar `CancellationToken` y traducir los resultados mediante `ResultProblemDetailsMapper` a HTTP 404/500 sin respuestas manuales.
- [X] T030 [US3] Verificar mediante `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ResultProblemDetailsMapper.cs` y sus pruebas existentes que `Error.NotFound` continúa en HTTP 404 y `Error.Internal` en HTTP 500 sin exponer rutas ni stack traces.
- [X] T031 [US3] Ejecutar la suite focalizada de `app/backend/tests/NetRentManagerApiTests/Features/Properties/GetPropertyById/` para US3 y confirmar ausencia de respuesta parcial, datos de otras propiedades y continuación tras cancelación.

**Checkpoint**: los tres resultados negativos están definidos y probados: 404 por
ruta/recurso ausente, 500 por imagen insegura y cancelación propagada.

---

## Fase 6: Regresión, contrato manual, rendimiento y cierre

**Propósito**: verificar compatibilidad con 004, documentar los cuatro escenarios
manuales y cerrar la iniciativa solo con evidencia completa.

- [X] T032 [P] Actualizar `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` con consultas de propiedad con imagen, sin imagen, GUID inexistente e id `no-es-guid`, siguiendo `specs/008-properties-get-by-id/quickstart.md`.
- [X] T033 [P] Revisar `specs/008-properties-get-by-id/contracts/backend-properties-get-by-id.openapi.yaml` contra el endpoint y confirmar schemas de response 200, ProblemDetails 404/500 y ausencia de paginación.
- [X] T034 Ejecutar `dotnet build .\app\NetRentManager.sln --no-restore` para `app/NetRentManager.sln` y corregir únicamente errores en archivos trazados por esta iniciativa.
- [X] T035 Ejecutar `dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore` para `app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` y confirmar la suite existente más las pruebas US1, US2, US3, errores, cancelación y mapping.
- [X] T036 Ejecutar los cuatro escenarios de `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` y `specs/008-properties-get-by-id/quickstart.md` y registrar HTTP 200 con/sin imagen, HTTP 404 para GUID inexistente e id inválido y HTTP 500 para imagen inconsistente cubierto por pruebas.
- [X] T037 Medir en el entorno documentado de `specs/008-properties-get-by-id/quickstart.md` al menos 100 consultas válidas, calcular p95 de 176.37 ms y confirmar 100 respuestas HTTP 200, por debajo de 250 ms.
- [X] T038 Inspeccionar `app/backend/src/NetRentManagerApi/` y confirmar que no se agregaron controllers, migraciones, filtros, paginación, registros manuales, entidades EF en responses, URLs relativas ni cambios de frontend.
- [X] T039 Registrar en `specs/008-properties-get-by-id/quickstart.md`, en `## Evidencia de validación`, fecha, comandos, cantidad de pruebas, URLs absolutas, códigos HTTP, cancelación, auto-descubrimiento y p95.
- [X] T040 Confirmar que todas las tareas de `specs/008-properties-get-by-id/tasks.md` estén marcadas `[X]` solo después de build, tests, escenarios, OpenAPI, medición p95 y evidencia completa; mantener la spec conforme al flujo Speckit.

**Checkpoint final**: el endpoint individual, contrato OpenAPI, URL absoluta,
ausencia válida de imagen, errores, cancelación, rendimiento y evidencia están
completos y trazados.

---

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Fase 1** no depende de otras fases.
- **Fase 2** depende de T001-T004 y bloquea todas las historias porque fija el
  contrato unitario, OpenAPI y proyección.
- **US1** depende de T005-T009 y entrega el MVP de consulta con imagen.
- **US2** depende de US1 porque extiende el mapping y pruebas del mismo response.
- **US3** depende de US1 y US2 porque prueba los errores y la cancelación del flujo.
- **Fase 6** depende de US1, US2 y US3.

### Dependencias de historias

- **US1** es el MVP y establece la consulta 200 y el contrato de nueve campos.
- **US2** depende de US1 para conservar el mismo response y añadir `imageUrl: null`.
- **US3** depende de US1/US2 para validar ausencia, inconsistencia, routing y cancelación.

### Oportunidades paralelas

- T001-T004 pueden ejecutarse en paralelo porque inspeccionan superficies distintas.
- T005-T009 pueden prepararse en paralelo; T013-T016 integran el slice después.
- T010-T012 pueden ejecutarse en paralelo en archivos de prueba distintos.
- T018-T020 pueden ejecutarse en paralelo; T021 depende del contrato base.
- T023-T026 pueden ejecutarse en paralelo; T027-T030 integran el comportamiento.
- T032-T033 pueden ejecutarse en paralelo; T034-T040 son validaciones secuenciales.

---

## Ejemplos de ejecución paralela

### US1 - MVP

```text
T010 GetPropertyByIdMappingTests.cs
T011 GetPropertyByIdHandlerTests.cs
T012 GetPropertyByIdSliceTests.cs

Después de preparar las pruebas:
T013 GetPropertyByIdHandler.cs
T014 GetPropertyByIdMapping.cs
T015 GetPropertyByIdSlice.cs
T016 integración con scanners
T017 pruebas focalizadas P1
```

### US2 - Propiedad sin imagen

```text
T018 pruebas de mapping http/https/host/puerto
T019 pruebas de handler con imageUrl null
T020 pruebas de ausencia de paginación

Después de US1:
T021 mapping imageUrl null
T022 pruebas focalizadas US1/US2
```

### US3 - Errores y cancelación

```text
T023 pruebas de imágenes inseguras y 404
T024 prueba de restricción Guid
T025 prueba de cancelación
T026 prueba del validator de routing

Después de preparar las pruebas:
T027 handler NotFound
T028 mapping seguro
T029 slice y ProblemDetails
T030 mapper común
T031 suite focalizada US3
```

---

## Estrategia de implementación

### MVP primero: solo Historia de Usuario 1

1. Completar Fase 1 y Fase 2.
2. Implementar US1 para consulta de propiedad existente con imagen.
3. Ejecutar las pruebas independientes de US1.
4. Detenerse para validar HTTP 200, los nueve campos y URL absoluta pública.

### Entrega incremental

1. Completar preparación y contratos.
2. Entregar US1 como MVP.
3. Añadir US2 para propiedades sin imagen y hosts variables.
4. Añadir US3 para 404, 500, routing seguro y cancelación.
5. Ejecutar cierre, OpenAPI, escenarios manuales, p95 y evidencia.

### Notas de ejecución

- Las tareas `[P]` solo son paralelizables cuando usan archivos distintos y sus
  dependencias anteriores están completas.
- No crear migraciones ni modificar `Property`, configuración, seeding o assets.
- Cada response debe ser explícito y no debe serializar entidades EF.
- No marcar `[X]` hasta verificar la tarea y registrar evidencia cuando corresponda.
