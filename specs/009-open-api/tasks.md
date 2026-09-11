---

description: "Tareas para el contrato OpenAPI v1 público"
---

# Tareas: OpenAPI v1 Público

**Entrada**: Documentos de diseño de `specs/009-open-api/`

**Prerequisitos**: `spec.md`, `plan.md`, `research.md`, `data-model.md`,
`quickstart.md`, `package.json`

**Pruebas**: Se incluyen pruebas porque la spec exige validar el inventario runtime,
drift inducido, publicación JSON, schemas compartidos, ausencia de UI y consumo
mediante NSwag/Redocly.

**Regla de trazabilidad**: Todo cambio en `.csproj`, `Program.cs`, herramientas,
manifest NPM, configuración Redocly, scripts, documento generado, pruebas y
artifacts debe corresponder a una tarea de este archivo.

## Fases y dependencias

- **Fase 1 - Herramientas y configuración**: prepara Redocly, NSwag y manifests.
- **Fase 2 - Generación y publicación**: configura MSBuild y JSON estático v1.
- **Fase 3 - US1 P1**: publica y consume el documento OpenAPI completo; es el MVP.
- **Fase 4 - US2 P2**: valida lint y genera cliente C# tipado offline.
- **Fase 5 - US3 P3**: implementa drift, normalización y exclusión de UI.
- **Fase 6 - Regresión y cierre**: script reproducible, build, tests y evidencia.

---

## Fase 1: Herramientas y configuración compartida

**Propósito**: dejar disponibles las herramientas obligatorias antes de generar o
validar el contrato.

- [X] T001 [P] Verificar en `package.json` y `package-lock.json` que `@redocly/cli` esté disponible para `npx` y documentar Node.js `>= 20`, npm y npx como prerrequisitos.
- [X] T002 [P] Crear `.redocly.yaml` con `security-defined: off` y `operation-4xx-response: warn`, manteniendo estrictas las demás reglas de lint para la API pública.
- [X] T003 [P] Crear `dotnet-tools.json` en la raíz con `NSwag.ConsoleCore` registrado y verificable mediante `dotnet tool restore`, `dotnet tool list` y `dotnet tool run nswag`.
- [X] T004 [P] Crear `support/scripts/openapi-v1.nswag.json` para leer `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` y generar el cliente C# en `artifacts/openapi-client-smoke/` sin uso runtime.
- [X] T005 [P] Añadir a `specs/009-open-api/quickstart.md` la verificación previa de `node --version`, `npm --version`, `npx @redocly/cli --version`, `dotnet tool restore` y `dotnet tool list`.

**Checkpoint**: la máquina y la configuración de validación están listas antes de
tocar generación o publicación.

---

## Fase 2: Generación durante build y publicación estática

**Propósito**: generar `openapi/v1.json` desde la implementación real durante
MSBuild y servirlo estáticamente sin UI.

- [X] T006 [P] Configurar `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` para activar la generación OpenAPI de ASP.NET Core/MSBuild durante build y escribir el documento en `wwwroot/openapi/v1.json`.
- [X] T007 [P] Asegurar mediante `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` que `wwwroot/openapi/v1.json` se incluya en output/publish como contenido estático versionado, sin generarlo en startup ni por request.
- [X] T008 Actualizar `app/backend/src/NetRentManagerApi/Program.cs` para mantener `UseStaticFiles`, servir `/openapi/v1.json` en todos los entornos soportados y eliminar o evitar `MapOpenApi()` runtime si genera una ruta duplicada o un documento alternativo.
- [X] T009 Ejecutar el build de `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` y revisar `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` para confirmar que contiene `openapi` v1, los cinco endpoints y `components/schemas` compartidos.
- [X] T010 [P] Añadir una prueba en `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiRuntimeRouteTests.cs` que confirme HTTP 200, `Content-Type: application/json`, documento JSON y ausencia de Swagger UI/ReDoc UI/Scalar UI en el inventario runtime.

**Checkpoint**: el build genera un único JSON v1 y runtime solo expone el archivo
estático, sin superficie interactiva.

---

## Fase 3: Historia de Usuario 1 - Consumir contrato OpenAPI v1 (P1) - MVP

**Objetivo**: publicar un documento v1 completo con el 100% de los endpoints reales
implementados y sus operaciones, parámetros, bodies y errores.

**Prueba independiente**: compilar, solicitar `/openapi/v1.json` y comprobar los
cinco endpoints: `GET /health`, `GET /api/properties`, `POST /api/properties`,
`GET /api/properties/{id}` y `PUT /api/properties/{id}`.

### Pruebas de US1

- [X] T011 [P] [US1] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDocumentTests.cs` para comprobar `openapi` v1, los cinco paths normalizados, métodos GET/POST/PUT, parámetros y ausencia de paths fuera del inventario real.
- [X] T012 [P] [US1] Añadir a `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDocumentTests.cs` validaciones de `components/schemas/PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails` y referencias `$ref` desde las operaciones.
- [X] T013 [P] [US1] Añadir a `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDocumentTests.cs` comprobaciones de request bodies multipart para `POST /api/properties` y `PUT /api/properties/{id}`, incluyendo `image` opcional cuando aplique.

### Implementación de US1

- [X] T014 [US1] Completar la configuración de `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` y regenerar `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` desde endpoints reales sin editar manualmente el JSON.
- [X] T015 [US1] Revisar y corregir los metadatos de `app/backend/src/NetRentManagerApi/Infrastructure/Endpoints/HealthSlice.cs` y de los slices de propiedades solo si son necesarios para que método, ruta, body y responses aparezcan en el documento real, sin cambiar su comportamiento funcional.
- [X] T016 [US1] Verificar que `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` no incluya Swagger UI, ReDoc UI, Scalar UI, HTML ni endpoints interactivos y que solo documente el contrato JSON v1.
- [X] T017 [US1] Ejecutar las pruebas de `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/` correspondientes a US1 y confirmar el MVP de documento consumible.

**Checkpoint**: `GET /openapi/v1.json` publica el contrato completo y estable del
backend sin UI ni endpoints de documentación adicionales.

---

## Fase 4: Historia de Usuario 2 - Validar y generar cliente tipado (P2)

**Objetivo**: validar el JSON con Redocly y generar un cliente C# smoke con NSwag
sin introducir herramientas en runtime.

**Prueba independiente**: ejecutar el script reproducible y comprobar lint limpio,
cliente C# generado y fallos explícitos cuando una herramienta o salida falta.

### Pruebas de US2

- [X] T018 [P] [US2] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiToolingTests.cs` para validar que `.redocly.yaml` existe con `security-defined: off` y `operation-4xx-response: warn`.
- [X] T019 [P] [US2] Añadir a `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiToolingTests.cs` una prueba que invoque `npx @redocly/cli lint --config .redocly.yaml`, falle cuando el JSON sea inválido y falle también cuando Redocly o Node.js no estén disponibles; no debe omitir silenciosamente la validación.
- [X] T020 [P] [US2] Añadir a `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiToolingTests.cs` una prueba de disponibilidad de `NSwag.ConsoleCore` y generación del cliente C# en `artifacts/openapi-client-smoke/`; la ausencia de la herramienta DEBE hacer fallar la validación obligatoria.

### Implementación de US2

- [X] T021 [US2] Corregir `support/scripts/generate-openapi-v1.ps1` para usar `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj`, `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` y `support/scripts/openapi-v1.nswag.json`, eliminando toda referencia a `RealtorApi`.
- [X] T022 [US2] Completar `support/scripts/generate-openapi-v1.ps1` con comprobaciones explícitas de Node.js, npx, Redocly, dotnet tool restore, códigos `$LASTEXITCODE`, existencia del JSON y existencia del cliente generado.
- [X] T023 [US2] Ejecutar `support/scripts/generate-openapi-v1.ps1` y comprobar lint Redocly exitoso y cliente C# generado en `artifacts/openapi-client-smoke/` sin cambios manuales al contrato.

**Checkpoint**: las herramientas offline validan y consumen el contrato; ninguna
forma parte de la aplicación runtime.

---

## Fase 5: Historia de Usuario 3 - Detectar drift sin UI runtime (P3)

**Objetivo**: detectar cualquier desalineación entre endpoints reales y paths/métodos
documentados, normalizando constraints y manteniendo runtime sin UI.

**Prueba independiente**: comparar el inventario correcto y ejecutar una variante
inducida con un path/método adicional o ausente; la primera pasa y la segunda falla.

### Pruebas de US3

- [X] T024 [P] [US3] Crear `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDriftTests.cs` para obtener `EndpointDataSource`, extraer métodos/patrones y normalizar `{id:guid}` a `{id}` antes de comparar contra `openapi/v1.json`.
- [X] T025 [P] [US3] Añadir a `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDriftTests.cs` un caso de drift inducido en memoria que elimine o agregue una operación y compruebe un fallo con diferencia detallada.
- [X] T026 [P] [US3] Añadir a `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDriftTests.cs` comprobación de que no existen rutas runtime de `/swagger`, `/redoc`, `/scalar` ni HTML de exploración.
- [X] T027 [US3] Añadir a `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDriftTests.cs` la exclusión controlada de `/openapi/v1.json` del inventario de negocio, evitando comparar el documento consigo mismo.

### Implementación de US3

- [X] T028 [US3] Implementar el comparador de inventario en `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDriftTests.cs` con diferencias de método/path legibles y normalización limitada a constraints de parámetros.
- [X] T029 [US3] Revisar `app/backend/src/NetRentManagerApi/Program.cs` y todos los slices bajo `app/backend/src/NetRentManagerApi/` para confirmar que no se registran Swagger UI, ReDoc UI, Scalar UI ni endpoints interactivos.
- [X] T030 [US3] Ejecutar las pruebas de drift de `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDriftTests.cs` con documento correcto y drift inducido, confirmando que el caso válido pasa y el inducido falla.

**Checkpoint**: el contrato no puede quedar obsoleto silenciosamente y runtime sigue
exponiendo únicamente el JSON estático.

---

## Fase 6: Regresión, reproducibilidad y cierre

**Propósito**: ejecutar toda la verificación, documentar herramientas y cerrar con
evidencia reproducible.

- [X] T031 [P] Registrar en `specs/009-open-api/quickstart.md` las versiones de Node.js, npm, Redocly, .NET y NSwag obtenidas por los comandos de prerrequisito.
- [X] T032 [P] Revisar `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` contra `specs/009-open-api/data-model.md` para confirmar inventario, schemas, `$ref`, bodies multipart y exclusión de UI.
- [X] T033 Ejecutar `dotnet build .\app\NetRentManager.sln --no-restore` para `app/NetRentManager.sln` y corregir únicamente errores en archivos trazados por esta iniciativa.
- [X] T034 Ejecutar `dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore` para `app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` y confirmar la suite existente más drift, runtime route, documento y tooling.
- [X] T035 Ejecutar `support/scripts/generate-openapi-v1.ps1` dos veces consecutivas y confirmar que ambas regeneraciones producen documento válido, lint correcto y cliente smoke.
- [X] T036 Consultar `GET /openapi/v1.json` desde `app/backend/src/NetRentManagerApi` y registrar HTTP 200, `Content-Type: application/json`, cinco operaciones y ausencia de UI.
- [X] T037 Inspeccionar `app/backend/src/NetRentManagerApi/` y confirmar que no se agregaron controllers, endpoints de UI, lógica de negocio, migraciones ni cambios de frontend.
- [X] T038 Registrar en `specs/009-open-api/quickstart.md`, en `## Evidencia de validación`, comandos, versiones, build, lint, NSwag, drift válido/inducido, respuesta runtime y ausencia de UI.
- [X] T039 Confirmar que todas las tareas de `specs/009-open-api/tasks.md` estén marcadas `[X]` solo después de build, tests, script, lint, NSwag, drift y evidencia completa; mantener la spec conforme al flujo Speckit.
- [X] T040 [P] Verificar que `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` versionado coincide byte a byte con la salida regenerada por build y hacer fallar la validación si existe drift del artefacto commiteado.
- [X] T041 [P] Añadir a `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDocumentTests.cs` una matriz explícita de respuestas esperadas para `/health`, listado, detalle, creación y actualización, incluyendo `200`, `201`, `400`, `404`, `413`, `415` y `500` según corresponda.
- [X] T042 [P] Añadir a `app/backend/tests/NetRentManagerApiTests/Infrastructure/OpenApi/OpenApiDocumentTests.cs` una validación de nombres exactos `PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails` y fallo ante schemas duplicados o variantes no autorizadas.

**Checkpoint final**: OpenAPI v1 se genera desde build, se publica como JSON estático,
se valida con Redocly/NSwag, detecta drift y no agrega UI runtime.

---

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Fase 1** no depende de otras fases.
- **Fase 2** depende de T001-T005 y bloquea las historias porque habilita el artefacto generado.
- **US1** depende de T006-T010 y entrega el MVP público.
- **US2** depende de US1 porque valida el JSON generado y lo consume con herramientas offline.
- **US3** depende de US1 porque compara el documento ya generado contra runtime.
- **Fase 6** depende de US1, US2 y US3.

### Dependencias de historias

- **US1** es el MVP y establece el archivo estático y el inventario contractual.
- **US2** depende de US1 para lintar y generar un cliente desde un documento real.
- **US3** depende de US1 para comparar runtime contra paths existentes; puede ejecutarse en paralelo con US2 si el JSON ya está generado.

### Oportunidades paralelas

- T001-T005 pueden ejecutarse en paralelo.
- T006-T007 y T010 pueden prepararse en paralelo; T008 depende del diseño de publicación.
- T011-T013 pueden ejecutarse en paralelo en distintos archivos de pruebas.
- T018-T020 pueden ejecutarse en paralelo.
- T024-T027 pueden ejecutarse en paralelo en el mismo archivo solo si se integran de forma secuencial antes de T028.
- T031-T032 pueden ejecutarse en paralelo; T033-T042 son validaciones ordenadas.

---

## Ejemplos de ejecución paralela

### US1 - MVP

```text
T011 OpenApiDocumentTests.cs: paths y métodos
T012 OpenApiDocumentTests.cs: schemas y $ref
T013 OpenApiDocumentTests.cs: bodies multipart

Después de configurar MSBuild y static files:
T014 generación del documento
T015 metadatos de endpoints
T016 exclusión de UI
T017 pruebas P1
```

### US2 - Herramientas offline

```text
T018 configuración Redocly
T019 lint inducido
T020 disponibilidad NSwag

Después de US1:
T021 corrección del script
T022 fallos explícitos
T023 ejecución completa
```

### US3 - Drift

```text
T024 inventario EndpointDataSource
T025 drift inducido
T026 ausencia de UI
T027 exclusión del documento

Después de preparar pruebas:
T028 comparador final
T029 revisión runtime
T030 suite drift
```

---

## Estrategia de implementación

### MVP primero: solo Historia de Usuario 1

1. Completar Fase 1 y Fase 2.
2. Generar `wwwroot/openapi/v1.json` durante build.
3. Publicarlo estáticamente en `/openapi/v1.json`.
4. Validar los cinco endpoints y schemas compartidos.

### Entrega incremental

1. Completar herramientas y configuración.
2. Entregar documento v1 público como MVP.
3. Añadir lint Redocly y cliente smoke NSwag.
4. Añadir drift y exclusión de UI.
5. Ejecutar dos regeneraciones, suite completa y registrar evidencia.

### Notas de ejecución

- Todas las tareas nuevas deben conservar el backend existente como fuente de verdad.
- No editar manualmente el JSON para resolver drift; corregir metadata/generación real.
- NSwag, Redocly y clientes generados nunca se agregan al runtime.
- No marcar `[X]` hasta verificar la tarea y registrar evidencia cuando corresponda.
