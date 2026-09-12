# Feature Specification: Secret Manager para credenciales de conexión

**Feature Branch**: `011-secret-manager-credentials`

**Created**: 2026-09-12

**Estado**: Implementada

**Input**: User description: "Necesito implementación para modificar la cadena de conexión en appsettings.json y appsettings.Development.json ubicados en app/backend/src/NetRentManagerApi/, de modo que el usuario y la contraseña no estén hardcodeados (en texto plano). Contexto del proyecto: El proyecto es una aplicación ASP.NET Core Minimal API (.NET 10) ubicada en app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj. Objetivo: Implementar el uso de Secret Manager de .NET para leer las credenciales de forma segura en desarrollo. Para producción, documentar el uso de variables de entorno o un servicio externo como Azure Key Vault. Alcance: Añadir al .gitignore cualquier archivo local de configuración de desarrollo (por ejemplo appsettings.Development.json u otro archivo de overrides local) que pudiera contener credenciales, en caso de que se use como alternativa al Secret Manager, para asegurarme de que las credenciales locales nunca se suban a GitHub en el futuro."

## Clarifications

### Session 2026-09-12

- Q: ¿Qué mecanismo se debe documentar como opción principal para proveer credenciales en producción? → A: Documentar ambas opciones (variables de entorno y Azure Key Vault) sin priorizar una.
- Q: ¿Cómo debe ensamblarse la cadena de conexión al usar Secret Manager? → A: Secret Manager guarda solo usuario y contraseña, combinados con host/puerto/db/SSL que permanecen en appsettings.
- Q: ¿Se debe introducir un nuevo patrón de archivo de overrides local para el .gitignore, en vez de ignorar appsettings.Development.json (ya trackeado)? → A: Sí, introducir un nuevo patrón de archivo de overrides local que nunca se trackea en Git.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Eliminar credenciales en texto plano de los archivos versionados (Priority: P1)

Como desarrollador del proyecto, quiero que `appsettings.json` y `appsettings.Development.json` no contengan el usuario ni la contraseña de la base de datos en texto plano, para evitar exponer credenciales sensibles en el repositorio.

**Why this priority**: Es el riesgo de seguridad más crítico: las credenciales reales de la base de datos están actualmente expuestas en archivos versionados en el repositorio.

**Independent Test**: Se puede verificar inspeccionando el contenido de ambos archivos y confirmando que la cadena de conexión ya no incluye usuario ni contraseña reales.

**Acceptance Scenarios**:

1. **Given** el repositorio clonado, **When** se inspecciona `appsettings.json` y `appsettings.Development.json`, **Then** ninguno de los dos contiene usuario ni contraseña de base de datos en texto plano.
2. **Given** los archivos de configuración sin credenciales, **When** se revisa el historial de commits futuros, **Then** no se introducen nuevas credenciales en texto plano en dichos archivos.

---

### User Story 2 - Leer credenciales de forma segura en desarrollo mediante Secret Manager (Priority: P2)

Como desarrollador, quiero configurar el usuario y la contraseña de la base de datos usando Secret Manager de .NET, para que la aplicación se conecte correctamente en mi entorno local sin necesidad de modificar archivos versionados.

**Why this priority**: Sin este mecanismo, eliminar las credenciales de los archivos versionados dejaría a la aplicación sin forma de conectarse en desarrollo.

**Independent Test**: Se puede probar configurando las credenciales con `dotnet user-secrets` y arrancando la aplicación localmente, verificando que se conecta correctamente a la base de datos.

**Acceptance Scenarios**:

1. **Given** un desarrollador con Secret Manager configurado con las credenciales correctas, **When** ejecuta la aplicación en modo desarrollo, **Then** la aplicación se conecta exitosamente a la base de datos.
2. **Given** un desarrollador sin credenciales configuradas en Secret Manager, **When** ejecuta la aplicación en modo desarrollo, **Then** la aplicación falla al iniciar con un mensaje claro indicando que faltan las credenciales de conexión.

---

### User Story 3 - Documentar el manejo de credenciales en producción (Priority: P3)

Como responsable de despliegue, quiero contar con documentación clara sobre cómo proveer las credenciales de base de datos en producción, para configurar el entorno productivo de forma segura sin depender de Secret Manager.

**Why this priority**: Secret Manager solo aplica a desarrollo local; sin esta documentación el equipo no tendría una guía clara para producción, aunque la aplicación ya seguiría funcionando en desarrollo gracias a la User Story 2.

**Independent Test**: Se puede verificar revisando que la documentación del proyecto explica al menos una opción viable (variables de entorno o Azure Key Vault) para producción.

**Acceptance Scenarios**:

1. **Given** la documentación del proyecto, **When** un responsable de despliegue la consulta, **Then** encuentra instrucciones claras sobre cómo proveer las credenciales en producción mediante variables de entorno o un servicio externo como Azure Key Vault.

---

### Edge Cases

- ¿Qué sucede si el desarrollador olvida configurar Secret Manager? El sistema debe fallar al iniciar con un error explícito, en lugar de conectarse con una cadena de conexión vacía o inválida de forma silenciosa.
- ¿Qué sucede con las credenciales que ya fueron expuestas en el historial de Git antes de este cambio? Quedan fuera del alcance de esta spec la rotación automática o la reescritura del historial; se documenta como acción manual recomendada para el equipo.
- ¿Qué sucede si un archivo local de configuración de desarrollo (por ejemplo, un nuevo archivo de overrides local nunca trackeado en Git) contiene credenciales reales? Dicho archivo debe quedar excluido mediante `.gitignore` desde antes de crearse, para que nunca se suba al repositorio.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: `appsettings.json` y `appsettings.Development.json` NO DEBEN contener usuario ni contraseña de la base de datos en texto plano.
- **FR-002**: El sistema DEBE permitir proveer el usuario y la contraseña de la base de datos mediante Secret Manager de .NET (`dotnet user-secrets`) en el entorno de desarrollo local.
- **FR-003**: El sistema DEBE combinar la información no sensible de la cadena de conexión (host, puerto, base de datos, opciones SSL), que permanece en `appsettings.json`/`appsettings.Development.json`, con el usuario y la contraseña provistos por separado mediante Secret Manager, de forma que la aplicación se conecte correctamente en desarrollo.
- **FR-004**: El sistema DEBE fallar al iniciar con un mensaje de error claro cuando falten las credenciales requeridas para la conexión a la base de datos.
- **FR-005**: El repositorio DEBE incluir documentación que explique cómo configurar Secret Manager para un nuevo entorno de desarrollo.
- **FR-006**: El repositorio DEBE documentar ambos mecanismos posibles para proveer las credenciales en producción (variables de entorno de la plataforma de hosting y un servicio externo como Azure Key Vault), sin priorizar uno sobre el otro, dejando la elección a criterio del equipo según el entorno de hosting.
- **FR-007**: El repositorio DEBE definir un nuevo patrón de archivo de overrides local de configuración (por ejemplo, `appsettings.*.local.json`), que nunca haya sido trackeado en Git, y el `.gitignore` DEBE excluir dicho patrón para que, en caso de usarse como alternativa a Secret Manager, las credenciales locales nunca se suban al repositorio.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: `appsettings.json` y `appsettings.Development.json` versionados en el repositorio no contienen usuario ni contraseña de la base de datos en texto plano.
- **SC-002**: Un desarrollador nuevo puede configurar sus credenciales locales siguiendo la documentación provista en menos de 5 minutos.
- **SC-003**: La aplicación se conecta correctamente a la base de datos en desarrollo local usando únicamente las credenciales provistas por Secret Manager, sin requerir cambios en archivos versionados.
- **SC-004**: El 100% de los archivos locales de configuración que puedan contener credenciales quedan excluidos del control de versiones mediante `.gitignore`.

## Assumptions

- El proyecto backend `NetRentManagerApi` (ASP.NET Core Minimal API, .NET 10) es el único componente que actualmente maneja la cadena de conexión a la base de datos.
- La contraseña actualmente expuesta en `appsettings.json` y `appsettings.Development.json` ya fue commiteada en el historial de Git; su rotación queda como acción manual recomendada para el equipo y fuera del alcance de esta spec.
- Para producción, esta spec solo requiere documentar ambos enfoques (variables de entorno y Azure Key Vault); no requiere implementar la integración con un servicio externo de secretos.
- El entorno de desarrollo de todos los miembros del equipo soporta `dotnet user-secrets` (Secret Manager de .NET), incluido en el SDK de .NET 10.
