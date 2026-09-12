# Quickstart: Secret Manager para credenciales de conexión

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

Esta guía valida que un desarrollador nuevo pueda configurar sus credenciales
de base de datos localmente sin modificar archivos versionados, y que la
aplicación falle de forma clara si faltan.

## Prerrequisitos

- .NET 10 SDK instalado (según `global.json`).
- Acceso a las credenciales reales de la base de datos (host, usuario,
  contraseña) provistas por el equipo, fuera de este repositorio.

## Configurar Secret Manager en desarrollo

1. Ubicarse en el proyecto de la API:

   ```powershell
   cd app/backend/src/NetRentManagerApi
   ```

2. Inicializar Secret Manager si el proyecto aún no tiene un `UserSecretsId`:

   ```powershell
   dotnet user-secrets init
   ```

3. Configurar el usuario y la contraseña de la base de datos:

   ```powershell
   dotnet user-secrets set "DatabaseCredentials:Username" "<usuario-real>"
   dotnet user-secrets set "DatabaseCredentials:Password" "<contraseña-real>"
   ```

4. Ejecutar la aplicación:

   ```powershell
   dotnet run
   ```

**Resultado esperado**: la aplicación arranca correctamente y `AppDbContext` se
conecta a la base de datos usando la cadena de conexión no sensible de
`appsettings.Development.json` combinada con las credenciales de Secret
Manager.

## Validar el fallo explícito sin credenciales

1. Eliminar temporalmente una de las credenciales:

   ```powershell
   dotnet user-secrets remove "DatabaseCredentials:Password"
   ```

2. Ejecutar la aplicación:

   ```powershell
   dotnet run
   ```

**Resultado esperado**: la aplicación falla al iniciar con un
`InvalidOperationException` que indica explícitamente que falta la
contraseña de la base de datos, en lugar de intentar conectarse con una cadena
incompleta.

3. Restaurar la credencial eliminada antes de continuar trabajando:

   ```powershell
   dotnet user-secrets set "DatabaseCredentials:Password" "<contraseña-real>"
   ```

## Verificar que no hay credenciales en archivos versionados

```powershell
Select-String -Path appsettings.json,appsettings.Development.json -Pattern "Username|Password"
```

**Resultado esperado**: sin coincidencias en `ConnectionStrings:DefaultConnection`.

## Configurar credenciales en producción

Elegir una de las siguientes opciones según el entorno de hosting (ninguna se
prioriza sobre la otra):

- **Variables de entorno**: definir `DatabaseCredentials__Username` y
  `DatabaseCredentials__Password` en la plataforma de hosting.
- **Azure Key Vault** (u otro servicio externo de secretos): configurar el
  proveedor de configuración correspondiente para que exponga las mismas
  claves `DatabaseCredentials:Username` y `DatabaseCredentials:Password`.

## Uso opcional de archivo de overrides local

Si se prefiere un archivo local en vez de Secret Manager, usar el patrón
`appsettings.*.Local.json` (por ejemplo, `appsettings.Development.Local.json`),
que queda excluido en `.gitignore` junto con el ya existente
`appsettings.Local.json`, y nunca debe trackearse en Git.

## Evidencia de validación

**Fecha**: 2026-09-12

- `dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj`:
  140 pruebas, 0 con error (incluye las 4 nuevas de
  `InfrastructureServiceCollectionExtensionsTests`).
- `dotnet user-secrets set "DatabaseCredentials:Username" "..."` y
  `"DatabaseCredentials:Password" "..."` seguido de `dotnet run`: la aplicación
  arrancó correctamente (`Now listening on: http://localhost:5000`,
  `Application started`), conectada a la base de datos con las credenciales de
  Secret Manager.
- `dotnet user-secrets remove "DatabaseCredentials:Password"` seguido de
  `dotnet run`: la aplicación falló al iniciar con
  `System.InvalidOperationException: No se encontró 'DatabaseCredentials:Password'...`,
  confirmando el fallo explícito (FR-004). La credencial fue restaurada
  inmediatamente después.
- `Select-String -Path appsettings.json,appsettings.Development.json -Pattern "Username|Password"`:
  sin coincidencias, confirmando SC-001.

