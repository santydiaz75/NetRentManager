# Especificación de la Funcionalidad: OpenAPI v1 Público

**Rama de la funcionalidad**: `009-open-api`

**Creado**: 2026-09-11

**Estado**: Implementada

**Trazabilidad de estado**:

- `Aprobada` -> `En implementación` | Motivo: inicio de `speckit.implement` tras validar análisis, checklist, plan y tareas refinadas | Fecha: 2026-09-11
- `En implementación` -> `Implementada` | Motivo: 42 tareas completadas, build correcto, 118 pruebas correctas, Redocly sin errores, NSwag generado, drift validado, runtime HTTP 200 y JSON reproducible registrado en `quickstart.md` | Fecha: 2026-09-11

**Entrada**: Descripción de usuario: "Crear una nueva spec para publicar un único documento OpenAPI v1 generado desde la implementación real de NetRentManagerApi, accesible mediante GET /openapi/v1.json, sin Swagger UI ni ReDoc UI."

## Clarificaciones

### Decisiones de contrato

- El documento versionado se generará desde los endpoints reales registrados en `EndpointDataSource`; no se aceptará editar manualmente `openapi/v1.json` como fuente de verdad.
- La generación de `openapi/v1.json` ocurrirá durante el build mediante la generación OpenAPI de ASP.NET Core/MSBuild; runtime solo servirá el archivo estático y no lo regenerará por request ni por startup.
- Las constraints de routing como `{id:guid}` se normalizarán a `{id}` al comparar inventario runtime y paths documentados.
- La API es pública y no requiere autenticación; `.redocly.yaml` deberá deshabilitar `security-defined` y convertir `operation-4xx-response` en warning.
- `@redocly/cli` y NSwag son herramientas CLI offline de desarrollo/validación y no forman parte de la superficie runtime.
- `openapi/v1.json` es un artefacto generado y versionado en el repositorio: debe quedar almacenado en Git, pero nunca se edita manualmente; cualquier cambio debe provenir de la implementación real y del build.
- Los tres artefactos canónicos de ejecución de la iniciativa siguen siendo `spec.md`, `plan.md` y `tasks.md`; `research.md`, `data-model.md`, `quickstart.md`, `.redocly.yaml`, manifests, scripts, pruebas y el JSON generado son artefactos auxiliares trazables exigidos por esta iniciativa, no iniciativas Spec-Driven independientes.
- La ausencia de Node.js, npx, Redocly, NSwag o `.redocly.yaml` en una validación obligatoria debe hacer fallar el proceso; no se permiten omisiones silenciosas ni tests verdes por skip ambiental.
- Los nombres `PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails` deben ser estables en `components/schemas`; si la generación automática usa otros nombres, la implementación deberá configurar los transformadores OpenAPI para producir esos nombres sin editar manualmente el JSON.

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Consumir el contrato OpenAPI v1 (Prioridad: P1)

Como consumidor o integrador de NetRentManagerApi, necesito descargar un documento OpenAPI v1 estable y completo, para conocer y generar clientes contra todos los endpoints HTTP públicos implementados.

**Por qué esta prioridad**: El documento es el contrato público central y debe ser consumible antes de cualquier validación visual o generación de clientes.

**Prueba independiente**: Compilar el backend, solicitar `GET /openapi/v1.json` y comprobar HTTP 200, `Content-Type: application/json`, versión v1 y los cinco paths normalizados: `/health`, `/api/properties`, `/api/properties/{id}`, `/api/properties` para POST/PUT según método y sus operaciones correspondientes.

**Escenarios de aceptación**:

1. **Dado** el backend compilado, **cuando** solicito `GET /openapi/v1.json`, **entonces** recibo HTTP 200 con JSON OpenAPI v1 y sin una interfaz HTML de exploración.
2. **Dado** el inventario real de endpoints, **cuando** genero el documento, **entonces** están representados el 100% de `GET /health`, `GET /api/properties`, `GET /api/properties/{id}`, `POST /api/properties` y `PUT /api/properties/{id}`.
3. **Dado** un endpoint con constraint `{id:guid}`, **cuando** comparo el inventario runtime con `v1.json`, **entonces** la comparación normaliza la constraint a `{id}` y no produce falso drift.
4. **Dado** el documento generado, **cuando** inspecciono sus schemas, **entonces** `PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails` aparecen como componentes reutilizables referenciados mediante `$ref`.

### Historia de Usuario 2 - Validar y generar un cliente tipado (Prioridad: P2)

Como desarrollador, necesito validar el contrato con herramientas CLI reproducibles y generar un cliente C# de prueba, para detectar documentos inválidos o incompletos antes de publicar cambios.

**Por qué esta prioridad**: La validación offline evita que un documento aparentemente correcto falle al consumirlo o al integrarse en automatizaciones.

**Prueba independiente**: Ejecutar el checklist de herramientas, `support/scripts/generate-openapi-v1.ps1`, `npx @redocly/cli lint` y NSwag; comprobar que el documento valida y que se genera un cliente tipado bajo `artifacts/openapi-client-smoke/` sin correcciones manuales.

**Escenarios de aceptación**:

1. **Dado** Node.js 20+, `npx`, `@redocly/cli` y NSwag restaurado, **cuando** ejecuto el script reproducible, **entonces** se compila el backend, se genera `wwwroot/openapi/v1.json`, Redocly valida y NSwag genera el cliente smoke.
2. **Dado** un documento OpenAPI inválido, **cuando** ejecuto `npx @redocly/cli lint --config .redocly.yaml`, **entonces** la validación falla con código de salida distinto de cero.
3. **Dado** un contrato consumible, **cuando** ejecuto `dotnet tool run nswag openapi2csclient` o el flujo equivalente configurado, **entonces** se genera código C# sin editar manualmente el documento.
4. **Dado** una máquina sin Node.js, npx o NSwag restaurable, **cuando** ejecuto la verificación previa, **entonces** el proceso falla temprano con un mensaje accionable y no simula una validación exitosa.

### Historia de Usuario 3 - Detectar drift sin UI runtime (Prioridad: P3)

Como responsable del backend, necesito que las pruebas detecten desalineación entre endpoints reales y `openapi/v1.json`, manteniendo el runtime sin Swagger UI ni ReDoc UI, para evitar contratos obsoletos sin ampliar la superficie de la aplicación.

**Por qué esta prioridad**: La detección automática protege la evolución del contrato, mientras la exclusión de UI mantiene el alcance deliberadamente estático y operativo.

**Prueba independiente**: Ejecutar pruebas de drift con el documento correcto y con un path/metodo inducido; comprobar que la primera pasa, la segunda falla y ninguna registra una UI interactiva.

**Escenarios de aceptación**:

1. **Dado** `EndpointDataSource` y `openapi/v1.json` alineados, **cuando** ejecuto la prueba de drift, **entonces** pasa comparando método y path con normalización de constraints.
2. **Dado** un documento al que se elimina un endpoint o se agrega uno inexistente, **cuando** ejecuto la prueba de drift, **entonces** falla indicando la diferencia.
3. **Dado** el backend en runtime, **cuando** consulto rutas de documentación, **entonces** solo existe `GET /openapi/v1.json`; no existen Swagger UI, ReDoc UI ni endpoints de exploración interactiva.
4. **Dado** cualquier endpoint con respuestas de error, **cuando** reviso el contrato, **entonces** `ProblemDetails` y `HttpValidationProblemDetails` se referencian consistentemente sin duplicación inline innecesaria.

### Casos límite

- La normalización debe transformar constraints ASP.NET Core como `{id:guid}` a `{id}` solo para comparación; no debe alterar rutas no equivalentes.
- Las operaciones `GET` y `POST` sobre `/api/properties` deben coexistir como dos operaciones del mismo path.
- Las operaciones `GET` y `PUT` sobre `/api/properties/{id}` deben coexistir como dos operaciones del mismo path normalizado.
- El endpoint `/health` debe documentarse aunque su response sea un contrato pequeño propio.
- Un endpoint nuevo o eliminado debe producir drift hasta regenerar el documento y verificarlo.
- El documento no debe publicarse como HTML ni incluir enlaces o assets de Swagger UI/ReDoc UI.
- El JSON debe ser servido como archivo estático desde `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json`.
- El script debe fallar si el archivo no se genera, si Redocly falla, si `dotnet tool restore` falla o si NSwag no genera el cliente esperado.
- La configuración Redocly debe permitir una API pública sin autenticación sin degradar otros errores de lint a éxito.
- El cliente NSwag debe generarse en un directorio de artifacts reproducible y no convertirse en parte del runtime.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El sistema DEBE generar durante el build un único documento OpenAPI versionado como `v1` desde la implementación real de Minimal APIs mediante ASP.NET Core/MSBuild, sin usar edición manual del JSON como fuente de verdad.
- **RF-002**: El documento DEBE ubicarse en `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` como archivo versionado del repositorio.
- **RF-003**: El backend DEBE servir estáticamente el documento generado mediante `GET /openapi/v1.json` con HTTP 200 y `Content-Type: application/json` en cualquier entorno soportado por la aplicación; no DEBE regenerarlo durante startup ni por request.
- **RF-004**: El documento DEBE cubrir el 100% de los endpoints implementados en el backend: `GET /health`, `GET /api/properties`, `GET /api/properties/{id}`, `POST /api/properties` y `PUT /api/properties/{id}`.
- **RF-005**: Cada operación DEBE describir método, path, parámetros de ruta y query, request body cuando aplique, responses de éxito y responses de error.
- **RF-006**: La comparación de inventario DEBE normalizar constraints del router como `{id:guid}` a `{id}` para verificar equivalencia contractual sin falsos positivos.
- **RF-007**: Los tipos compartidos `PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails` DEBEN exponerse bajo `components/schemas` y reutilizarse mediante `$ref`.
- **RF-008**: El formato de error ProblemDetails DEBE ser consistente entre las operaciones y distinguir respuestas de validación cuando corresponda.
- **RF-009**: El sistema DEBE incluir en el contrato los request bodies multipart de creación y actualización de propiedades, incluyendo sus campos y el archivo opcional `image` cuando corresponda.
- **RF-010**: El sistema NO DEBE incluir Swagger UI, ReDoc UI, Scalar UI, endpoints de exploración interactiva ni páginas HTML de documentación en runtime.
- **RF-011**: La aplicación DEBE exponer únicamente el documento JSON estático como superficie de documentación pública, sin registrar middleware o endpoints de UI.
- **RF-012**: La raíz del repositorio DEBE incluir `.redocly.yaml` con `security-defined: off` para API pública y `operation-4xx-response: warn`, manteniendo Redocly estricto para el resto de reglas.
- **RF-013**: El repositorio DEBE incluir `dotnet-tools.json` con `NSwag.ConsoleCore` registrado y restaurable mediante `dotnet tool restore` y ejecutable mediante `dotnet tool run nswag`.
- **RF-014**: La máquina de desarrollo DEBE verificar Node.js 20+ mediante `node --version`, npm mediante `npm --version`, npx disponible y `npx @redocly/cli --version` antes de validar el contrato.
- **RF-015**: El repositorio DEBE incluir `@redocly/cli` como dependencia NPM disponible para ejecución reproducible mediante `npx`, sin incorporarla al runtime del backend.
- **RF-016**: DEBE existir `support/scripts/generate-openapi-v1.ps1`, que ejecute en secuencia: build de `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` con generación OpenAPI de build, comprobación de `wwwroot/openapi/v1.json`, lint Redocly con `.redocly.yaml`, restore de herramientas y generación NSwag hacia `artifacts/openapi-client-smoke/`.
- **RF-017**: El script DEBE fallar con código distinto de cero y mensaje accionable si falta una herramienta, no se genera el documento, Redocly falla, NSwag falla o no aparece el cliente esperado.
- **RF-018**: DEBE existir una prueba xUnit de drift que obtenga el inventario runtime desde `EndpointDataSource`, normalice constraints y lo compare contra paths y métodos de `openapi/v1.json`.
- **RF-019**: La prueba de drift DEBE fallar cuando se elimine o agregue artificialmente una operación del documento respecto del inventario runtime.
- **RF-020**: La validación automatizada DEBE ejecutar Redocly lint y generar un cliente C# real mediante NSwag como smoke test, con prerrequisitos documentados.
- **RF-021**: El documento OpenAPI DEBE generarse de la implementación real y no debe contener endpoints fuera del inventario real, salvo la propia ruta estable `/openapi/v1.json` cuando se excluya explícitamente del inventario de negocio/documentación.
- **RF-022**: Todo código, configuración, herramienta, package manifest, script, documento generado, prueba y cambio de runtime DEBE quedar trazado a una tarea específica en `tasks.md`.
- **RF-023**: Las validaciones obligatorias de Redocly, NSwag y drift DEBEN fallar cuando falte una herramienta requerida; no DEBEN omitir la verificación ni marcarla como exitosa por condiciones ambientales.
- **RF-024**: Cada operación DEBE tener una matriz verificable de respuestas documentadas: `GET /health` y consultas exitosas con HTTP 200; listado inválido con HTTP 400; detalle inexistente con HTTP 404; creación con HTTP 201 y sus errores 400/500; actualización con HTTP 200 y sus errores 400/404/413/415/500.
- **RF-025**: La configuración de schemas DEBE garantizar exactamente los nombres `PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails` bajo `components/schemas`, y las pruebas DEBEN fallar si aparecen variantes duplicadas no autorizadas.

### Entidades clave

- **OpenAPI v1**: documento JSON versionado que representa el contrato HTTP público completo.
- **EndpointInventory**: inventario de métodos y paths expuestos por `EndpointDataSource`, normalizado para comparar constraints.
- **ProblemDetails**: schema compartido para errores HTTP públicos.
- **HttpValidationProblemDetails**: schema compartido para errores de validación.
- **PropertyStatus**: schema compartido para `Available`, `Rented` y `Maintenance`.
- **RedoclyConfiguration**: `.redocly.yaml` con reglas de validación para API pública.
- **OpenApiGenerationScript**: `support/scripts/generate-openapi-v1.ps1`, responsable de regenerar, validar y generar el cliente smoke.
- **OpenApiClientSmoke**: cliente C# generado por NSwag bajo `artifacts/openapi-client-smoke/`, sin uso en runtime.

## Requisitos técnicos y herramientas externas

### Requisitos de sistema

Antes de implementar y validar, la máquina DEBE disponer de:

- Node.js 20 o superior instalado y disponible en `PATH`, verificable con `node --version`.
- npm disponible, verificable con `npm --version`.
- npx disponible para ejecutar `@redocly/cli`.
- .NET SDK requerido por `global.json`.

### Herramientas NPM

- `@redocly/cli` DEBE estar disponible en el manifest NPM y poder ejecutarse con `npx @redocly/cli --version`.
- Redocly se usa únicamente como validador CLI offline; no se publica Redoc UI.

### Herramientas .NET locales

- `NSwag.ConsoleCore` DEBE estar registrado en `dotnet-tools.json`.
- Debe ser restaurable con `dotnet tool restore`.
- Debe poder ejecutarse con `dotnet tool run nswag`.
- NSwag se usa SOLO para generar un cliente C# tipado de prueba y verificar que el documento sea consumible sin correcciones manuales; no sirve Swagger ni ninguna UI.

### Archivo de configuración requerido

- `.redocly.yaml` DEBE existir en la raíz del repositorio.
- La configuración DEBE deshabilitar `security-defined` porque la API pública no requiere autenticación.
- La configuración DEBE degradar `operation-4xx-response` a `warn`.
- Las demás reglas Redocly DEBEN continuar estrictas salvo justificación explícita en la planificación.

### Verificación previa de herramientas

Antes de implementar o validar, ejecutar y registrar:

```powershell
node --version
npm --version
npx @redocly/cli --version
dotnet tool restore
dotnet tool list
```

La verificación debe confirmar Node.js `>= 20`, npx disponible y NSwag listado como herramienta restaurada.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **SC-001**: El 100% de los cinco endpoints implementados (`GET /health`, `GET /api/properties`, `GET /api/properties/{id}`, `POST /api/properties`, `PUT /api/properties/{id}`) aparece en `openapi/v1.json` con método y path correctos tras normalizar constraints.
- **SC-002**: `GET /openapi/v1.json` responde HTTP 200 con `Content-Type: application/json` en el 100% de las validaciones funcionales.
- **SC-003**: `npx @redocly/cli lint --config .redocly.yaml` termina sin errores en el 100% de ejecuciones válidas.
- **SC-004**: La prueba de drift pasa con el documento correcto y falla en el 100% de los casos de drift inducido de path o método.
- **SC-005**: `dotnet tool run nswag` genera un cliente C# bajo `artifacts/openapi-client-smoke/` sin correcciones manuales del contrato.
- **SC-006**: `support/scripts/generate-openapi-v1.ps1` regenera y valida `openapi/v1.json` de principio a fin sin éxito falso cuando una etapa falla.
- **SC-007**: `.redocly.yaml` existe, es válido y contiene `security-defined: off` y `operation-4xx-response: warn`.
- **SC-008**: No existe ninguna ruta de Swagger UI, ReDoc UI, Scalar UI o exploración interactiva en el inventario runtime de la aplicación.
- **SC-009**: Los schemas compartidos `PropertyStatus`, `ProblemDetails` y `HttpValidationProblemDetails` están definidos como components y son referenciados por `$ref` en las operaciones correspondientes.
- **SC-010**: La regeneración produce el mismo inventario de endpoints y documento válido después de dos ejecuciones consecutivas en la misma revisión.

## Suposiciones

- La implementación real del backend prevalece sobre contratos de specs anteriores; esta iniciativa solo representa y valida sus endpoints sin cambiar su comportamiento de negocio.
- La generación del documento es un artefacto de build mediante ASP.NET Core/MSBuild; la aplicación no genera OpenAPI durante startup ni bajo demanda.
- El proyecto backend es `app/backend/src/NetRentManagerApi`; el script no puede apuntar a proyectos o namespaces ajenos como `NetRentManagerApi`.
- El documento generado debe quedar bajo `wwwroot/openapi/v1.json` y la configuración actual de archivos estáticos se reutilizará para servirlo.
- `@redocly/cli` se mantendrá como dependencia de desarrollo del workspace y no se incluirá en artefactos runtime del backend.
- `artifacts/openapi-client-smoke/` es salida generada de validación y no forma parte de la superficie HTTP de la aplicación.
- La ruta `/openapi/v1.json` se excluye del inventario de endpoints de negocio para evitar comparar el documento consigo mismo.
- La feature soporta una única versión `v1`; no se implementan simultáneamente v2 ni negociación de versiones.
- No se agregan autenticación, autorización, UI, cambios de frontend ni cambios funcionales en los cinco endpoints existentes.

## Gobernanza refinada y versionado del artefacto

- La regla de tres archivos canónicos (`spec.md`, `plan.md`, `tasks.md`) se aplica a la iniciativa como unidad de ejecución; los artefactos auxiliares requeridos por la planificación, validación y tooling permanecen dentro de la misma carpeta o en las rutas técnicas establecidas, pero no crean nuevas iniciativas ni sustituyen los tres artefactos canónicos.
- `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json` se commitea y se revisa como salida generada. El build debe regenerarlo desde la implementación; la validación debe detectar cualquier diferencia entre la salida regenerada y el archivo versionado.
- La fuente de verdad es el código real y sus metadatos OpenAPI; el JSON versionado es una salida auditable y publicable, no una fuente de edición.
- Si el artefacto versionado no coincide con la salida del build, la validación falla antes de permitir completar la iniciativa.
- La matriz de respuestas se verifica tanto en el documento como en pruebas automatizadas para evitar contratos incompletos aunque los paths estén presentes.

## Compatibilidad y gobernanza

- Esta iniciativa depende de las specs `001-NetRentManager-solution-foundation`, `002-foundation-backend`, `003-properties-persistence-seeding`, `004-properties-list-pagination`, `005-swagger-ui`, `006-properties-create`, `007-properties-update` y `008-properties-get-by-id`.
- Debe respetar .NET 10, ASP.NET Core Minimal APIs, Vertical Slice, EF Core, PostgreSQL, FluentValidation y ProblemDetails de la constitución.
- Debe mantener las entidades y respuestas públicas existentes; la generación OpenAPI no autoriza cambios de contrato no descritos en las specs previas.
- Debe respetar el flujo Spec-Driven: esta spec precede a `plan.md`, `tasks.md` e implementación; todo artefacto nuevo queda trazado en `tasks.md`.
- El estado inicial es `Borrador`; las transiciones posteriores deben ser ejecutadas por Speckit con trazabilidad de origen, destino, motivo y fecha ISO.

## Fuera de alcance

- Swagger UI, ReDoc UI, Scalar UI, páginas HTML, exploradores interactivos y cualquier endpoint visual.
- Servir NSwag, Redocly o cualquier cliente generado en runtime.
- Cambiar lógica de negocio, persistencia, autenticación, autorización o frontend.
- Crear endpoints nuevos distintos de `GET /openapi/v1.json`.
- Mantener múltiples documentos o versiones simultáneas.
- Editar manualmente `openapi/v1.json` para corregir drift.
