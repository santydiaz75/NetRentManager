---

description: "Tareas para el listado paginado de propiedades"
---

# Tareas: Listado Paginado de Propiedades

**Entrada**: Documentos de diseño de `specs/004-properties-list-pagination/`

**Prerequisitos**: `spec.md`, `plan.md`, `research.md`, `data-model.md`,
`contracts/http-contracts.md`, `quickstart.md`

**Pruebas**: Se incluyen pruebas unitarias porque la spec y el plan exigen validar
request, paginación, mapping, cancelación, errores ProblemDetails y
auto-descubrimiento. No se introduce una prueba de integración ni un proveedor
nuevo sin una tarea específica aprobada.

**Regla de trazabilidad**: Todo código, prueba, cambio de infraestructura y cambio
del archivo HTTP debe corresponder a una tarea de este archivo antes de marcarse
como completado.

## Fases y dependencias

- **Fase 1 - Preparación**: confirma la estructura existente y prepara directorios de la feature y pruebas.
- **Fase 2 - Fundamentos**: define contratos comunes del slice y deja lista la infraestructura compartida de errores y assets.
- **Fase 3 - US1**: entrega el listado predeterminado con contratos explícitos y orden estable. Es el MVP.
- **Fase 4 - US2**: completa navegación por páginas y metadatos consistentes.
- **Fase 5 - US3**: integra validación, URLs absolutas, errores de imagen y cancelación.
- **Fase 6 - Pulido**: ejecuta build, tests, prueba manual y registra evidencia.

---

## Fase 1: Preparación compartida

**Propósito**: preparar la estructura sin modificar el esquema persistente ni crear
un proyecto paralelo.

- [X] T001 [P] Crear el directorio `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties` para el slice de listado.
- [X] T002 [P] Crear el directorio `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties` para las pruebas del caso de uso.
- [X] T003 [P] Revisar `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/SliceRegistrationExtensions.cs`, `app/backend/src/NetRentManagerApi/Infrastructure/Handlers/HandlerRegistrationExtensions.cs` y `app/backend/src/NetRentManagerApi/Infrastructure/Validation/ValidatorRegistrationExtensions.cs` para confirmar que no requieren registro manual del nuevo slice, handler ni validator.
- [X] T004 [P] Revisar `app/backend/src/NetRentManagerApi/Program.cs` y `app/backend/src/NetRentManagerApi/wwwroot/assets/properties/` para confirmar la configuración necesaria para servir assets públicos sin modificar la persistencia de la spec 003.

**Checkpoint**: la estructura de la feature está preparada y se ha confirmado que
el auto-descubrimiento existente será reutilizado.

---

## Fase 2: Fundamentos bloqueantes

**Propósito**: fijar contratos, validación de error interno y criterios de prueba
antes de implementar las historias.

- [X] T005 [P] Crear `ListPropertiesRequest` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesRequest.cs` con `page = 1` y `pageSize = 6` como valores predeterminados.
- [X] T006 [P] Crear `PropertyListItem` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/PropertyListItem.cs` con los diez campos de salida definidos en `data-model.md`.
- [X] T007 [P] Crear `PagedPropertiesResponse` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/PagedPropertiesResponse.cs` con `items` y los seis metadatos de paginación.
- [X] T008 [P] Extender `ErrorType` en `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ErrorType.cs` con una categoría interna que represente inconsistencias de datos como HTTP 500 sin exponer detalles internos.
- [X] T009 Actualizar `app/backend/src/NetRentManagerApi/Infrastructure/Errors/ResultProblemDetailsMapper.cs` para traducir el error interno a `ProblemDetails` con estado 500 y código estable, preservando los mapeos existentes.
- [X] T010 [P] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/Errors/ResultProblemDetailsMapperTests.cs` para verificar que el error interno produce HTTP 500 y que los errores de validación existentes continúan produciendo HTTP 400.

**Checkpoint**: los contratos de request/response y la traducción central de error
están definidos antes de conectar la consulta.

---

## Fase 3: Historia de Usuario 1 - Consultar el primer listado (P1) - MVP

**Objetivo**: devolver la primera página predeterminada con hasta seis propiedades,
contrato explícito y orden estable por título e identificador.

**Prueba independiente**: ejecutar `GET /api/properties` sobre propiedades
persistidas y comprobar `page = 1`, `pageSize = 6`, `items` y el orden ascendente.

### Pruebas de US1

- [X] T011 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesHandlerTests.cs` con datos controlados para verificar que la consulta predeterminada devuelve como máximo seis items, proyecta todos los campos requeridos y ordena por `Title` ascendente con `Id` como desempate.
- [X] T012 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesMappingTests.cs` para verificar que el mapping produce `PropertyListItem` y `PagedPropertiesResponse` sin devolver entidades `Property`.

### Implementación de US1

- [X] T013 [US1] Crear `ListPropertiesMapping` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesMapping.cs` con mapping explícito de la proyección persistente al contrato `PropertyListItem`.
- [X] T014 [US1] Crear `ListPropertiesHandler` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesHandler.cs` implementando `IHandler`, consultando `AppDbContext.Properties` con `AsNoTracking`, `OrderBy(Title)`, `ThenBy(Id)`, `Skip`, `Take` y `ToListAsync` con cancelación.
- [X] T015 [US1] Crear `ListPropertiesSlice` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesSlice.cs` implementando `ISlice`, registrando únicamente `GET /api/properties` y delegando la ejecución al handler sin modificar `Program.cs` ni crear un registry manual.
- [X] T016 [US1] Compilar `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` y verificar que el auto-descubrimiento registra el nuevo slice y handler sin cambios manuales en los registradores.

**Checkpoint**: el MVP devuelve la primera página con el contrato explícito y el
orden estable, y puede validarse de forma independiente.

---

## Fase 4: Historia de Usuario 2 - Navegar entre páginas (P1)

**Objetivo**: soportar `page` y `pageSize` válidos, calcular el total y devolver
metadatos coherentes para páginas intermedias, vacías y posteriores a la última.

**Prueba independiente**: consultar con `page = 2` y `pageSize = 6` sobre más de seis
propiedades y comparar `items`, `totalItems`, `totalPages`, `hasNext` y
`hasPrevious` con el conjunto conocido.

### Pruebas de US2

- [X] T017 [P] [US2] Ampliar `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesHandlerTests.cs` para verificar `CountAsync` antes de paginar, cálculo de `totalPages`, `hasNext`, `hasPrevious`, página posterior a la última y conjunto vacío.
- [X] T018 [P] [US2] Añadir a `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesHandlerTests.cs` una prueba de repetición de la misma consulta para confirmar orden y contenido deterministas con títulos duplicados.

### Implementación de US2

- [X] T019 [US2] Completar `ListPropertiesHandler` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesHandler.cs` para ejecutar `CountAsync(cancellationToken)`, calcular `totalPages` con cero para colecciones vacías y construir todos los metadatos del contrato.
- [X] T020 [US2] Completar `PagedPropertiesResponse` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/PagedPropertiesResponse.cs` para conservar la propiedad JSON `items` y los nombres camelCase esperados por `contracts/http-contracts.md`.
- [X] T021 [US2] Ejecutar las pruebas focalizadas de `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesHandlerTests.cs` y corregir únicamente defectos de paginación o metadatos del slice.

**Checkpoint**: las páginas válidas, las páginas vacías y los metadatos son
consistentes sin ampliar el alcance a filtros u orden configurable.

---

## Fase 5: Historia de Usuario 3 - Imágenes públicas y errores claros (P1)

**Objetivo**: validar parámetros, propagar cancelación, construir URLs absolutas
servibles y devolver errores ProblemDetails correctos.

**Prueba independiente**: comprobar solicitudes inválidas con HTTP 400, URLs bajo
`/assets/properties`, cancelación y HTTP 500 ante `ImageUrl` persistida inválida.

### Pruebas de US3

- [X] T022 [P] [US3] Crear `ListPropertiesRequestValidatorTests` en `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesRequestValidatorTests.cs` para validar `page >= 1`, `1 <= pageSize <= 100`, defaults y rechazo de valores fuera de rango.
- [X] T023 [P] [US3] Crear `ListPropertiesErrorTests` en `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesErrorTests.cs` para verificar que una `ImageUrl` vacía, física, bajo `support` o no convertible produce `Result` con error interno y HTTP 500 sin detalles sensibles.
- [X] T024 [P] [US3] Añadir pruebas de cancelación a `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesHandlerTests.cs` para comprobar que `CancellationToken` llega a `CountAsync` y `ToListAsync` y que la operación cancelada no continúa el mapping.

### Implementación de US3

- [X] T025 [US3] Crear `ListPropertiesRequestValidator` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesRequestValidator.cs` con FluentValidation para rechazar `page <= 0`, `pageSize <= 0` y `pageSize > 100`, usando el registro automático existente.
- [X] T026 [US3] Completar `ListPropertiesMapping` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesMapping.cs` para construir `imageUrl` absoluta con esquema y host del request y `/assets/properties/{fileName}`, rechazando traversal, rutas físicas, `support` y valores vacíos.
- [X] T027 [US3] Completar `ListPropertiesHandler` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesHandler.cs` para convertir la inconsistencia de imagen en `Result<PagedPropertiesResponse>` con el error interno definido en T008-T009, sin lanzar excepciones como control de flujo.
- [X] T028 [US3] Actualizar `app/backend/src/NetRentManagerApi/Program.cs` únicamente si es necesario para ejecutar `UseStaticFiles()` antes de mapear slices, garantizando que `/assets/properties/{fileName}` sea servible sin alterar `HealthSlice` ni la secuencia de migración.
- [X] T029 [US3] Completar `ListPropertiesSlice` en `app/backend/src/NetRentManagerApi/Features/Properties/ListProperties/ListPropertiesSlice.cs` para delegar el `Result` mediante `ResultProblemDetailsMapper`, conservar HTTP 200 para respuestas válidas y propagar `HttpContext` y `CancellationToken`.

**Checkpoint**: las validaciones devuelven HTTP 400, las imágenes válidas son URLs
absolutas servibles, las inconsistentes devuelven HTTP 500 y la cancelación llega a
la consulta.

---

## Fase 6: Pulido y validación transversal

**Propósito**: documentar las solicitudes manuales, verificar restricciones y
cerrar solo con evidencia reproducible.

- [X] T030 [P] Actualizar `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` con `GET /api/properties` y `GET /api/properties?page=1&pageSize=6`, manteniendo el host local existente y agregando `Accept: application/json`.
- [X] T031 [P] Añadir en `app/backend/tests/NetRentManagerApiTests/Features/Properties/ListProperties/ListPropertiesSliceTests.cs` pruebas de auto-descubrimiento y mapping de `GET /api/properties` sin registro manual ni rutas adicionales.
- [X] T032 Ejecutar `dotnet build .\app\NetRentManager.sln --no-restore` y corregir errores de compilación exclusivamente en los archivos trazados por esta spec.
- [X] T033 Ejecutar `dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore` y confirmar que pasan las pruebas existentes y las nuevas de las tres historias.
- [X] T034 [P] Ejecutar las solicitudes de `app/backend/src/NetRentManagerApi/NetRentManagerApi.http` y comprobar en `specs/004-properties-list-pagination/quickstart.md` el contrato 200, las URLs absolutas y el 400 de `pageSize=101`.
- [X] T035 [P] Inspeccionar `app/backend/src/NetRentManagerApi/` para confirmar que no existen controllers, endpoints de propiedades fuera de `Features/Properties/ListProperties`, mappers automáticos, cambios de esquema o registros manuales paralelos.
- [X] T036 Registrar en `specs/004-properties-list-pagination/quickstart.md`, en `## Evidencia de validación`, fecha, comandos, conteo de pruebas, respuestas HTTP y verificación de assets públicos.
- [X] T037 Confirmar que todas las tareas de `specs/004-properties-list-pagination/tasks.md` están marcadas `[X]` únicamente después de validar build, tests y quickstart, y que la spec permanece en `En implementación` hasta la transición autorizada del flujo.

**Checkpoint final**: el endpoint está probado, documentado, trazado y conforme a
Minimal APIs, ISlice, Vertical Slice, ProblemDetails y las restricciones de la spec.

---

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Fase 1** no depende de otras tareas y solo prepara estructura y comprobaciones.
- **Fase 2** depende de T001-T004 y bloquea la implementación de las historias.
- **US1** depende de T005-T010; T013-T015 implementan el MVP y T016 lo compila.
- **US2** depende de US1 porque amplía el handler y la respuesta ya creados.
- **US3** depende de US1 y US2 para integrar mapping, validación, cancelación y errores.
- **Fase 6** depende de completar las tres historias.

### Dependencias de historias

- **US1** es el MVP: contratos, consulta proyectada, primera página, orden estable y slice.
- **US2** depende de US1 y agrega conteo y navegación sin cambiar el contrato base.
- **US3** depende de US1/US2 y completa validación, imágenes, cancelación y errores.

### Oportunidades paralelas

- T001-T004 pueden ejecutarse en paralelo porque son comprobaciones o directorios independientes.
- T005-T008 y T010 pueden prepararse en paralelo antes de T009.
- T011-T012 pueden escribirse en paralelo antes de T013-T015.
- T017-T018 pueden escribirse en paralelo, aunque ambos modifican el mismo archivo y deben integrarse secuencialmente.
- T022-T024 pueden prepararse en paralelo en archivos o superficies distintas.
- T030-T031 y T034-T035 pueden ejecutarse en paralelo después de cerrar la implementación.

---

## Ejemplos de ejecución paralela

### US1 - MVP

```text
T011 ListPropertiesHandlerTests.cs
T012 ListPropertiesMappingTests.cs

Después de preparar las pruebas:
T013 ListPropertiesMapping.cs
T014 ListPropertiesHandler.cs
T015 ListPropertiesSlice.cs
T016 compilación focalizada
```

### US2 - Navegación

```text
T017 pruebas de metadatos en ListPropertiesHandlerTests.cs
T018 prueba de determinismo en ListPropertiesHandlerTests.cs

Después de integrar las pruebas:
T019 cálculo de total y metadatos en ListPropertiesHandler.cs
T020 contrato PagedPropertiesResponse.cs
T021 pruebas focalizadas del handler
```

### US3 - Imágenes y errores

```text
T022 ListPropertiesRequestValidatorTests.cs
T023 ListPropertiesErrorTests.cs
T024 pruebas de cancelación en ListPropertiesHandlerTests.cs

Después de preparar las pruebas:
T025 validator
T026 mapping de URL
T027 error interno en handler
T028 static files
T029 integración final del slice
```

---

## Estrategia de implementación

### MVP primero

1. Completar Fase 1 y Fase 2.
2. Implementar US1 hasta T016.
3. Ejecutar las pruebas independientes de US1 y validar el contrato `items`.
4. Detenerse en el checkpoint para demostrar el listado predeterminado antes de ampliar alcance.

### Entrega incremental

1. Añadir US2 y validar segunda página, páginas vacías y metadatos.
2. Añadir US3 y validar 400, URLs absolutas, cancelación y 500.
3. Ejecutar la Fase 6 y registrar evidencia en `quickstart.md`.
4. Marcar tareas como `[X]` solo después de su validación correspondiente.

### Criterio de cierre

La iniciativa solo puede avanzar de estado cuando no queden tareas pendientes,
`quickstart.md` contenga evidencia de validación, el build y la suite de pruebas
sean correctos y la revisión confirme que no se introdujeron controllers ni cambios
fuera del alcance aprobado.
