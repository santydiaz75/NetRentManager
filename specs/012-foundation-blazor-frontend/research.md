# Investigación: Fundación del Frontend Blazor

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

## Decisión 1: Estado real del proyecto respecto a Bootstrap

**Decisión**: Tratar la ausencia de Bootstrap CSS/Icons como una condición ya
cumplida que debe preservarse y verificarse mediante pruebas, no como archivos
que eliminar.

**Justificación**: `NetRentManagerWeb` fue generado con la plantilla `dotnet new
blazor` de .NET 10, que ya no incluye `wwwroot/lib/bootstrap/` ni un enlace a
`bootstrap.min.css` en `App.razor`. Inspeccionar el proyecto confirma que no
existen dichas referencias hoy.

**Alternativas consideradas**: asumir que existe Bootstrap y crear tareas de
eliminación de archivos inexistentes, descartado porque generaría tareas sin
efecto real y confundiría la trazabilidad de `tasks.md`.

## Decisión 2: Paquete de Lucide Icons para Blazor

**Decisión**: Usar el paquete NuGet `BlazorBlueprint.Icons.Lucide`.

**Justificación**: Es uno de los paquetes de íconos Lucide para Blazor más
descargados y activamente mantenidos (actualizado recientemente), compatible
con `net10.0`, expone componentes Razor por ícono (SVG con `currentColor`) y
no depende de ningún framework CSS.

**Alternativas consideradas**: `Blazicons.Lucide` (igualmente popular, pero con
convención de uso distinta); `InfiniLore.Lucide` (requiere generadores de
código adicionales); construir SVGs manualmente sin paquete, descartado porque
duplicaría trabajo de mantenimiento que un paquete ya resuelve.

## Decisión 3: Estructura de comunicación con backend

**Decisión**: Registrar un único `HttpClient` nombrado hacia el backend
mediante `IHttpClientFactory` en `Program.cs`, leyendo la URL base desde
`ApiSettings:BaseUrl` en `appsettings.json`/`appsettings.Development.json`, sin
definir todavía ninguna interfaz Refit. Las interfaces Refit de features
futuras (ej. `IPropertiesApi`) se ubicarán en `Services/Api/{Feature}/` y se
registrarán con `AddRefitClient<TInterface>()` sobre el mismo `HttpClient`
nombrado.

**Justificación**: Cumple FR-008/FR-009 sin acoplar esta fundación a ningún
endpoint de negocio; deja un punto único de configuración de base address para
que las specs futuras (013, 014) solo agreguen su interfaz.

**Alternativas consideradas**: registrar `HttpClient` con URL hardcodeada en
código, descartado porque contradice la clarificación de leer la URL desde
configuración; posponer todo registro de `HttpClient` a la primera feature de
negocio, descartado porque el objetivo explícito de esta fundación es dejar el
andamiaje listo (FR-008).

## Decisión 4: Layout y navegación

**Decisión**: Reescribir `MainLayout.razor` con `.app-shell`/`.app-sidebar`/
`.app-main`/`.app-content`, extrayendo la navegación a un componente
`NavMenu.razor` propio en `Components/Layout/`, con enlaces "Inicio" y
"Propiedades" (apuntando a `/` y a una ruta vacía o `/`), según la
clarificación de la spec.

**Justificación**: Cumple FR-004/FR-005 y dejar una sidebar representativa de
las secciones futuras sin implementar lógica de negocio.

**Alternativas consideradas**: mantener un ítem de navegación único genérico
("Inicio"), descartado explícitamente en la clarificación de la spec a favor
de enlaces representativos de secciones futuras.

## Decisión 5: Puntos de quiebre responsive

**Decisión**: Definir dos puntos de quiebre en `app.css`: `tablet` (sidebar
compacta, con íconos y sin texto) por debajo de ~1024px, y `móvil` (navegación
colapsada tipo drawer, activada por un botón) por debajo de ~640px.

**Justificación**: Son puntos de quiebre convencionales para layouts
desktop-first con sidebar, consistentes con el estilo visual objetivo
(dashboards administrativos modernos) descrito en `frontend.instructions.md`.

**Alternativas consideradas**: un único punto de quiebre genérico, descartado
porque la spec exige comportamiento diferenciado en tablet y en móvil
(FR-006).
