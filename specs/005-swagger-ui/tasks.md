---

description: "Lista de tareas para implementar Swagger UI en el backend"
---

# Tareas: Swagger UI para el Backend

**Entrada**: Documentos de diseño de `specs/005-swagger-ui/`

**Prerrequisitos**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/http-contracts.md` y `quickstart.md`

**Organización**: Las tareas están agrupadas por historia de usuario para permitir implementación y validación independientes.

## Dependencias y orden de ejecución

- `009-open-api` debe haber generado y versionado `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` antes de validar la carga del contrato desde Swagger UI.
- La Fase 1 confirma la estructura y la dependencia existente.
- La Fase 2 define la configuración común y bloquea el trabajo de las historias.
- Las tres historias P1 dependen de la Fase 2 y pueden implementarse en paralelo después de preparar las pruebas, aunque `US2` y `US3` reutilizan la configuración de `US1`.
- La Fase final depende de las tres historias y de la evidencia de `quickstart.md`.

## Fase 1: Preparación

**Propósito**: Confirmar la base existente y los artefactos necesarios sin introducir una segunda fuente de contrato.

- [X] T001 [P] Confirmar en `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` que `Swashbuckle.AspNetCore` versión `10.2.3` está disponible y que no se requiere agregar otra dependencia de documentación.
- [X] T002 [P] Confirmar en `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` que el documento OpenAPI v1 de `009-open-api` existe y es el contrato que consumirá Swagger UI.
- [X] T003 [P] Revisar `specs/005-swagger-ui/contracts/http-contracts.md` y fijar en las pruebas las rutas `/swagger`, `/swagger/index.html` y `/openapi/v1.json` sin crear rutas alternativas.

## Fase 2: Fundamentos

**Propósito**: Preparar el registro condicional de la UI y la composición de pruebas del backend.

- [X] T004 Configurar en `app/backend/src/NetRentManagerApi/Program.cs` el registro de servicios requerido por `Swashbuckle.AspNetCore`, sin habilitar `UseSwagger()` ni publicar `/swagger/v1/swagger.json` como documento paralelo.
- [X] T005 Configurar de forma única en `app/backend/src/NetRentManagerApi/Program.cs` el bloque basado exclusivamente en `app.Environment.IsDevelopment()` que registra Swagger UI, usa `/swagger` y apunta a `SwaggerEndpoint("/openapi/v1.json", "NetRentManagerApi v1")`.
- [X] T006 [P] Crear la estructura de pruebas de documentación en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/`, registrar Playwright como dependencia de pruebas y documentar la instalación de sus navegadores, siguiendo las convenciones xUnit existentes y reutilizando la composición del backend.
- [X] T007 [P] Documentar en `specs/005-swagger-ui/quickstart.md` los comandos y prerrequisitos de validación local para `Development` y `Production`, incluyendo las respuestas esperadas.

**Punto de control**: La infraestructura está preparada; ninguna historia debe comenzar si el registro condicional o la composición de pruebas no están disponibles.

## Fase 3: Historia de Usuario 1 - Explorar el contrato HTTP en desarrollo (Prioridad: P1) 🎯 MVP

**Objetivo**: Permitir que un desarrollador abra `/swagger` en `Development` y vea las operaciones del contrato OpenAPI v1.

**Prueba independiente**: Ejecutar el backend con `ASPNETCORE_ENVIRONMENT=Development`, solicitar `/swagger/index.html` y `/openapi/v1.json`, y comprobar HTML válido, referencia al documento v1 y presencia de las operaciones existentes.

### Pruebas de la Historia de Usuario 1

- [X] T008 [P] [US1] Crear en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiRuntimeTests.cs` una prueba xUnit que inicie la aplicación en `Development` y compruebe que `GET /swagger` responde `200 OK` o redirección a `/swagger/index.html` con destino final `text/html`.
- [X] T009 [P] [US1] Añadir en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiRuntimeTests.cs` una prueba que solicite la página de Swagger UI en `Development` y compruebe que referencia exactamente `/openapi/v1.json`.
- [X] T010 [P] [US1] Añadir en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiRuntimeTests.cs` una prueba que lea `/openapi/v1.json` y compruebe versión OpenAPI v1 y las operaciones existentes de salud, listado, detalle, creación y actualización de propiedades.

- [X] T011 [US1] Añadir en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiBrowserTests.cs` una prueba Playwright que intercepte `/openapi/v1.json` con JSON inválido y compruebe que Swagger UI muestra un contenedor de error de carga identificable.

### Implementación de la Historia de Usuario 1

- [X] T012 [US1] Verificar en `app/backend/src/NetRentManagerApi/Program.cs` que la configuración consolidada de Swagger UI identifica NetRentManagerApi, conserva la versión v1 y no registra rutas dinámicas adicionales.
- [X] T013 [US1] Ejecutar las pruebas de `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiRuntimeTests.cs` y `SwaggerUiBrowserTests.cs`, corrigiendo únicamente la integración si fallan y verificando que la UI muestra el contrato v1 sin generar un JSON paralelo.

**Punto de control**: La interfaz de Swagger UI funciona de forma independiente en `Development` y muestra el contrato existente.

## Fase 4: Historia de Usuario 2 - Probar operaciones desde la interfaz (Prioridad: P1)

**Objetivo**: Permitir ejecutar solicitudes desde Swagger UI y visualizar respuestas exitosas, errores y formularios multipart según el contrato.

**Prueba independiente**: Abrir Swagger UI en `Development`, comprobar la configuración de `Try it out`, ejecutar `GET /health` y `GET /api/properties`, y verificar método, URL, código HTTP, encabezados y cuerpo de respuesta.

### Pruebas de la Historia de Usuario 2

- [X] T014 [P] [US2] Añadir en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiBrowserTests.cs` una prueba Playwright que abra `/swagger/index.html`, espere a que cargue `/openapi/v1.json`, pulse `Try it out` sobre `GET /health`, ejecute la solicitud y compruebe método, URL, código HTTP y cuerpo de respuesta.
- [X] T015 [P] [US2] Validar en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiRuntimeTests.cs` y mediante smoke HTTP que `GET /health` y `GET /api/properties` conservan sus códigos y cuerpos en `Development`.
- [X] T016 [P] [US2] Validar en `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` que las operaciones multipart de creación y actualización declaran `multipart/form-data`, campos de propiedad y archivo `image` cuando estén presentes en v1.
- [X] T017 [P] [US2] Validar mediante el contrato OpenAPI y las respuestas HTTP existentes que una solicitud inválida conserva `ProblemDetails` o `HttpValidationProblemDetails` como error visible.

### Implementación de la Historia de Usuario 2

- [X] T018 [US2] Verificar en `app/backend/src/NetRentManagerApi/Program.cs` que Swagger UI conserva método, ruta, parámetros, cuerpo, encabezados y tipo de contenido definidos por `/openapi/v1.json`, sin filtros que oculten operaciones o respuestas.
- [X] T019 [US2] Verificar en `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` que Swagger UI no sustituye ni modifica la documentación de `ProblemDetails`, `HttpValidationProblemDetails` ni las solicitudes multipart generadas por `009-open-api`.
- [X] T020 [US2] Ejecutar las pruebas de `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiBrowserTests.cs` y comprobar manualmente desde la UI `GET /health` y `GET /api/properties` en `Development`.

**Punto de control**: La UI permite explorar y probar operaciones reales, incluyendo errores y contratos multipart, sin cambiar el comportamiento del backend.

## Fase 5: Historia de Usuario 3 - Mantener la UI fuera de producción (Prioridad: P1)

**Objetivo**: Garantizar que Swagger UI y sus recursos no existan fuera de `Development` y respondan `404 Not Found`.

**Prueba independiente**: Iniciar la aplicación con `ASPNETCORE_ENVIRONMENT=Production`, solicitar `/swagger`, `/swagger/index.html` y un recurso estático conocido, y comprobar `404` sin contenido de UI.

### Pruebas de la Historia de Usuario 3

- [X] T021 [P] [US3] Añadir en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiRuntimeTests.cs` una prueba de composición con entorno `Production` que compruebe que `GET /swagger` devuelve `404 Not Found`.
- [X] T022 [P] [US3] Añadir en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiRuntimeTests.cs` una prueba con entorno distinto de `Development` que compruebe `404 Not Found` para `/swagger/index.html` y un recurso JavaScript o CSS concreto de la versión instalada.
- [X] T023 [P] [US3] Validar en `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiRuntimeTests.cs` que no existe un endpoint dinámico `/swagger/v1/swagger.json` en ningún entorno.

### Implementación de la Historia de Usuario 3

- [X] T024 [US3] Validar en `app/backend/src/NetRentManagerApi/Program.cs` que tanto el registro de Swagger UI como su middleware estén dentro de la condición `app.Environment.IsDevelopment()` y que no haya rutas alternativas habilitadas.
- [X] T025 [US3] Ejecutar las pruebas de `app/backend/tests/NetRentManagerApiTests/Infrastructure/Swagger/SwaggerUiRuntimeTests.cs` en `Production` y en un entorno no `Development`, verificando respuestas `404` para la página y los recursos de UI.

**Punto de control**: Ninguna ruta ni recurso de Swagger UI queda publicado fuera de `Development`.

## Fase 6: Pulido y preocupaciones transversales

**Propósito**: Cerrar la trazabilidad, la validación y la evidencia de la iniciativa.

- [X] T026 [P] Revisar `app/backend/src/NetRentManagerApi/Program.cs` para confirmar que no se agregaron controllers, endpoints de negocio, documentos OpenAPI paralelos ni cambios de persistencia.
- [X] T027 [P] Ejecutar `dotnet build app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` y verificar que la generación y publicación de `wwwroot/openapi/v1.json` de `009-open-api` continúan funcionando.
- [X] T028 Ejecutar `dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` y registrar en `specs/005-swagger-ui/quickstart.md` el resultado de las pruebas y las respuestas HTTP Development/Production.
- [X] T029 [P] Revisar `specs/005-swagger-ui/checklists/requirements.md` y confirmar que cada requisito de la spec queda cubierto por una tarea completada, sin modificar los marcadores del checklist durante la implementación.
- [X] T030 Confirmar en `specs/005-swagger-ui/tasks.md` que todas las tareas completadas están marcadas `[X]` y que no queda ninguna tarea sin trazabilidad antes de solicitar el cierre de la spec.

## Dependencias y oportunidades de paralelización

### Dependencias por fase

- Fase 1 no depende de otras fases y puede ejecutarse inmediatamente.
- Fase 2 depende de Fase 1 y bloquea las historias.
- Fase 3, Fase 4 y Fase 5 dependen de Fase 2; sus tareas de prueba pueden prepararse en paralelo porque usan archivos distintos.
- Solo T004 y T005 modifican `Program.cs`; T012, T018, T024 y T026 son validaciones de solo lectura y pueden ejecutarse después de la configuración consolidada.
- T011 depende de la configuración de US1 y debe ejecutarse junto con T009/T010 porque cubre el mismo contrato desde `SwaggerUiBrowserTests.cs`.
- Fase 6 depende de todas las historias y de sus pruebas.

### Oportunidades paralelas

- T001-T003 pueden ejecutarse en paralelo porque solo inspeccionan archivos distintos.
- T006 y T007 pueden ejecutarse en paralelo con T004-T005 si no modifican el mismo archivo.
- T008-T011 cubren US1; T009/T010 usan `SwaggerUiRuntimeTests.cs` y T011 usa `SwaggerUiBrowserTests.cs`.
- T014 cubre la interacción de navegador en `SwaggerUiBrowserTests.cs`; T015-T017 son validaciones del contrato y respuestas reales.
- T021-T023 usan `SwaggerUiRuntimeTests.cs` y conviene consolidarlas en una única edición.
- T026, T027 y T029 pueden ejecutarse en paralelo; T028 debe ejecutarse después de T027.

## Ejemplos de ejecución paralela

### Historia de Usuario 1

```text
T008: prueba de disponibilidad de /swagger en SwaggerUiRuntimeTests.cs
T009-T011: pruebas del HTML, contrato v1 y documento inválido en SwaggerUiRuntimeTests.cs y SwaggerUiBrowserTests.cs
```

### Historia de Usuario 2

```text
T014-T017: prueba Playwright y validaciones de respuestas, multipart y ProblemDetails
```

### Historia de Usuario 3

```text
T021-T023: pruebas de aislamiento por entorno y ausencia de JSON dinámico en SwaggerUiRuntimeTests.cs
```

## Estrategia de implementación

### MVP: Historia de Usuario 1

1. Completar Fase 1 y Fase 2.
2. Implementar la Historia de Usuario 1.
3. Ejecutar las pruebas de disponibilidad, referencia y contenido de OpenAPI v1.
4. Detenerse para validar que `/swagger` permite explorar el contrato en `Development`.

### Entrega incremental

1. Incorporar US1 para explorar el contrato.
2. Incorporar US2 para ejecutar operaciones y visualizar respuestas reales.
3. Incorporar US3 para verificar el aislamiento completo fuera de `Development`.
4. Ejecutar la Fase 6 y registrar evidencia en `quickstart.md`.

## Criterios de prueba independientes

- **US1**: En `Development`, `/swagger` o `/swagger/index.html` devuelve HTML de Swagger UI, referencia exactamente `/openapi/v1.json` y el documento contiene las operaciones actuales.
- **US2**: Desde la UI en `Development`, `GET /health` y `GET /api/properties` pueden ejecutarse y muestran sus respuestas reales; el contrato expone multipart y errores documentados.
- **US3**: Fuera de `Development`, `/swagger`, `/swagger/index.html`, un recurso exclusivo de UI y `/swagger/v1/swagger.json` devuelven `404 Not Found`.

## Notas de trazabilidad

- No se crean entidades, migraciones ni cambios en PostgreSQL.
- No se crea una feature slice porque Swagger UI es infraestructura de documentación configurada en `Program.cs`.
- Toda modificación de código, pruebas y documentación queda asociada a una tarea con ruta explícita.
