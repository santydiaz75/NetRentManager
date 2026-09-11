# Plan de Implementación: Actualización de Estado de Propiedad

**Rama**: `010-update-status` | **Fecha**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/010-update-status/spec.md`

## Resumen

Agregar `PATCH /api/properties/{id}/status` como slice auto-descubrible de
Vertical Slice. El caso de uso aceptará un cuerpo JSON estricto con solo
`status`, normalizará `Available`, `Rented` y `Maintenance` sin distinguir
mayúsculas/minúsculas, actualizará únicamente la propiedad `Status` y devolverá
la propiedad completa con todos los demás datos conservados.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y SDK `10.0.400` fijado en `global.json`.

**Dependencias principales**: ASP.NET Core Minimal APIs, EF Core 10, Npgsql,
FluentValidation, `ISlice`, `IHandler`, `Result<T>`, ProblemDetails y mapping
explícito existentes.

**Persistencia**: PostgreSQL mediante `AppDbContext`; `Property.Status` usa el
enum persistente `PropertyStatus` con conversión textual. No se requieren cambios
de esquema ni migraciones.

**Pruebas**: xUnit, EF Core InMemory para handler y persistencia aislada, pruebas
de validator, mapping, contrato HTTP y regresión de conservación de campos.

**Plataforma objetivo**: ASP.NET Core .NET 10 ejecutado localmente y publicado
desde `app/backend/src/NetRentManagerApi`.

**Tipo de proyecto**: Web API Minimal API organizada por features y casos de uso.

**Objetivos de rendimiento**: una lectura y un `SaveChangesAsync` por solicitud,
sin cargar ni escribir imágenes; conservar la latencia de los endpoints existentes.

**Restricciones**: solo `status`; cuerpo JSON estricto; rechazar propiedades
adicionales con 400; no multipart, uploads, controllers, repositorios triviales,
registros manuales, migraciones ni cambios de frontend.

**Escala/Alcance**: un endpoint, un request, un response, un handler, un validator,
un mapping, pruebas focalizadas y un ejemplo `.http`.

## Comprobación de Constitución

*Puerta de control: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec está en `Borrador` y el plan no modifica su estado.
- **Aprobado**: se conservan .NET 10, Minimal APIs, Vertical Slice, EF Core,
  PostgreSQL, FluentValidation y ProblemDetails.
- **Aprobado**: el slice vivirá en `Features/Properties/UpdatePropertyStatus` y
  se registrará mediante `ISlice`, `IHandler` y el auto-descubrimiento existente.
- **Aprobado**: no se agregan controllers, repositorios triviales, migraciones,
  cambios de esquema, autenticación, frontend ni endpoints paralelos.
- **Aprobado**: el endpoint modifica solo `Status` y conserva `imageUrl` y el
  resto de la entidad.
- **Aprobado**: las pruebas y la evidencia de `quickstart.md` serán prerrequisitos
  para cualquier cierre posterior como `Implementada`.

No existen violaciones constitucionales que justificar.

## Estructura del proyecto

### Documentación de la funcionalidad

```text
specs/010-update-status/
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
├── Features/Properties/UpdatePropertyStatus/
│   ├── UpdatePropertyStatusSlice.cs
│   ├── UpdatePropertyStatusHandler.cs
│   ├── UpdatePropertyStatusRequest.cs
│   ├── UpdatePropertyStatusRequestValidator.cs
│   ├── UpdatePropertyStatusMapping.cs
│   └── UpdatePropertyStatusResponse.cs
└── NetRentManagerApi.http

app/backend/tests/NetRentManagerApiTests/
└── Features/Properties/UpdatePropertyStatus/
   ├── UpdatePropertyStatusHandlerTests.cs
   ├── UpdatePropertyStatusRequestValidatorTests.cs
   ├── UpdatePropertyStatusMappingTests.cs
   └── UpdatePropertyStatusContractTests.cs
```

**Decisión estructural**: el caso de uso se mantiene autocontenido bajo
`Features/Properties/UpdatePropertyStatus`; el handler accede directamente a
`AppDbContext` siguiendo el patrón local. No se crea una feature paralela ni se
modifican entidades, migraciones o registradores centrales.

## Investigación y decisiones

Las decisiones están consolidadas en [research.md](research.md):

- request JSON estricto con solo `status` y rechazo de propiedades adicionales;
- normalización case-insensitive a valores canónicos de `PropertyStatus`;
- búsqueda de la entidad por id, mutación única de `Status` y persistencia cancelable;
- response explícito reutilizando la forma de propiedad existente;
- mapping local para conservar todos los campos e `imageUrl`;
- validación OpenAPI generada desde metadatos del slice y ejemplo `.http` local.

## Diseño técnico

### Flujo del endpoint

1. `UpdatePropertyStatusSlice` registra `PATCH /api/properties/{id}/status` con
  metadatos de tags, summary, request body y respuestas 200/400/404/500.
2. El binding recibe `Guid id` y un request JSON estrictamente limitado a `status`.
3. El validator rechaza ausencia, null, vacío, valores no permitidos y propiedades
  JSON adicionales; compara sin distinguir mayúsculas/minúsculas.
4. El handler busca `Property` por id, devuelve `Error.NotFound` si no existe,
  normaliza el estado, cambia solo `property.Status` y ejecuta
  `SaveChangesAsync(cancellationToken)`.
5. El mapping transforma la entidad a response explícito, manteniendo título,
  descripción, dirección, precio, dimensiones e `imageUrl`.
6. Las excepciones inesperadas se registran con `ILogger` y se convierten al
  ProblemDetails interno ya existente.

### Contrato y validación

- `status` se serializa como texto y se persiste como uno de `Available`, `Rented`
  o `Maintenance`.
- El contrato no contiene multipart, `image`, `imageUrl` como entrada ni campos
  editables adicionales.
- El endpoint se auto-descubre mediante `ISlice`; el handler y validator mediante
  los registros existentes.
- La generación OpenAPI de build debe documentar solo la operación PATCH nueva y
  no alterar la semántica de endpoints previos.

## Fases de implementación previstas

1. Crear request, response, mapping y validator del slice.
2. Crear handler con búsqueda por id, normalización, mutación exclusiva de Status,
  persistencia cancelable y manejo ProblemDetails.
3. Registrar el slice con metadatos OpenAPI y agregar el ejemplo PATCH al archivo
  `NetRentManagerApi.http`.
4. Añadir pruebas unitarias del validator, handler, mapping, conservación de
  campos, 400/404/500, cancelación y propiedades adicionales.
5. Ejecutar build, tests, generación OpenAPI y quickstart; verificar que no se
  generan migraciones ni archivos de imagen.

## Seguimiento de complejidad

No se requiere seguimiento adicional porque no existen violaciones constitucionales.

| Violación | Motivo | Alternativa simple descartada |
|---|---|---|
| No aplica | No hay violaciones constitucionales | No se introduce complejidad excepcional |

