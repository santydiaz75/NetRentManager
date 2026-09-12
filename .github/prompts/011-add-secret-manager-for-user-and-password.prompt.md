---
name: 011-add-secret-manager-for-user-and-password
agent: speckit.specify
---
/speckit.specify

Necesito implementación para modificar la cadena de conexión en appsettings.json y appsettings.Development.json ubicados en app/backend/src/NetRentManagerApi/, de modo que el usuario y la contraseña no estén hardcodeados (en texto plano).

Contexto del proyecto:

El proyecto es una aplicación ASP.NET Core Minimal API (.NET 10) ubicada en app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj.

Objetivo:

Implementar el uso de Secret Manager de .NET para leer las credenciales de forma segura en desarrollo. Para producción, documentar el uso de variables de entorno o un servicio externo como Azure Key Vault.

Alcance:

Añadir al .gitignore cualquier archivo local de configuración de desarrollo (por ejemplo appsettings.Development.json u otro archivo de overrides local) que pudiera contener credenciales, en caso de que se use como alternativa al Secret Manager, para asegurarme de que las credenciales locales nunca se suban a GitHub en el futuro.