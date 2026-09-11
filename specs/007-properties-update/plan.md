# Plan de Implementación: Actualización de Propiedades

**Rama**: `007-properties-update` | **Fecha**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/007-properties-update/spec.md`

## Resumen

Agregar `PUT /api/properties/{id}` como slice Vertical Slice para reemplazar todos
los campos editables obligatorios de una propiedad existente. El archivo `image`
será opcional: cuando no se envíe se conservará `ImageUrl`; cuando se envíe se
validará por tamaño, extensión, MIME y magic bytes, se escribirá con nombre UUID y
se persistirá la nueva URL pública. La operación usará staging compensatorio para
que los fallos previos al commit no cambien la propiedad ni dejen un archivo nuevo
publicado. La imagen anterior se eliminará solo después del commit y un fallo de
esa limpieza se registrará sin revertir una actualización consistente.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y SDK `10.0.400` fijado en `global.json`.

**Dependencias principales**: ASP.NET Core Minimal APIs, EF Core 10, Npgsql,
FluentValidation, `ISlice`, `IHandler`, `Result<T>` y
`ResultProblemDetailsMapper` existentes.

**Persistencia**: PostgreSQL mediante `AppDbContext`; `Property.ImageUrl` ya es
nullable y `properties.image_url` ya fue migrada por `006`. La propiedad se
actualiza directamente con EF Core y `UpdatedAt` se establece en UTC.

**Almacenamiento de archivos**: `IWebHostEnvironment.WebRootPath` con destino
runtime `wwwroot/assets/properties`; la URL persistida es relativa bajo
`/assets/properties/` y `UseStaticFiles` continúa sirviendo el destino.

**Pruebas**: xUnit, EF Core InMemory, almacenamiento temporal controlado y dobles
para fallos de I/O y persistencia. Se cubrirán contrato, validator, handler,
mapping, compensación, cancelación y regresión de comportamiento.

**Plataforma objetivo**: API ASP.NET Core sobre PostgreSQL, ejecutable localmente
desde `app/backend/src/NetRentManagerApi`.

**Tipo de proyecto**: Web API Minimal API organizada por features con Vertical Slice.

**Objetivos de rendimiento**: cumplir p95 menor de 500 ms en una muestra funcional
de 100 actualizaciones válidas documentada en `quickstart.md`; validar tamaño y
cabecera antes de copiar el cuerpo completo y usar I/O asíncrono.

**Restricciones**: `PUT` es reemplazo completo de campos de negocio; `image` es la
única parte opcional. No se agrega control de concurrencia ni HTTP 409. No se usan
controllers, repositorios triviales, mappers automáticos, almacenamiento externo,
frameworks nuevos ni cambios de frontend.

**Escala/Alcance**: una propiedad por solicitud, un upload opcional de hasta 5 MiB,
un archivo temporal/nuevo por operación y dos rutas de compensación: fallo antes
del commit y limpieza posterior de la imagen reemplazada.

## Comprobación de Constitución

*Gate: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec está en `specs/007-properties-update`, declara el estado
  canónico `Borrador` y el plan no altera la transición de estado.
- **Aprobado**: el cambio permanece en la solución existente con .NET 10, Minimal
  APIs, EF Core, PostgreSQL, FluentValidation y ProblemDetails.
- **Aprobado**: el caso de uso vive en `Features/Properties/UpdateProperty` y se
  registra por auto-descubrimiento de `ISlice`, `IHandler` y validators.
- **Aprobado**: el endpoint solo enlaza y delega; el handler coordina el caso de
  uso y el mapping no contiene reglas de negocio ni I/O.
- **Aprobado**: la extensión de `ErrorType`/mapper es compartida y mínima, necesaria
  para cumplir los códigos 413 y 415 sin respuestas manuales paralelas.
- **Aprobado**: no se crea migración; la nulabilidad de `ImageUrl` pertenece a la
  spec 006 y el update reutiliza ese modelo.
- **Aprobado**: las pruebas unitarias y la evidencia de `quickstart.md` son
  prerrequisitos para cualquier transición posterior a `Implementada`.

No existen violaciones constitucionales que justificar.

## Investigación y decisiones

Las decisiones están consolidadas en [research.md](research.md):

- reutilizar el request y las reglas de upload de `006` sin acoplar el handler de
  creación;
- buscar la entidad antes de escribir y validar el reemplazo completo antes de
  mutar el estado tracked;
- escribir la imagen nueva con nombre UUID y eliminarla compensatoriamente ante
  cualquier fallo previo al commit;
- conservar la imagen anterior hasta confirmar la nueva URL y limpiar después;
- representar `413` y `415` con tipos de error comunes mapeados a ProblemDetails;
- mantener última escritura válida, sin concurrencia optimista ni `409`.

## Estructura del proyecto

### Documentación de la funcionalidad

```text
specs/007-properties-update/
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
├── Features/Properties/UpdateProperty/
│   ├── UpdatePropertySlice.cs
│   ├── UpdatePropertyHandler.cs
│   ├── UpdatePropertyRequest.cs
│   ├── UpdatePropertyRequestValidator.cs
│   ├── UpdatePropertyMapping.cs
│   └── UpdatePropertyResponse.cs
├── Infrastructure/Errors/
│   ├── ErrorType.cs
│   └── ResultProblemDetailsMapper.cs
└── wwwroot/assets/properties/

app/backend/tests/NetRentManagerApiTests/
├── Features/Properties/UpdateProperty/
│   ├── UpdatePropertySliceTests.cs
│   ├── UpdatePropertyHandlerTests.cs
│   ├── UpdatePropertyRequestValidatorTests.cs
│   ├── UpdatePropertyMappingTests.cs
│   └── UpdatePropertyErrorTests.cs
├── Infrastructure/Errors/
│   └── ErrorStatusMappingTests.cs
└── Features/Properties/ListProperties/
    └── pruebas de regresión de imageUrl nullable
```

**Decisión estructural**: el nuevo slice se mantiene autocontenido bajo
`Features/Properties/UpdateProperty`. Solo se toca infraestructura común para
los nuevos tipos de error HTTP y, si la implementación lo exige, el logger de
limpieza. No se modifica `PropertyConfiguration`, la migración de 006 ni el
frontend.

## Diseño técnico

### Flujo sin imagen

1. `UpdatePropertySlice` registra `PUT /api/properties/{id}` con binding
   `[FromForm]` y `IFormFile? Image`.
2. El validator comprueba que todos los campos editables estén presentes y sean
   válidos; la ausencia de `image` es válida.
3. El handler busca la propiedad por `id` antes de modificarla. Si no existe,
   devuelve `Error.NotFound` y no realiza I/O.
4. El handler aplica todos los campos editables y conserva exactamente
   `property.ImageUrl` cuando el archivo no está presente.
5. Establece `UpdatedAt`, ejecuta `SaveChangesAsync(cancellationToken)` y mapea
   la entidad actualizada a HTTP 200.

### Flujo con reemplazo de imagen

1. Después de encontrar la entidad y validar los campos, el handler valida tamaño,
   extensión/MIME auxiliar y magic bytes PNG/JPEG.
2. Genera `Guid.NewGuid():N` más `.png` o `.jpg`, crea el directorio runtime y
   copia el stream de forma asíncrona con `FileMode.CreateNew`.
3. Conserva la URL anterior en una variable de compensación, asigna la URL nueva
   a la entidad y guarda los cambios.
4. Si falla la copia, la persistencia, la cancelación o una validación posterior
   al staging, elimina únicamente el archivo nuevo y restaura los valores tracked
   anteriores antes de devolver el error.
5. Tras un commit correcto, intenta eliminar la imagen anterior solo si pertenece
   al destino público administrado por la aplicación; un fallo se registra y no
   convierte la respuesta exitosa en error.

### Contratos y errores

- Éxito: HTTP 200 con `UpdatePropertyResponse` y `imageUrl` nullable.
- Entidad ausente: HTTP 404 mediante `Error.NotFound`.
- Campos faltantes o inválidos y formulario mal formado: HTTP 400.
- Archivo vacío o contenido/formato no permitido: HTTP 415 mediante un nuevo
  tipo de error común, sin lógica de status en el slice.
- Archivo mayor de 5 MiB: HTTP 413 mediante un nuevo tipo de error común.
- Fallo inesperado de I/O, persistencia o compensación previa al commit: HTTP 500.
- Limpieza de imagen anterior después del commit: logging estructurado, respuesta
  200 conservada y sin revertir la referencia nueva.

## Fases de implementación previstas

1. Confirmar contratos de error `UnsupportedMediaType` y `PayloadTooLarge` y sus
   ProblemDetails, con pruebas unitarias del mapper.
2. Crear request, response, mapping y validator del slice de actualización.
3. Implementar búsqueda por id, reemplazo completo sin imagen, `UpdatedAt` y
   respuesta HTTP 200.
4. Implementar staging de imagen, UUID, validación real, rollback previo al
   commit y limpieza posterior de la imagen anterior.
5. Registrar el slice por auto-descubrimiento, ejemplos HTTP y pruebas de errores,
   cancelación, colisiones y última escritura válida.
6. Ejecutar build/tests, comprobar regresión del listado y documentar la muestra
   p95 y la evidencia funcional en `quickstart.md`.

## Validación posterior al diseño

*Gate: re-evaluado tras crear los artefactos de investigación y diseño.*

- **Aprobado**: [research.md](research.md) resuelve el binding, atomicidad,
  almacenamiento, errores HTTP, concurrencia y estrategia de pruebas.
- **Aprobado**: [data-model.md](data-model.md) define request, response, entidad
  mutada, staging y valores de compensación.
- **Aprobado**: [contracts/http-contracts.md](contracts/http-contracts.md) fija
  multipart, reemplazo completo, 200, 400, 404, 413, 415 y 500.
- **Aprobado**: [quickstart.md](quickstart.md) contiene comandos, escenarios
  automatizados, medición p95 y evidencia requerida para el cierre.
- **Aprobado**: no quedan decisiones `NEEDS CLARIFICATION` ni placeholders de
  plantilla.

## Complejidad

No hay violaciones constitucionales ni complejidad excepcional que justificar.
