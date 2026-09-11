# Plan de Implementación: Alta de Propiedades

**Rama**: `006-properties-create` | **Fecha**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/006-properties-create/spec.md`

## Resumen

Agregar `POST /api/properties` como slice Vertical Slice con formulario
`multipart/form-data`, creación de propiedades con `imageUrl` nullable y upload
opcional PNG/JPG de hasta 5 MiB. El handler validará y almacenará el archivo con
nombre UUID, persistirá la propiedad mediante EF Core y eliminará el archivo si la
persistencia falla. La migración hará nullable `properties.image_url` y el listado
004 aceptará la ausencia de imagen sin responder 500.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y SDK `10.0.400` fijado en `global.json`.

**Dependencias principales**: ASP.NET Core Minimal APIs, EF Core 10 preview,
Npgsql, FluentValidation, `ISlice`, `IHandler`, `Result<T>` y
`ResultProblemDetailsMapper` ya existentes.

**Persistencia**: PostgreSQL mediante `AppDbContext`; una migración altera
`image_url` de requerido a nullable. El filesystem runtime sirve
`wwwroot/assets/properties`.

**Pruebas**: xUnit, EF Core InMemory para handler y validación aislada, directorios
temporales para uploads y pruebas de migración/mapping. No se agrega infraestructura
de integración externa sin tarea explícita.

**Plataforma objetivo**: ASP.NET Core sobre PostgreSQL, con ejecución local y
publish del proyecto `app/backend/src/NetRentManagerApi`.

**Tipo de proyecto**: Web API Minimal API con Vertical Slice Architecture.

**Objetivos de rendimiento**: validar cabeceras antes de copiar el cuerpo completo,
limitar cada archivo a 5 MiB, usar I/O asíncrono y evitar cargar la imagen en
memoria completa cuando se pueda copiar por stream.

**Restricciones**: no controllers, no repositorios triviales, no mappers automáticos,
no cambios frontend, no almacenamiento externo, no formatos distintos de PNG/JPG,
no nombres de archivo controlados por el cliente y no efectos parciales.

**Escala/Alcance**: una propiedad por solicitud, un upload opcional de máximo 5 MiB,
un archivo público por creación y una migración de nulabilidad.

## Investigación y decisiones

Las decisiones están consolidadas en [research.md](research.md):

- `ImageUrl` nullable en dominio, configuración y migración.
- Binding multipart explícito con `image` opcional.
- Validación por tamaño, extensión/MIME auxiliar y magic bytes.
- Nombre UUID con extensión normalizada y ruta pública relativa.
- Limpieza compensatoria ante fallo de persistencia, I/O o cancelación.
- Response 201 con `Location` y `status` textual.
- Listado 004 compatible con `imageUrl: null`.

## Comprobación de Constitución

*Gate: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec vive en `specs/006-properties-create`, declara `Borrador`
  y la planificación no altera su estado.
- **Aprobado**: el cambio permanece en la solución existente con .NET 10, Minimal
  APIs, EF Core, PostgreSQL, FluentValidation y ProblemDetails.
- **Aprobado**: el caso de uso vive en `Features/Properties/CreateProperty` y se
  registra mediante los scanners existentes de `ISlice`, `IHandler` y validators.
- **Aprobado**: el handler coordina dominio, DB y filesystem; el endpoint delega y
  el mapping solo transforma contratos.
- **Aprobado**: la migración es necesaria, única y queda trazada a una tarea; no se
  modifica la migración histórica de la spec 003.
- **Aprobado**: las pruebas son unitarias y `quickstart.md` define evidencia antes
  de permitir el estado `Implementada`.

No existen violaciones constitucionales que justificar.

## Estructura del proyecto

### Documentación de la funcionalidad

```text
specs/006-properties-create/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── http-contracts.md
├── checklists/
│   └── requirements.md
└── tasks.md                 # Lo generará /speckit.tasks
```

### Código fuente

```text
app/backend/src/NetRentManagerApi/
├── Domain/Properties/Property.cs
├── Features/Properties/CreateProperty/
│   ├── CreatePropertySlice.cs
│   ├── CreatePropertyHandler.cs
│   ├── CreatePropertyRequest.cs
│   ├── CreatePropertyRequestValidator.cs
│   ├── CreatePropertyMapping.cs
│   └── CreatePropertyResponse.cs
├── Infrastructure/Persistence/Configurations/PropertyConfiguration.cs
├── Infrastructure/Persistence/Migrations/
│   └── <migración de ImageUrl nullable>
├── Program.cs
└── wwwroot/assets/properties/

app/backend/tests/NetRentManagerApiTests/
├── Features/Properties/CreateProperty/
│   ├── CreatePropertySliceTests.cs
│   ├── CreatePropertyHandlerTests.cs
│   ├── CreatePropertyRequestValidatorTests.cs
│   ├── CreatePropertyMappingTests.cs
│   └── CreatePropertyErrorTests.cs
├── Infrastructure/Persistence/
│   └── PropertyImageNullabilityTests.cs
└── Features/Properties/ListProperties/
    └── regresión de imageUrl nullable en el listado existente
```

**Decisión estructural**: los contratos, validator, handler, slice y mapping quedan
juntos en el nuevo caso de uso. Solo se modifica infraestructura compartida para
la configuración/migración nullable y se ajusta el mapping de listado por una
dependencia funcional directa. No se crea `PropertyImageStorage` separado salvo que
la implementación demuestre reutilización real o reduzca complejidad verificable.

## Diseño técnico

### Flujo de creación

1. El slice registra `POST /api/properties` con formulario multipart y campo `image` opcional.
2. El filtro global valida `CreatePropertyRequest` antes de invocar el handler.
3. El handler valida presencia, tamaño, extensión/MIME y magic bytes del archivo si existe.
4. Genera `Guid + extensión` y copia el archivo al WebRoot runtime con token de cancelación.
5. Crea `Property` con `ImageUrl = null` o `/assets/properties/{nombre}` y guarda con `SaveChangesAsync`.
6. Devuelve `Result<CreatePropertyResponse>`; el slice mapea éxito a `Results.Created` y errores a ProblemDetails.
7. En cualquier fallo posterior a la creación del archivo, elimina el archivo generado y registra logging estructurado.

### Binding y validación

El request se declarará como entrada de formulario explícita para evitar inferencia
de JSON. `IFormFile? image` será opcional. Las reglas de campos viven en
`CreatePropertyRequestValidator`; las reglas de bytes, tamaño y nombre viven en el
handler o un componente local de I/O, no en el mapping.

### Persistencia y migración

`Property.ImageUrl` y la configuración EF pasarán a nullable. Se generará una sola
migración nueva que altere `image_url` y su snapshot, sin modificar la migración
histórica. Los registros semilla existentes conservarán sus URLs y la spec 004 se
probará con una propiedad sin imagen.

### Errores

- Validación de entrada o upload: `ValidationProblemDetails` HTTP 400.
- Fallo esperado de conflicto si alguna restricción futura lo exige: mapper común.
- Fallo de filesystem o DB después de validar: `Error.Internal` y `ProblemDetails` HTTP 500.
- Cancelación: propagar `OperationCanceledException`/token, ejecutar limpieza y no convertirla en éxito.

### Compatibilidad del listado

El mapping de `ListProperties` debe aceptar `ImageUrl == null`, producir `imageUrl:
null` y continuar devolviendo el item. Solo las rutas no nulas inválidas permanecen
como error interno.

## Fases de implementación previstas

1. Hacer nullable `Property.ImageUrl`, actualizar configuración y generar/revisar migración.
2. Crear request, response, validator y mapping explícito del nuevo slice.
3. Implementar validación de upload, nombre seguro, escritura y compensación en handler.
4. Registrar el slice y respuesta 201 con `Location` sin cambios manuales en scanners.
5. Ajustar el listado 004 y añadir pruebas de regresión para `imageUrl: null`.
6. Ejecutar build, tests, migración revisada y escenarios manuales; documentar evidencia.

## Validación posterior al diseño

*Gate: re-evaluado tras crear los artefactos de investigación y diseño.*

- **Aprobado**: [research.md](research.md) resuelve nulabilidad, multipart, validación,
  nombres, compensación, response, compatibilidad y estrategia de pruebas.
- **Aprobado**: [data-model.md](data-model.md) define request, upload, entidad nullable,
  response y efectos compensatorios.
- **Aprobado**: [contracts/http-contracts.md](contracts/http-contracts.md) fija multipart,
  201, 400, 500, `Location` e `imageUrl` nullable.
- **Aprobado**: [quickstart.md](quickstart.md) contiene pruebas automatizadas y ejemplos
  manuales de creación con y sin imagen.
- **Aprobado**: no quedan decisiones sin resolver ni placeholders de plantilla.

## Complejidad

No hay violaciones constitucionales ni complejidad excepcional que justificar.
