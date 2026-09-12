# Modelo de Configuración: Secret Manager para credenciales de conexión

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

Esta iniciativa no introduce entidades de dominio ni cambios de esquema en
PostgreSQL. El único "modelo" relevante es la estructura de configuración usada
para ensamblar la cadena de conexión de `AppDbContext`.

## Configuración: Cadena de conexión base

**Ubicación**: `ConnectionStrings:DefaultConnection` en `appsettings.json` y
`appsettings.Development.json`.

**Campos permitidos (no sensibles)**:

| Campo | Descripción |
|---|---|
| `Host` | Servidor PostgreSQL (incluye pooler si aplica) |
| `Database` | Nombre de la base de datos |
| `SSL Mode` | Modo de validación SSL (ej. `VerifyFull`) |
| `Channel Binding` | Requisito de channel binding (ej. `Require`) |

**Reglas de validación**:
- NO DEBE contener `Username` ni `Password`.
- DEBE seguir siendo una cadena de conexión Npgsql válida al combinarse con las
  credenciales (Decisión 2 en `research.md`).

## Configuración: Credenciales de base de datos

**Ubicación**: sección `DatabaseCredentials` en configuración de ASP.NET Core
(Secret Manager en desarrollo, variables de entorno o Key Vault en producción).

**Campos**:

| Campo | Descripción | Origen en desarrollo | Origen en producción |
|---|---|---|---|
| `DatabaseCredentials:Username` | Usuario de la base de datos | `dotnet user-secrets` | Variable de entorno `DatabaseCredentials__Username` o Azure Key Vault |
| `DatabaseCredentials:Password` | Contraseña de la base de datos | `dotnet user-secrets` | Variable de entorno `DatabaseCredentials__Password` o Azure Key Vault |

**Reglas de validación**:
- Ambos campos son obligatorios; si falta alguno, la aplicación DEBE fallar al
  iniciar con un mensaje de error claro (FR-004), antes de registrar
  `AppDbContext`.
- Nunca deben persistirse en `appsettings.json` ni `appsettings.Development.json`.

## Configuración: Archivo de overrides local (opcional)

**Ubicación**: patrón `appsettings.*.Local.json`, excluido en `.gitignore`
(complementa el patrón `appsettings.Local.json` ya existente en el repositorio).

**Reglas**:
- Nunca debe trackearse en Git (FR-007).
- Es una alternativa local documentada a Secret Manager, no un requisito.

## Relación entre configuraciones

```text
ConnectionStrings:DefaultConnection (host, db, ssl)   [appsettings, versionado]
                    +
DatabaseCredentials:Username / Password               [Secret Manager / env vars / Key Vault, no versionado]
                    =
NpgsqlConnectionStringBuilder → cadena de conexión completa → AppDbContext.UseNpgsql(...)
```
