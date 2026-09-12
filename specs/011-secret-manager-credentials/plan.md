# Plan de Implementación: Secret Manager para credenciales de conexión

**Rama**: `011-secret-manager-credentials` | **Fecha**: 2026-09-12 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/011-secret-manager-credentials/spec.md`

## Resumen

Eliminar el usuario y la contraseña en texto plano de `appsettings.json` y
`appsettings.Development.json`, dejando en la cadena de conexión solo la
información no sensible (host, base de datos, SSL). El usuario y la contraseña
se leerán desde configuración externa (Secret Manager de .NET en desarrollo
local) y se combinarán en runtime con `NpgsqlConnectionStringBuilder` antes de
registrar `AppDbContext`. Se documentará el uso de Secret Manager para
desarrollo y de variables de entorno o Azure Key Vault para producción, y se
agregará al `.gitignore` un nuevo patrón de archivo de overrides local que
nunca debe trackearse en Git.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y SDK fijado en `global.json`.

**Dependencias principales**: ASP.NET Core Minimal APIs, EF Core 10, Npgsql
(`NpgsqlConnectionStringBuilder`), Secret Manager de .NET (`dotnet user-secrets`),
`IConfiguration` existente en `InfrastructureServiceCollectionExtensions`.

**Persistencia**: PostgreSQL mediante `AppDbContext`; no se requieren cambios de
esquema ni migraciones. Solo cambia el origen de las credenciales usadas para
construir la cadena de conexión.

**Pruebas**: xUnit para validar que el ensamblado de la cadena de conexión
combine correctamente la base no sensible con usuario/contraseña provistos por
configuración, y que falle con un mensaje claro cuando falten.

**Plataforma objetivo**: ASP.NET Core .NET 10 ejecutado localmente desde
`app/backend/src/NetRentManagerApi`.

**Tipo de proyecto**: Web API Minimal API existente; cambio de infraestructura
de configuración, sin nuevos endpoints.

**Objetivos de rendimiento**: sin impacto; el ensamblado de la cadena de
conexión ocurre una sola vez durante el arranque de la aplicación.

**Restricciones**: no modificar el esquema de base de datos, no introducir
controllers, no implementar integración real con Azure Key Vault (solo
documentarla), mantener el host/puerto/base de datos/SSL en `appsettings.json` y
`appsettings.Development.json`.

**Escala/Alcance**: dos archivos de configuración, un punto de registro de
`AppDbContext`, documentación de Secret Manager y de producción, y una entrada
de `.gitignore`.

## Comprobación de Constitución

*Puerta de control: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec está en `Borrador` y el plan no modifica su estado.
- **Aprobado**: se conservan .NET 10, Minimal APIs, EF Core, PostgreSQL/Npgsql;
  no se introducen frameworks nuevos.
- **Aprobado**: el cambio vive en `Infrastructure/DependencyInjection` existente,
  sin crear controllers ni carpetas técnicas genéricas nuevas para el dominio.
- **Aprobado**: no se agregan migraciones, cambios de esquema, autenticación,
  frontend ni endpoints nuevos.
- **Aprobado**: no se implementa Azure Key Vault; solo se documenta como opción
  de producción, junto con variables de entorno, según lo decidido en
  `Clarifications`.
- **Aprobado**: las pruebas y la evidencia de `quickstart.md` serán
  prerrequisitos para cualquier cierre posterior como `Implementada`.

No existen violaciones constitucionales que justificar.

## Estructura del proyecto

### Documentación de la funcionalidad

```text
specs/011-secret-manager-credentials/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── checklists/requirements.md
└── tasks.md             # Lo generará /speckit.tasks
```

### Código fuente

```text
app/backend/src/NetRentManagerApi/
├── appsettings.json                                  # Solo host/db/SSL, sin usuario/contraseña
├── appsettings.Development.json                       # Solo host/db/SSL, sin usuario/contraseña
└── Infrastructure/DependencyInjection/
    └── InfrastructureServiceCollectionExtensions.cs   # Ensambla la cadena de conexión con credenciales de configuración

app/backend/tests/NetRentManagerApiTests/
└── Infrastructure/DependencyInjection/
    └── InfrastructureServiceCollectionExtensionsTests.cs  # Ensamblado y fallo claro sin credenciales

.gitignore                                             # Nuevo patrón de overrides local (ej. appsettings.*.local.json)
```

**Decisión estructural**: el ensamblado de la cadena de conexión se mantiene en
`InfrastructureServiceCollectionExtensions`, punto único donde ya se registra
`AppDbContext`. No se crea una capa de configuración paralela ni se modifican
entidades de dominio.

## Investigación y decisiones

Las decisiones están consolidadas en [research.md](research.md):

- separación de la cadena de conexión en información no sensible (appsettings)
  y credenciales sensibles (Secret Manager en desarrollo);
- uso de `NpgsqlConnectionStringBuilder` para combinar ambas partes en runtime;
- clave de configuración dedicada para usuario y contraseña, leída mediante
  `IConfiguration` (compatible con Secret Manager, variables de entorno y
  Azure Key Vault sin cambios de código);
- documentación de Secret Manager para desarrollo y de variables de
  entorno/Azure Key Vault para producción;
- nuevo patrón de archivo de overrides local excluido en `.gitignore`.

## Diseño técnico

### Flujo de configuración

1. `appsettings.json` y `appsettings.Development.json` conservan
   `ConnectionStrings:DefaultConnection` solo con host, base de datos y opciones
   SSL, sin usuario ni contraseña.
2. `InfrastructureServiceCollectionExtensions.AddInfrastructure` lee la cadena
   base con `configuration.GetConnectionString("DefaultConnection")` y las
   credenciales desde una sección de configuración dedicada
   (`DatabaseCredentials:Username` y `DatabaseCredentials:Password`).
3. Si falta la cadena base o alguna credencial, se lanza
   `InvalidOperationException` con un mensaje claro indicando qué falta, antes
   de registrar `AppDbContext` (cumple FR-004).
4. Cuando todo está presente, se usa `NpgsqlConnectionStringBuilder` para
   combinar la cadena base con `Username` y `Password`, y el resultado se pasa a
   `UseNpgsql(...)`.
5. En desarrollo, `DatabaseCredentials:Username`/`Password` se configuran con
   `dotnet user-secrets` (Secret Manager), documentado en `quickstart.md`.
6. En producción, las mismas claves se proveen mediante variables de entorno
   (`DatabaseCredentials__Username` / `DatabaseCredentials__Password`) o un
   proveedor de configuración respaldado por Azure Key Vault; ambas opciones se
   documentan sin priorizar una (FR-006).
7. Se agrega al `.gitignore` el patrón `appsettings.*.Local.json` (nunca
   trackeado en Git), complementando el patrón `appsettings.Local.json` ya
   existente en el repositorio, como alternativa local documentada a Secret
   Manager (FR-007).

### Validación

- Pruebas unitarias verifican: ensamblado correcto de la cadena de conexión con
  credenciales presentes, y `InvalidOperationException` con mensaje claro
  cuando falta la cadena base, el usuario o la contraseña.
- `quickstart.md` documenta los pasos manuales para verificar que la aplicación
  arranca correctamente con Secret Manager configurado en desarrollo.

## Complexity Tracking

No existen violaciones constitucionales que justificar en esta iniciativa.

