# Investigación: Secret Manager para credenciales de conexión

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

## Decisión 1: Separar la cadena de conexión en información no sensible y credenciales

**Decisión**: `ConnectionStrings:DefaultConnection` en `appsettings.json` y
`appsettings.Development.json` conserva solo host, base de datos y opciones SSL
(`Host`, `Database`, `SSL Mode`, `Channel Binding`). El usuario y la contraseña
se leen por separado desde configuración externa.

**Justificación**: Resuelve la clarificación de la spec (Secret Manager guarda
solo usuario/contraseña, combinados con el resto que permanece en
`appsettings`). Minimiza duplicación: host/puerto/base de datos no cambian
entre desarrolladores ni entre entornos del mismo proveedor, mientras que las
credenciales sí son sensibles y varían por entorno.

**Alternativas consideradas**:
- Guardar la cadena de conexión completa como un único secreto: descartada en
  la clarificación por duplicar información no sensible en cada secreto y
  dificultar la lectura/mantenimiento.

## Decisión 2: Ensamblado con `NpgsqlConnectionStringBuilder`

**Decisión**: Usar `NpgsqlConnectionStringBuilder` para parsear la cadena base
y asignar `Username`/`Password` en runtime, dentro de
`InfrastructureServiceCollectionExtensions.AddInfrastructure`.

**Justificación**: Es la API oficial del driver Npgsql ya usado en el proyecto;
evita concatenación manual de strings propensa a errores y respeta el formato
de la cadena de conexión existente.

**Alternativas consideradas**:
- Concatenación manual de strings: descartada por ser frágil ante cambios de
  formato u orden de parámetros.

## Decisión 3: Clave de configuración dedicada para credenciales

**Decisión**: Usar la sección `DatabaseCredentials` con las claves `Username` y
`Password`, leídas mediante `IConfiguration` estándar de ASP.NET Core. Esto
permite que la misma clave se satisfaga con Secret Manager en desarrollo
(`dotnet user-secrets`), variables de entorno en producción
(`DatabaseCredentials__Username`) o un proveedor de configuración respaldado
por Azure Key Vault, sin cambios de código entre entornos.

**Justificación**: ASP.NET Core resuelve automáticamente `IConfiguration` desde
múltiples proveedores (user secrets, variables de entorno, Key Vault) usando la
misma clave jerárquica; esto es justamente lo que permite documentar ambas
opciones de producción sin priorizar una (FR-006) sin duplicar lógica de
lectura de configuración.

**Alternativas consideradas**:
- Reutilizar la clave `ConnectionStrings:DefaultConnection` para credenciales:
  descartada porque mezclaría información sensible y no sensible en la misma
  clave, dificultando su exclusión selectiva de logs/diagnóstico.

## Decisión 4: Falla explícita ante credenciales faltantes

**Decisión**: Lanzar `InvalidOperationException` con mensaje explícito
indicando qué configuración falta (cadena base, usuario o contraseña), antes de
registrar `AppDbContext`, siguiendo el patrón ya existente en
`InfrastructureServiceCollectionExtensions` para la cadena de conexión.

**Justificación**: Cumple FR-004 y el edge case de la spec: evita que la
aplicación intente conectarse silenciosamente con una cadena inválida o vacía.

**Alternativas consideradas**:
- Conectar con cadena incompleta y dejar que Npgsql falle en el primer uso:
  descartada porque retrasa el error hasta el primer request y dificulta el
  diagnóstico.

## Decisión 5: Nuevo patrón de archivo de overrides local en `.gitignore`

**Decisión**: El `.gitignore` ya excluye `appsettings.Local.json`, pero no
cubre variantes por entorno. Se agrega el patrón `appsettings.*.Local.json`
(manteniendo la convención de mayúscula `Local` ya usada en el repositorio),
para cubrir archivos como `appsettings.Development.Local.json`, nunca
trackeados en Git, como alternativa local opcional a Secret Manager.

**Justificación**: Resuelve la clarificación de la spec: un archivo ya
trackeado (`appsettings.Development.json`) no deja de versionarse solo por
agregarlo a `.gitignore`; se requiere una convención de nombre nueva y nunca
comprometida, consistente con la convención existente del repositorio.

**Alternativas consideradas**:
- Agregar `appsettings.Development.json` directamente al `.gitignore`:
  descartada porque el archivo ya está trackeado y seguiría presente en el
  historial y en clones existentes.

## Decisión 6: Documentación de producción sin priorizar un mecanismo

**Decisión**: Documentar en `quickstart.md`/README pertinente ambas opciones
para producción: variables de entorno de la plataforma de hosting y un
servicio externo como Azure Key Vault, dejando la elección al equipo según el
entorno de hosting.

**Justificación**: Resuelve la clarificación de la spec (FR-006); ambas
opciones son compatibles con la clave de configuración `DatabaseCredentials`
elegida en la Decisión 3, sin requerir cambios de código para alternar entre
ellas.

**Alternativas consideradas**:
- Recomendar un único mecanismo: descartada explícitamente en la clarificación
  para mantener flexibilidad según el entorno de hosting de cada despliegue.
