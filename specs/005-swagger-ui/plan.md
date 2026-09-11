# Plan de Implementación: Swagger UI para el Backend

**Rama**: `005-swagger-ui` | **Fecha**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/005-swagger-ui/spec.md`

## Resumen

Integrar Swagger UI en `NetRentManagerApi` mediante `Swashbuckle.AspNetCore`,
disponible únicamente con `ASPNETCORE_ENVIRONMENT=Development`, y configurarla
para consumir el documento estático `GET /openapi/v1.json` generado por la
iniciativa `009-open-api`. La UI se servirá bajo `/swagger`; fuera de
`Development`, la ruta y sus recursos deben responder `404 Not Found`.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10, SDK fijado por `global.json`.

**Dependencias principales**: ASP.NET Core Minimal APIs, `Microsoft.AspNetCore.OpenApi`,
`Swashbuckle.AspNetCore` 10.2.3 ya referenciado, xUnit y `WebApplicationFactory`
para las pruebas de composición HTTP.

**Persistencia**: Sin cambios. Swagger UI consume el documento OpenAPI estático
generado en `app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json`.

**Pruebas**: xUnit con pruebas de composición HTTP y Playwright para verificar
rutas de middleware, carga de HTML, referencia al documento, interacción `Try it
out` y aislamiento por entorno. La spec autoriza estas pruebas porque la
funcionalidad es de runtime.

**Plataforma objetivo**: ASP.NET Core .NET 10 ejecutado localmente y publicado
desde `app/backend/src/NetRentManagerApi`.

**Tipo de proyecto**: Web API con Minimal APIs y Vertical Slice Architecture.

**Objetivos de rendimiento**: Servir los recursos de Swagger UI sin modificar la
latencia ni el comportamiento de los endpoints de negocio; la UI es una capacidad
de desarrollo y no se evalúa como ruta de producción.

**Restricciones**: no controllers, no endpoints de negocio nuevos, no documento
OpenAPI paralelo, no UI fuera de `Development`, no cambios en persistencia ni en
clientes Refit. `UseSwagger()` no debe publicar un segundo documento dinámico.

**Escala/Alcance**: Una UI, un documento OpenAPI v1, una ruta `/swagger`, un
proyecto backend y pruebas centradas en entorno y contrato.

## Comprobación de Constitución

*Puerta de control: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec declara `Borrador`; el plan no cambia estados ni añade
  trazabilidad de transición prematuramente.
- **Aprobado**: se conserva .NET 10, ASP.NET Core Minimal APIs, Vertical Slice y
  el proyecto backend existente.
- **Aprobado**: la UI se configura como middleware/infraestructura de `Program`,
  sin controllers, handlers ni lógica de dominio.
- **Aprobado**: Swagger UI consume el JSON estático de `009-open-api` y no crea
  una segunda fuente de contrato.
- **Aprobado**: el alcance restringe la UI a `Development`; fuera de ese entorno
  las rutas deben devolver 404.
- **Aprobado**: las pruebas de runtime estarán justificadas por la spec y se
  limitarán a la superficie HTTP de documentación.
- **Aprobado**: `quickstart.md` documentará evidencia de desarrollo y no desarrollo
  antes de permitir completar la iniciativa.

No existen violaciones constitucionales que justificar.

## Estructura del proyecto

### Documentación de la funcionalidad

```text
specs/005-swagger-ui/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── http-contracts.md
├── checklists/requirements.md
└── tasks.md             # Lo generará /speckit.tasks
```

### Código fuente

```text
app/backend/src/NetRentManagerApi/
├── NetRentManagerApi.csproj       # paquete Swashbuckle ya referenciado
├── Program.cs                     # registro condicional y UseSwaggerUI
└── wwwroot/openapi/v1.json        # documento estático consumido por la UI

app/backend/tests/NetRentManagerApiTests/
└── Infrastructure/Swagger/
   ├── SwaggerUiRuntimeTests.cs   # rutas y entorno
   └── SwaggerUiContractTests.cs  # referencia y contenido del documento
```

**Decisión estructural**: La integración se mantiene en `Program.cs` y en el
proyecto backend existente porque Swagger UI es infraestructura de documentación,
no un caso de uso de dominio. Las pruebas se ubican junto a las pruebas del backend
existentes. No se crea una feature slice ni un proyecto nuevo.

## Investigación y decisiones

Las decisiones están consolidadas en [research.md](research.md):

- registrar `AddSwaggerGen` solo para la infraestructura requerida por
  `Swashbuckle.AspNetCore`, sin publicar `UseSwagger()` como JSON adicional;
- configurar `UseSwaggerUI` con `SwaggerEndpoint("/openapi/v1.json", "NetRentManagerApi v1")`;
- envolver registro y middleware de Swagger UI en `if (app.Environment.IsDevelopment())`;
- dejar que el pipeline normal de endpoints produzca 404 fuera de `Development`;
- validar que el HTML de la UI referencia el documento estático y que el JSON
  contiene las operaciones ya definidas por `009-open-api`.

## Diseño técnico

### Registro y publicación

1. Mantener `Swashbuckle.AspNetCore` en el `.csproj`; no agregar otra librería de
  documentación ni cambiar la versión existente sin tarea explícita.
2. Registrar los servicios de generación/configuración necesarios para la UI sin
  publicar el endpoint JSON dinámico de Swashbuckle.
3. En `Program.cs`, registrar `UseSwaggerUI` solo en `Development` y apuntarlo a
  `/openapi/v1.json`.
4. Mantener `UseStaticFiles` y el pipeline OpenAPI estático de `009-open-api`.
5. Confirmar que `/swagger` carga HTML en desarrollo y que `/swagger` y sus assets
  devuelven 404 fuera de desarrollo.

### Contrato y fuente única

Swagger UI no generará ni versionará un segundo documento. El endpoint configurado
en la UI será el JSON estático de `009-open-api`; cualquier cambio en operaciones,
schemas o respuestas provendrá de ese contrato y de su generación de build.

### Pruebas

- composición `Development`: `/swagger` responde HTML y contiene la configuración
  de la UI;
- composición `Development`: la configuración referencia exactamente
  `/openapi/v1.json`;
- composición no `Development`: `/swagger`, `/swagger/index.html` y un recurso
  estático conocido de la UI responden `404`;
- contrato: el documento cargado contiene la versión v1 y las operaciones actuales;
- documento ausente o inválido: un `WebApplicationFactory` sustituye la respuesta
  del documento y una prueba verifica el mensaje de carga fallida;
- navegador: Playwright ejecuta `Try it out` sobre `GET /health` y comprueba la
  respuesta real mostrada por la UI;
- regresión: `/health` y una lectura de propiedades mantienen su respuesta HTTP.

## Fases de implementación previstas

1. Confirmar el contrato estático v1 y la versión existente de
  `Swashbuckle.AspNetCore`.
2. Registrar la infraestructura de Swagger UI y conectarla a
  `/openapi/v1.json`, con activación exclusiva en `Development`.
3. Añadir pruebas de runtime y contrato para desarrollo, no desarrollo y carga de
  recursos.
4. Actualizar `NetRentManagerApi.http` o la guía de validación si las tareas lo
  requieren, y registrar evidencia en `quickstart.md`.

## Seguimiento de complejidad

No se requiere seguimiento adicional porque no existen violaciones constitucionales.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| No aplica | No hay violaciones constitucionales | No se introduce complejidad excepcional |
