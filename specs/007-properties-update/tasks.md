---

description: "Tareas para la actualización de propiedades con imagen opcional"
---

# Tareas: Actualización de Propiedades

**Entrada**: Documentos de diseño de `specs/007-properties-update/`

**Prerequisitos**: `spec.md`, `plan.md`, `research.md`, `data-model.md`,
`contracts/http-contracts.md`, `quickstart.md`

**Pruebas**: Se incluyen pruebas unitarias porque la spec exige cubrir actualización
sin imagen, reemplazo PNG/JPG, códigos 400/404/413/415/500, cancelación,
compensación, limpieza posterior al commit, mapping y auto-descubrimiento.

**Regla de trazabilidad**: Todo código, prueba, cambio de infraestructura,
contrato y evidencia debe corresponder a una tarea de este archivo antes de
marcarse como completado.

## Fases y dependencias

- **Fase 1 - Preparación**: confirma rutas, scanners y almacenamiento existentes.
- **Fase 2 - Fundamentos comunes**: agrega el mapeo tipado de HTTP 413/415 sin
  iniciar todavía el slice de actualización.
- **Fase 3 - US1 P1**: implementa `PUT` completo sin imagen y conservación de
  `imageUrl`; es el MVP.
- **Fase 4 - US2 P2**: agrega reemplazo de imagen válida con UUID, staging y
  limpieza posterior al commit.
- **Fase 5 - US3 P3**: completa errores, rollback, cancelación, logging y
  comportamiento de última escritura válida.
- **Fase 6 - Regresión y cierre**: ejecuta build/tests, p95, ejemplos y evidencia.

---

## Fase 1: Preparación compartida

**Propósito**: confirmar que la implementación se integra en la solución existente
sin migraciones, registros manuales ni cambios de frontend.

- [X] T001 [P] Confirmar la estructura del slice en `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty` y de sus pruebas en `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty` según `specs/007-properties-update/plan.md`.
- [X] T002 [P] Revisar `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/ISlice.cs`, `app/backend/src/NetRentManagerApi/Infrastructure/Handlers/IHandler.cs` y los registradores existentes para confirmar auto-descubrimiento sin registros manuales nuevos.
- [X] T003 [P] Revisar `app/backend/src/NetRentManagerApi/Program.cs` y `app/backend/src/NetRentManagerApi/Features/Properties/CreateProperty/CreatePropertyHandler.cs` para confirmar `WebRootPath`, `UseStaticFiles` y la convención `/assets/properties/` reutilizable.
- [X] T004 [P] Revisar `app/backend/src/NetRentManagerApi/Domain/Properties/Property.cs` y `app/backend/src/NetRentManagerApi/Infrastructure/Persistence/Configurations/PropertyConfiguration.cs` para confirmar `ImageUrl` nullable y `UpdatedAt` sin crear migración.

**Checkpoint**: las convenciones existentes y las rutas de implementación están
confirmadas antes de modificar código.

---

## Fase 2: Fundamentos comunes bloqueantes

**Propósito**: habilitar la semántica de errores 413/415 requerida por el contrato
sin duplicar respuestas manuales en el slice.

- [X] T005 [P] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Errors/ErrorStatusMappingTests.cs` para comprobar el mapeo de `UnsupportedMediaType` a HTTP 415, `PayloadTooLarge` a HTTP 413 y la conservación de los códigos ProblemDetails.
- [X] T006 Extender `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ErrorType.cs` con `UnsupportedMediaType` y `PayloadTooLarge`, preservando los tipos existentes de validación, not found e interno.
- [X] T007 Actualizar `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ResultProblemDetailsMapper.cs` para mapear los nuevos tipos a HTTP 415 y HTTP 413 mediante ProblemDetails sin cambiar el comportamiento de HTTP 400, 404 y 500.
- [X] T008 Ejecutar las pruebas focalizadas de `app/backend/tests/NetRentManagerApiTests/Infrastructure/Errors/ErrorStatusMappingTests.cs` y confirmar que el mapper común permite implementar el contrato de errores de la iniciativa.

**Checkpoint**: la infraestructura común traduce 400/404/413/415/500 de forma
consistente y las historias pueden reutilizarla.

---

## Fase 3: Historia de Usuario 1 - Actualizar sin imagen (P1) - MVP

**Objetivo**: reemplazar todos los campos editables obligatorios de una propiedad
existente mediante `PUT /api/properties/{id}` y conservar exactamente su
`imageUrl` cuando no se envía `image`.

**Prueba independiente**: actualizar una propiedad con imagen previa y otra con
`imageUrl: null`, verificar HTTP 200, campos reemplazados, `UpdatedAt` y URL sin
cambios; verificar también HTTP 404 y cero efectos para un id inexistente.

### Pruebas de US1

- [X] T009 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyRequestValidatorTests.cs` para comprobar que todos los campos de `PUT` son obligatorios, los rangos son válidos y `image` ausente es aceptado.
- [X] T010 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyMappingTests.cs` para verificar mapping explícito de `Property` a `UpdatePropertyResponse`, `status` textual, `imageUrl` nullable y fechas.
- [X] T011 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyHandlerTests.cs` con EF Core InMemory para comprobar actualización completa sin imagen, conservación de URL existente, conservación de `null`, `UpdatedAt` y `CreatedAt` intacto.
- [X] T012 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyNotFoundTests.cs` para comprobar que un id inexistente devuelve `Error.NotFound`, no persiste cambios y no crea archivos.

### Implementación de US1

- [X] T013 [P] [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyRequest.cs` con los ocho campos editables obligatorios y `IFormFile? Image` opcional para binding multipart.
- [X] T014 [P] [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyResponse.cs` con id, campos persistidos, `status` textual, `imageUrl` nullable, `createdAt` y `updatedAt`.
- [X] T015 [P] [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyMapping.cs` con transformaciones explícitas entre entidad y response, sin acceso a datos, I/O ni reglas de negocio.
- [X] T016 [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyRequestValidator.cs` con FluentValidation para obligatoriedad, longitudes, rangos, estados permitidos y archivo opcional, usando auto-descubrimiento existente.
- [X] T017 [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` para buscar por id, devolver Not Found sin I/O, aplicar reemplazo completo sin imagen, conservar `ImageUrl` y guardar `UpdatedAt` con `SaveChangesAsync(cancellationToken)`.
- [X] T018 [US1] Crear `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertySlice.cs` para registrar `PUT /api/properties/{id}`, aceptar `multipart/form-data`, resolver el handler auto-registrado y delegar sin lógica de negocio.
- [X] T019 [US1] Mapear en `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertySlice.cs` el éxito a HTTP 200 y los resultados mediante `ResultProblemDetailsMapper`, incluyendo metadatos OpenAPI para 200, 400, 404, 413, 415 y 500.
- [X] T020 [US1] Ejecutar las pruebas de `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/` correspondientes a US1 y corregir solo el slice, validator, mapping o handler trazado a esta fase.

**Checkpoint**: US1 funciona de forma independiente con reemplazo completo sin
imagen, preservación de `imageUrl` y respuesta 404 sin efectos para ids ausentes.

---

## Fase 4: Historia de Usuario 2 - Reemplazar imagen válida (P2)

**Objetivo**: aceptar PNG/JPG/JPEG reales de hasta 5 MiB, almacenarlos con UUID,
persistir la URL nueva y retirar la imagen anterior solo después del commit.

**Prueba independiente**: ejecutar actualizaciones con PNG y JPG/JPEG, verificar
HTTP 200, archivos públicos, URLs distintas ante nombres originales iguales y
persistencia de la nueva referencia.

### Pruebas de US2

- [X] T021 [P] [US2] Ampliar `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyHandlerTests.cs` para cubrir PNG, JPG/JPEG, UUID, extensión normalizada, URL `/assets/properties/`, reemplazo de `imageUrl` y no sobrescritura.
- [X] T022 [P] [US2] Ampliar `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyMappingTests.cs` para comprobar que una URL nueva se serializa como relativa y nunca contiene ruta física, `support` o nombre original inseguro.
- [X] T023 [P] [US2] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertySliceTests.cs` para comprobar que el endpoint es público, implementa `ISlice`, usa `PUT /api/properties/{id}` y declara multipart/ProblemDetails.

### Implementación de US2

- [X] T024 [US2] Completar `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` con validación de tamaño máximo de 5 MiB, extensión/MIME auxiliar, magic bytes PNG/JPEG y errores tipados 413/415.
- [X] T025 [US2] Completar `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` para generar nombre UUID, crear el destino runtime y copiar el upload con `FileMode.CreateNew`, I/O asíncrono y `CancellationToken`.
- [X] T026 [US2] Completar `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` para conservar valores anteriores, asignar la nueva URL solo al estado staged y persistirla junto con los campos editables.
- [X] T027 [US2] Completar `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` para intentar eliminar la imagen anterior después de `SaveChangesAsync`, restringiendo la limpieza a rutas públicas administradas y registrando fallos sin cambiar HTTP 200.
- [X] T028 [US2] Completar `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyMapping.cs` para devolver `UpdatePropertyResponse` con `status` textual, `imageUrl` nueva y fechas persistidas.
- [X] T029 [US2] Ejecutar las pruebas focalizadas de `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/` para US1 y US2 y confirmar que los casos sin imagen siguen conservando la URL anterior después de añadir el reemplazo con imagen.

**Checkpoint**: US1 y US2 funcionan independientemente; las imágenes válidas se
publican con UUID, reemplazan la referencia solo tras persistir y no rompen el MVP.

---

## Fase 5: Historia de Usuario 3 - Rechazos y efectos parciales (P3)

**Objetivo**: completar validación negativa, rollback previo al commit, cancelación,
logging y consistencia de datos y filesystem.

**Prueba independiente**: ejecutar todos los errores 400/404/413/415/500 y fallos
simulados de almacenamiento/persistencia, comprobando que la propiedad conserva sus
valores previos y que no quedan archivos nuevos parciales.

### Pruebas de US3

- [X] T030 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyErrorTests.cs` para campos inválidos 400, archivo vacío 415, formato/MIME no permitido 415, contenido incompatible 415 y tamaño mayor de 5 MiB 413.
- [X] T031 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyCleanupTests.cs` para simular fallos de copia, persistencia y limpieza, comprobar rollback de entidad, conservación de URL anterior y eliminación del archivo nuevo.
- [X] T032 [P] [US3] Añadir a `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyCleanupTests.cs` casos de cancelación durante lectura/escritura y comprobar que se propaga `OperationCanceledException` con limpieza compensatoria.
- [X] T033 [P] [US3] Añadir a `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertyHandlerTests.cs` una prueba de dos actualizaciones válidas consecutivas que confirme última escritura confirmada sin `rowVersion`, bloqueo ni HTTP 409.
- [X] T034 [P] [US3] Confirmar mediante `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesHandlerTests.cs` existente que una propiedad con `imageUrl: null` continúa apareciendo correctamente después de actualizarse sin imagen.

### Implementación de US3

- [X] T035 [US3] Completar `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyRequestValidator.cs` para producir detalles por campo HTTP 400 y separar validaciones de negocio de las reglas de bytes que producen 413/415.
- [X] T036 [US3] Completar `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` con captura controlada de fallos, restauración de valores tracked, eliminación compensatoria del archivo nuevo, logging estructurado y `Error.Internal` HTTP 500.
- [X] T037 [US3] Completar `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertyHandler.cs` para propagar cancelación a lectura, escritura y persistencia sin convertirla en respuesta exitosa.
- [X] T038 [US3] Completar `app/backend/src/NetRentManagerApi/Features/Properties/UpdateProperty/UpdatePropertySlice.cs` para preservar el mapper común de 400/404/413/415/500 y no introducir respuestas manuales ni registros centrales.
- [X] T039 [US3] Revisar `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesMapping.cs` y confirmar que la regresión existente mantiene compatibilidad con `imageUrl: null` sin cambios adicionales.
- [X] T040 [US3] Ejecutar la suite focalizada de `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/` para US3 y confirmar ausencia de propiedades parciales, archivos nuevos huérfanos previos al commit y pérdida de la nueva referencia por fallo de limpieza posterior.

**Checkpoint**: todas las entradas inválidas producen su código contractual, los
fallos previos al commit revierten la operación y la limpieza posterior no rompe
una actualización ya confirmada.

---

## Fase 6: Regresión, rendimiento y cierre

**Propósito**: validar la iniciativa completa, documentar evidencia y cerrar tareas
solo después de build, tests y quickstart satisfactorios.

- [X] T041 [P] Actualizar `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` con ejemplos de `PUT` sin imagen, `PUT` con PNG, id inexistente y archivos inválidos.
- [X] T042 [P] Confirmar mediante `app/backend/tests/NetRentManagerApiTests/Features/Properties/UpdateProperty/UpdatePropertySliceTests.cs` y los scanners existentes el auto-descubrimiento sin registros manuales en `Program` ni registradores centrales.
- [X] T043 Ejecutar `dotnet build .\app\NetRentManager.sln --no-restore` para `app/NetRentManager.sln` y corregir únicamente errores en archivos trazados por esta iniciativa.
- [X] T044 Ejecutar `dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore` para `app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` y confirmar la suite existente más las pruebas US1, US2, US3, errores y regresión del listado.
- [X] T045 Ejecutar los escenarios de `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` y `specs/007-properties-update/quickstart.md` y registrar HTTP 200, 404, 413, 415, conservación de URL y disponibilidad pública del asset; los códigos 400/500 quedan cubiertos por pruebas unitarias de la suite.
- [X] T046 Medir en el entorno documentado de `specs/007-properties-update/quickstart.md` al menos 100 actualizaciones válidas sin imagen, calcular p95 de 330.95 ms y confirmar 100 respuestas HTTP 200, por debajo de 500 ms.
- [X] T047 Inspeccionar `app/backend/src/NetRentManagerApi/` y confirmar que no se agregaron controllers, migraciones, `rowVersion`, bloqueos, rutas físicas persistidas, registros manuales ni cambios frontend.
- [X] T048 Registrar en `specs/007-properties-update/quickstart.md`, en `## Evidencia de validación`, fecha, comandos, cantidad de pruebas, p95, respuestas HTTP, UUID, limpieza compensatoria y ausencia de control de concurrencia.
- [X] T049 Confirmar que todas las tareas de `specs/007-properties-update/tasks.md` están marcadas `[X]` solo después de build, tests, escenarios, medición p95 y evidencia completa; mantener el estado de la spec conforme al flujo Speckit.

**Checkpoint final**: el endpoint de actualización, la preservación/reemplazo de
imágenes, errores, atomicidad, rendimiento y evidencia están completos y trazados.

---

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Fase 1** no depende de otras fases.
- **Fase 2** depende de T001-T004 y bloquea las historias porque los errores 413/415
  deben estar disponibles antes del slice.
- **US1** depende de T005-T008 y entrega el MVP sin imagen.
- **US2** depende de US1 porque extiende el request, handler, mapping y slice ya
  funcionales con la ruta de reemplazo de imagen.
- **US3** depende de US1 y US2 porque prueba sus rutas de error, compensación y
  cancelación.
- **Fase 6** depende de US1, US2 y US3.

### Dependencias de historias

- **US1** es el MVP y establece el contrato base de actualización completa.
- **US2** depende de US1, pero sus pruebas de imagen pueden prepararse en paralelo
  antes de integrar cambios al handler.
- **US3** depende de US2 para validar staging, rollback y limpieza posterior.

### Oportunidades paralelas

- T001-T004 pueden ejecutarse en paralelo porque solo inspeccionan superficies distintas.
- T005 puede prepararse en paralelo con T006; T007 depende de T006 y T008 de T005-T007.
- T009-T012 y T013-T016 pueden ejecutarse en paralelo en archivos distintos; T017-T019 integran el slice después.
- T021-T023 pueden ejecutarse en paralelo; T024-T028 deben integrarse secuencialmente porque modifican el mismo flujo.
- T030-T034 pueden ejecutarse en paralelo en archivos de prueba distintos; T035-T039 integran los cambios de producción.
- T041-T042 pueden ejecutarse en paralelo; T043-T049 deben ejecutarse con las dependencias indicadas.

---

## Ejemplos de ejecución paralela

### US1 - MVP

```text
T009 UpdatePropertyRequestValidatorTests.cs
T010 UpdatePropertyMappingTests.cs
T011 UpdatePropertyHandlerTests.cs
T012 UpdatePropertyNotFoundTests.cs
T013 UpdatePropertyRequest.cs
T014 UpdatePropertyResponse.cs
T015 UpdatePropertyMapping.cs
T016 UpdatePropertyRequestValidator.cs

Después de cerrar los contratos y pruebas:
T017 UpdatePropertyHandler.cs
T018 UpdatePropertySlice.cs
T019 respuesta HTTP 200 y ProblemDetails
```

### US2 - Reemplazo de imagen

```text
T021 pruebas PNG/JPG, UUID y colisiones
T022 pruebas de response y URL segura
T023 pruebas del contrato y auto-descubrimiento

Después de completar US1:
T024 validación 413/415
T025 staging y copia UUID
T026 persistencia de nueva URL
T027 limpieza posterior al commit
T028 mapping final
```

### US3 - Errores y compensación

```text
T030 pruebas de códigos 400/413/415
T031 pruebas de rollback y limpieza
T032 pruebas de cancelación
T033 prueba de última escritura válida
T034 regresión de listado nullable

Después de preparar las pruebas:
T035 validator
T036 handler y rollback
T037 cancelación
T038 slice y mapper
T039 compatibilidad del listado
```

---

## Estrategia de implementación

### MVP primero: solo Historia de Usuario 1

1. Completar Fase 1 y Fase 2.
2. Implementar US1 con reemplazo completo sin imagen.
3. Ejecutar las pruebas independientes de US1.
4. Detenerse para validar que HTTP 200 conserva `imageUrl` y HTTP 404 no produce efectos.

### Entrega incremental

1. Completar preparación y fundamentos comunes.
2. Entregar US1 como MVP.
3. Añadir US2 para reemplazo seguro de imágenes y validar sin regresión del MVP.
4. Añadir US3 para errores, rollback, cancelación y limpieza.
5. Ejecutar cierre, p95 y evidencia antes de marcar tareas completas.

### Notas de ejecución

- Cada prueba debe escribirse antes de la implementación asociada y debe fallar por
  la ausencia del comportamiento, salvo pruebas de infraestructura ya existente.
- Las tareas `[P]` solo son paralelizables si no existen cambios concurrentes en el
  mismo archivo y sus dependencias previas están completas.
- No crear migración: `ImageUrl` nullable y `UpdatedAt` ya pertenecen al modelo de
  las iniciativas anteriores.
- No marcar `[X]` hasta verificar la tarea y registrar evidencia cuando corresponda.
