---

description: "Tareas para el alta de propiedades con imagen opcional"
---

# Tareas: Alta de Propiedades

**Entrada**: Documentos de diseño de `specs/006-properties-create/`

**Prerequisitos**: `spec.md`, `plan.md`, `research.md`, `data-model.md`,
`contracts/http-contracts.md`, `quickstart.md`

**Pruebas**: Se incluyen pruebas unitarias porque la spec exige validar creación
sin imagen, uploads PNG/JPG, límites, magic bytes, cancelación, limpieza,
migración y regresión del listado 004.

**Regla de trazabilidad**: Todo código, migración, prueba, contrato y cambio de
asset debe corresponder a una tarea de este archivo antes de marcarse como
completado.

## Fases y dependencias

- **Fase 1 - Preparación**: prepara directorios y confirma scanners existentes.
- **Fase 2 - Fundamentos persistentes**: hace nullable `ImageUrl` y genera la migración.
- **Fase 3 - US1**: crea propiedades válidas sin imagen. Es el MVP.
- **Fase 4 - US2**: agrega uploads PNG/JPG, nombres seguros y URLs públicas.
- **Fase 5 - US3**: completa validación, errores, cancelación y compensación.
- **Fase 6 - Regresión y pulido**: verifica listado 004, migración, build, tests y evidencia.

---

## Fase 1: Preparación compartida

**Propósito**: preparar la estructura sin añadir proyectos, dependencias o registros
manuales fuera del plan.

- [X] T001 [P] Crear `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty` para el nuevo slice.
- [X] T002 [P] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty` para las pruebas del caso de uso.
- [X] T003 [P] Revisar `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/SliceRegistrationExtensions.cs`, `app/backend/src/NetRentManagerApi/Infrastructure/Handlers/HandlerRegistrationExtensions.cs` y `app/backend/src/NetRentManagerApi/Infrastructure/Validation/ValidatorRegistrationExtensions.cs` para confirmar auto-descubrimiento sin registros manuales.
- [X] T004 [P] Revisar `app/backend/src/NetRentManagerApi/Program.cs` y la raíz runtime de `wwwroot/assets/properties` para confirmar que los uploads se sirven por `/assets/properties/`.

**Checkpoint**: la feature y sus pruebas tienen estructura preparada, y los scanners
existentes son suficientes.

---

## Fase 2: Fundamentos persistentes bloqueantes

**Propósito**: habilitar `imageUrl: null` y dejar la persistencia lista antes del
caso de uso.

- [X] T005 [P] Cambiar `ImageUrl` a `string?` en `app/backend/src/NetRentManagerApi/Domain/Properties/Property.cs` sin modificar otros campos persistentes.
- [X] T006 [P] Actualizar `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Configurations/PropertyConfiguration.cs` para configurar `image_url` nullable y conservar su longitud máxima.
- [X] T007 [P] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/PropertyImageNullabilityTests.cs` para comprobar que el modelo acepta `ImageUrl = null` y mantiene requeridos los demás campos.
- [X] T008 Generar la migración única de esta iniciativa en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Migrations/` mediante `dotnet ef migrations add`, sin modificar la migración histórica de la spec 003.
- [X] T009 Revisar `Up`, `Down` y snapshot de la migración de `ImageUrl` nullable en `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Migrations/` y verificar que solo altera `image_url`.

**Checkpoint**: el modelo, la configuración, la migración y la prueba de nulabilidad
están listas antes de implementar endpoints.

---

## Fase 3: Historia de Usuario 1 - Crear sin imagen (P1) - MVP

**Objetivo**: crear una propiedad válida mediante multipart sin archivo y persistir
`imageUrl: null` con respuesta HTTP 201.

**Prueba independiente**: enviar un formulario válido sin `image` y comprobar el
contrato 201, `Guid Id`, `status` textual, `Location` y `imageUrl: null`.

### Pruebas de US1

- [X] T010 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty/CreatePropertyRequestValidatorTests.cs` para validar campos obligatorios, `status` explícito y estados permitidos.
- [X] T011 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty/CreatePropertyMappingTests.cs` para verificar mapping explícito de la entidad a `CreatePropertyResponse`, incluyendo `imageUrl: null` y `status` textual.
- [X] T012 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty/CreatePropertyHandlerTests.cs` con EF Core InMemory para comprobar persistencia válida sin imagen y títulos repetidos con `Guid Id` distintos.

### Implementación de US1

- [X] T013 [P] [US1] Crear `CreatePropertyRequest` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyRequest.cs` con campos multipart y `IFormFile? Image` opcional.
- [X] T014 [P] [US1] Crear `CreatePropertyResponse` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyResponse.cs` con campos persistidos, `status` textual e `imageUrl` nullable.
- [X] T015 [US1] Crear `CreatePropertyMapping` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyMapping.cs` sin acceso a datos, I/O ni reglas de upload.
- [X] T016 [US1] Crear `CreatePropertyRequestValidator` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyRequestValidator.cs` con FluentValidation para campos, rangos y `status` obligatorio, usando auto-descubrimiento.
- [X] T017 [US1] Crear `CreatePropertyHandler` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs` para generar `Guid`, crear `Property` sin imagen y ejecutar `SaveChangesAsync(cancellationToken)`.
- [X] T018 [US1] Crear `CreatePropertySlice` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertySlice.cs` con `POST /api/properties`, binding multipart, resolución de `IHandler` y delegación sin modificar los scanners.
- [X] T019 [US1] Mapear el resultado exitoso del slice a HTTP 201 `Results.Created` con `Location: /api/properties/{id}` y errores mediante `ResultProblemDetailsMapper`.

**Checkpoint**: el MVP crea sin imagen, devuelve 201 y no inserta una ruta ficticia.

---

## Fase 4: Historia de Usuario 2 - Crear con imagen válida (P2)

**Objetivo**: aceptar PNG/JPG/JPEG reales hasta 5 MiB, almacenarlos con nombre
seguro y persistir una URL relativa pública.

**Prueba independiente**: crear con un PNG y un JPG válidos, comprobar archivos
existentes bajo `wwwroot/assets/properties` y URLs no físicas bajo `/assets/properties/`.

### Pruebas de US2

- [X] T020 [P] [US2] Ampliar `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty/CreatePropertyHandlerTests.cs` para cubrir PNG, JPG/JPEG, nombre UUID, URL relativa y no sobrescritura con nombres originales iguales.
- [X] T021 [P] [US2] Añadir a `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty/CreatePropertyMappingTests.cs` la serialización de `status` textual y `imageUrl` bajo `/assets/properties/`.
- [X] T022 [P] [US2] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty/CreatePropertySliceTests.cs` para verificar que el endpoint es público, implementa `ISlice`, se auto-descubre y registra `POST /api/properties`.

### Implementación de US2

- [X] T023 [US2] Completar `CreatePropertyHandler` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs` para validar tamaño máximo de `5 * 1024 * 1024`, extensión/MIME auxiliar y magic bytes PNG/JPEG antes de escribir.
- [X] T024 [US2] Completar `CreatePropertyHandler` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs` para generar nombre UUID con extensión normalizada y escribir por stream asíncrono al WebRoot runtime.
- [X] T025 [US2] Completar `CreatePropertyMapping` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyMapping.cs` para construir explícitamente `CreatePropertyResponse` con URL relativa nullable y `status` textual.
- [X] T026 [US2] Completar `CreatePropertySlice` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertySlice.cs` para recibir `image` opcional y preservar el binding de campos multipart.

**Checkpoint**: PNG/JPG válidos crean propiedades con archivo público, URL segura y
respuesta 201.

---

## Fase 5: Historia de Usuario 3 - Validación y ausencia de efectos parciales (P3)

**Objetivo**: rechazar entradas inválidas con HTTP 400 y limpiar archivos ante
fallos de almacenamiento, persistencia o cancelación.

**Prueba independiente**: ejecutar casos inválidos y fallos simulados, comprobando
400/500, ausencia de registros parciales y ausencia de archivos huérfanos.

### Pruebas de US3

- [X] T027 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty/CreatePropertyErrorTests.cs` para formatos no permitidos, archivo vacío, magic bytes incompatibles, tamaño mayor de 5 MiB y formularios mal formados.
- [X] T028 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty/CreatePropertyCleanupTests.cs` para simular fallos de almacenamiento/persistencia, verificar limpieza compensatoria y HTTP 500 sin detalles internos.
- [X] T029 [P] [US3] Añadir pruebas de cancelación a `app/backend/tests/NetRentManagerApiTests/Features/Properties/CreateProperty/CreatePropertyCleanupTests.cs` para comprobar cancelación durante lectura/escritura y limpieza del archivo generado.

### Implementación de US3

- [X] T030 [US3] Completar `CreatePropertyRequestValidator` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyRequestValidator.cs` para devolver errores por campo en solicitudes inválidas sin escribir archivos ni persistir datos.
- [X] T031 [US3] Completar `CreatePropertyHandler` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs` con `try/catch` controlado, `Error.Internal`, logging estructurado y eliminación compensatoria del archivo creado.
- [X] T032 [US3] Completar `CreatePropertySlice` en `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertySlice.cs` para mapear validaciones a HTTP 400, fallos internos a HTTP 500 y propagar `CancellationToken`.
- [X] T033 [US3] Verificar que `app/backend/src/NetRentManagerApi/Program.cs` mantiene `UseStaticFiles`, la raíz WebRoot runtime y el orden de migración sin agregar registros manuales del nuevo slice.

**Checkpoint**: las entradas inválidas producen 400, los fallos internos 500, y no
quedan propiedades parciales ni archivos públicos huérfanos.

---

## Fase 6: Regresión y validación transversal

**Propósito**: preservar el listado 004, revisar migración, ejecutar build/tests y
documentar evidencia.

- [X] T034 [P] [US3] Ajustar `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesMapping.cs` para devolver `imageUrl: null` cuando `Property.ImageUrl` sea null y mantener error 500 solo para rutas no nulas inválidas.
- [X] T035 [P] [US3] Añadir prueba de regresión en `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesHandlerTests.cs` para listar una propiedad sin imagen sin omitirla ni devolver HTTP 500.
- [X] T036 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Persistence/PropertyImageNullabilityTests.cs` para comprobar la columna nullable, la migración y la conservación de imágenes existentes.
- [X] T037 [P] Actualizar `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` con ejemplos multipart de creación sin imagen y con PNG.
- [X] T038 Ejecutar `dotnet build .\app\NetRentManager.sln --no-restore` y corregir errores exclusivamente en archivos trazados por esta iniciativa.
- [X] T039 Ejecutar `dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore` y confirmar la suite existente más las pruebas nuevas de US1, US2, US3 y regresión 004.
- [X] T040 [P] Inspeccionar `app/backend/src/NetRentManagerApi/` para confirmar ausencia de controllers, registros manuales, mappers automáticos, rutas físicas persistidas y cambios fuera del alcance.
- [X] T041 [P] Ejecutar los ejemplos de `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` o `quickstart.md` y verificar respuestas 201, 400/500, `imageUrl: null`, archivo público y `Location`.
- [X] T042 Registrar en `specs/006-properties-create/quickstart.md`, en `## Evidencia de validación`, fecha, migración, comandos, cantidad de pruebas, resultados HTTP y comprobación de limpieza.
- [X] T043 Confirmar que todas las tareas de `specs/006-properties-create/tasks.md` están marcadas `[X]` solo después de validar build, tests, migración, regresión y quickstart; mantener la spec en `En implementación` hasta la transición autorizada.

**Checkpoint final**: el alta sin imagen, el upload seguro, los errores, la migración,
la regresión del listado y la evidencia están completos y trazados.

---

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Fase 1** no depende de otras fases.
- **Fase 2** depende de T001-T004 y bloquea las historias porque `imageUrl` debe aceptar null.
- **US1** depende de T005-T009 y es el MVP funcional.
- **US2** depende de US1 y agrega escritura de archivos y URLs públicas.
- **US3** depende de US1/US2 y completa fallos, cancelación y validación negativa.
- **Fase 6** depende de US1, US2 y US3.

### Dependencias de historias

- **US1** entrega el alta sin imagen y los contratos base.
- **US2** depende de US1 porque reutiliza request, response, handler y slice.
- **US3** depende de US2 para probar la ruta de archivo y sus compensaciones.
- La regresión del listado 004 debe completarse antes del cierre de US3.

### Oportunidades paralelas

- T001-T004 pueden ejecutarse en paralelo.
- T005-T007 pueden ejecutarse en paralelo; T008-T009 deben ser secuenciales después de la configuración.
- T010-T012 y T013-T016 pueden prepararse en paralelo en archivos distintos, respetando T017-T019 antes de cerrar US1.
- T020-T022 pueden prepararse en paralelo; T023-T026 modifican superficies compartidas y deben integrarse secuencialmente.
- T027-T029 pueden prepararse en paralelo; T030-T033 integran el resultado de las pruebas.
- T034-T037 y T040-T041 pueden ejecutarse en paralelo tras la implementación.

---

## Ejemplos de ejecución paralela

### US1 - MVP

```text
T010 CreatePropertyRequestValidatorTests.cs
T011 CreatePropertyMappingTests.cs
T012 CreatePropertyHandlerTests.cs
T013 CreatePropertyRequest.cs
T014 CreatePropertyResponse.cs
T015 CreatePropertyMapping.cs
T016 CreatePropertyRequestValidator.cs

Después de cerrar contratos y pruebas:
T017 CreatePropertyHandler.cs
T018 CreatePropertySlice.cs
T019 respuesta 201 y Location
```

### US2 - Upload válido

```text
T020 pruebas PNG/JPG y colisiones
T021 prueba de response
T022 prueba de auto-descubrimiento

Después de preparar pruebas:
T023 validación de contenido y tamaño
T024 escritura segura por stream
T025 mapping de response
T026 binding multipart del slice
```

### US3 - Errores y rollback

```text
T027 CreatePropertyErrorTests.cs
T028 pruebas de limpieza por persistencia
T029 pruebas de cancelación

Después de preparar pruebas:
T030 validator negativo
T031 compensación y logging
T032 mapping HTTP 400/500
T033 revisión de composición
```

---

## Estrategia de implementación

### MVP primero

1. Completar Fase 1 y Fase 2.
2. Implementar US1 hasta T019.
3. Ejecutar sus pruebas independientes y verificar creación sin imagen con HTTP 201.
4. Detenerse en el checkpoint antes de incorporar I/O de archivos.

### Entrega incremental

1. Añadir US2 y validar PNG/JPG, tamaño, URL y colisiones.
2. Añadir US3 y validar 400, 500, cancelación y limpieza compensatoria.
3. Completar regresión del listado 004 y revisar la migración nullable.
4. Ejecutar build, suite completa, ejemplos manuales y registrar evidencia.
5. Marcar tareas como `[X]` solo después de su verificación.

### Criterio de cierre

La iniciativa solo puede avanzar de estado cuando no queden tareas pendientes,
`quickstart.md` contenga evidencia de validación, la migración sea revisada, build
y tests pasen y la revisión confirme ausencia de controllers, archivos huérfanos,
rutas físicas persistidas y cambios fuera del alcance.
