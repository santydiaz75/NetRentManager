# Plan de Implementación: OpenAPI v1 Público

**Rama**: `009-open-api` | **Fecha**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/009-open-api/spec.md`

## Resumen

Configurar la generación OpenAPI de ASP.NET Core durante el build del proyecto
`NetRentManagerApi`, producir `wwwroot/openapi/v1.json`, servirlo estáticamente
mediante `GET /openapi/v1.json` y validar el artefacto con Redocly y NSwag. La
fuente de verdad será el inventario real de Minimal APIs; no se añadirá Swagger UI,
ReDoc UI ni ningún explorador interactivo. Se incorporará una prueba de drift que
compare `EndpointDataSource` con el documento normalizando constraints de ruta.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10, SDK fijado por `global.json` y Node.js 20+
para las herramientas offline de validación.

**Dependencias principales**: `Microsoft.AspNetCore.OpenApi`, generación OpenAPI
MSBuild, `@redocly/cli` como dependencia NPM, `NSwag.ConsoleCore` como herramienta
local y xUnit para pruebas de drift.

**Persistencia**: sin cambios; el contrato se genera desde endpoints que consultan
PostgreSQL mediante las features existentes.

**Artefactos runtime**: `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json`
servido por `UseStaticFiles`; no se generan documentos al iniciar ni por request.
El archivo generado se commitea como salida auditable y el build debe permitir
detectar cualquier diferencia frente al artefacto versionado. La generación solo
está activa en configuración `Release`; los builds en `Debug` no la ejecutan.

**Pruebas**: xUnit con `WebApplicationFactory`/composición real cuando corresponda,
`EndpointDataSource`, parsing JSON del documento y procesos CLI para Redocly/NSwag.

**Plataforma objetivo**: ASP.NET Core .NET 10 en entorno local y publicación del
backend existente.

**Tipo de proyecto**: Web API Minimal API con Vertical Slice y contrato OpenAPI
estático versionado.

**Objetivos de rendimiento**: `GET /openapi/v1.json` debe servir un archivo estático;
la generación ocurre solo en build. La validación debe ser reproducible y el
inventario debe tener exactamente las cinco operaciones de negocio definidas.

**Restricciones**: no Swagger UI, ReDoc UI, Scalar UI, HTML, endpoints de
exploración, cambios de negocio, migraciones, frontend ni múltiples versiones.
La ausencia de Node.js, Redocly, NSwag o `.redocly.yaml` hace fallar las
validaciones obligatorias; no se permiten skips ambientales silenciosos.

**Escala/Alcance**: cinco endpoints existentes, un documento v1, un script de
regeneración, una configuración Redocly, una herramienta NSwag y pruebas de drift.

## Comprobación de Constitución

*Gate: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec está en `specs/009-open-api`, declara `Borrador` y el plan
  no modifica su estado.
- **Aprobado**: se mantienen .NET 10, Minimal APIs, Vertical Slice y ProblemDetails.
- **Aprobado**: la generación se integra en el proyecto existente y no crea una
  aplicación o registro paralelo.
- **Aprobado**: la documentación runtime se limita al JSON estático; no se agrega UI.
- **Aprobado**: `@redocly/cli` y NSwag permanecen fuera de runtime.
- **Aprobado**: no se modifica persistencia ni la lógica de negocio de los cinco
  endpoints.
- **Aprobado**: quickstart y tareas exigirán verificación de herramientas y evidencia
  antes de cualquier transición a `Implementada`.
- **Aprobado**: la spec distingue los tres artefactos canónicos de ejecución de
  los artefactos auxiliares trazables exigidos por esta feature.
- **Aprobado**: el JSON versionado se revisa como salida generada y no como fuente
  manual; el drift del artefacto bloquea el cierre.

No existen violaciones constitucionales que justificar.

## Investigación y decisiones

Las decisiones están consolidadas en [research.md](research.md):

- generación durante build con propiedades MSBuild de `Microsoft.AspNetCore.OpenApi`;
- archivo estático versionado servido por `UseStaticFiles` en cualquier entorno;
- exclusión de `MapOpenApi()` runtime para no crear una segunda ruta/documento;
- `.redocly.yaml` público con `security-defined: off` y `operation-4xx-response: warn`;
- NSwag en `dotnet-tools.json` solo para smoke client;
- drift mediante `EndpointDataSource` y normalización `{id:guid}` -> `{id}`;
- corrección del script para `NetRentManagerApi`, rutas y nombres del repositorio.
- el JSON generado se commitea como salida auditable y el build/validación detecta drift frente al archivo versionado;
- Redocly, NSwag y las pruebas de drift fallan de forma obligatoria cuando falta una herramienta requerida;
- pruebas de matriz de respuestas y nombres exactos de schemas compartidos.

## Estructura del proyecto

### Documentación de la funcionalidad

```text
specs/009-open-api/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── checklists/requirements.md
└── tasks.md                 # Lo generará /speckit.tasks
```

### Código y herramientas

```text
app/backend/src/NetRentManagerApi/
├── NetRentManagerApi.csproj       # generación OpenAPI en build
├── Program.cs                     # static file JSON; sin UI OpenAPI
└── wwwroot/openapi/v1.json        # artefacto generado y versionado

app/backend/tests/NetRentManagerApiTests/
├── Infrastructure/OpenApi/
│   ├── OpenApiDriftTests.cs
│   ├── OpenApiRuntimeRouteTests.cs
│   └── OpenApiToolingTests.cs
└── ... pruebas existentes

support/scripts/
├── generate-openapi-v1.ps1
└── openapi-v1.nswag.json

.redocly.yaml
dotnet-tools.json
package.json
package-lock.json
artifacts/openapi-client-smoke/
```

**Decisión estructural**: la implementación se mantiene en el backend existente;
las herramientas de validación viven en raíz/support y el cliente generado solo en
artifacts. No se agrega proyecto nuevo ni endpoint interactivo.

## Diseño técnico

### Generación y publicación

1. Configurar `Microsoft.AspNetCore.OpenApi`/MSBuild para generar el documento al
   compilar `NetRentManagerApi` en `wwwroot/openapi/v1.json`.
2. Asegurar que el JSON sea contenido estático copiado al output/publish.
3. Mantener `app.UseStaticFiles()` y retirar o no registrar `MapOpenApi()` runtime
   si genera una ruta adicional o contradice el documento único.
4. Verificar `GET /openapi/v1.json` en Development y no-Development con JSON 200.

### Contrato y schemas

El documento debe incluir las operaciones reales de health, listado, detalle,
creación y actualización. Los schemas compartidos se definirán mediante las
transformaciones OpenAPI disponibles en ASP.NET Core y se revisará que las
operaciones usen `$ref` para `PropertyStatus`, `ProblemDetails` y
`HttpValidationProblemDetails`, sin duplicación inline evitable. Los nombres de
estos tres componentes son estables y cualquier variante duplicada debe fallar.
La validación incluye una matriz de status codes por operación para comprobar
respuestas de éxito y error, no solo la presencia de paths.

### Drift

La prueba obtendrá `EndpointDataSource` desde la composición de la aplicación,
extraerá métodos y patrones de ruta, excluirá únicamente `/openapi/v1.json` si ya
aparece como archivo estático y normalizará constraints de parámetros. Comparará
el conjunto resultante con `paths` del JSON; una operación faltante, sobrante o con
método diferente fallará. El test de drift inducido trabajará sobre una copia en
memoria del documento, sin modificar el artefacto versionado.

### Validación offline

El script verificará Node.js, npm, npx, Redocly, dotnet y NSwag. Ejecutará build,
comprobará el archivo generado, ejecutará `npx @redocly/cli lint --config
.redocly.yaml`, restaurará herramientas y ejecutará NSwag hacia
`artifacts/openapi-client-smoke/`. Cada etapa comprobará `$LASTEXITCODE` y la
existencia de su salida.

## Fases de implementación previstas

1. Registrar NSwag, crear `.redocly.yaml` y confirmar `@redocly/cli` en NPM.
2. Configurar generación OpenAPI MSBuild y publicación estática v1; eliminar la
   ruta runtime condicionada de OpenAPI si crea duplicación.
3. Corregir `generate-openapi-v1.ps1` y crear su configuración NSwag.
4. Generar/revisar `wwwroot/openapi/v1.json` y comprobar operaciones/schemas.
5. Implementar pruebas de drift, runtime JSON, ausencia de UI y tooling CLI.
6. Añadir verificaciones de reproducibilidad contra el JSON commiteado, matriz de status codes y schemas estables.
7. Ejecutar quickstart completo y registrar evidencia de build, lint, NSwag y drift.

## Validación posterior al diseño

*Gate: re-evaluado tras crear los artefactos de investigación y diseño.*

- **Aprobado**: [research.md](research.md) resuelve generación, publicación, schemas,
  herramientas, drift y ausencia de UI.
- **Aprobado**: [data-model.md](data-model.md) define artefactos y relaciones de
  generación sin modificar persistencia.
- **Aprobado**: [quickstart.md](quickstart.md) fija prerrequisitos, comandos y
  evidencia reproducible.
- **Aprobado**: no quedan decisiones `NEEDS CLARIFICATION` ni placeholders.

## Complejidad

No hay violaciones constitucionales ni complejidad excepcional que justificar.
