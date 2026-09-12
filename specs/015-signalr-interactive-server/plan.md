# Plan de Implementación: Interactividad de Servidor en Blazor

**Rama**: `015-signalr-interactive-server` | **Fecha**: 2026-09-12 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/015-signalr-interactive-server/spec.md`

## Resumen

Habilitar el modo de render `InteractiveServer` únicamente en las dos páginas host que contienen acciones interactivas: `Home.razor` para el listado y `PropertyDetailPage.razor` para el detalle. La aplicación ya registra los componentes y el modo de render de servidor en `Program.cs`; por tanto, la implementación se limita a declarar el modo granular y a verificar que paginación, retorno y reintento funcionan mediante el circuito SignalR sin habilitar interactividad global.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y Razor Components.

**Dependencias principales**: Blazor Web App, `Microsoft.AspNetCore.Components`, render mode `InteractiveServer` y el circuito SignalR integrado de ASP.NET Core.

**Storage**: N/A. Esta iniciativa no modifica persistencia ni datos.

**Pruebas**: xUnit para pruebas estáticas y unitarias del frontend, más validación manual en navegador para confirmar hidratación, eventos y ausencia de recarga completa.

**Plataforma objetivo**: `NetRentManagerWeb` ejecutándose como Blazor Web App con render interactivo de servidor.

**Tipo de proyecto**: aplicación web frontend Blazor.

**Objetivos de rendimiento**: crear circuitos únicamente para `/` y `/properties/{id}`; no ampliar la interactividad a todas las rutas.

**Restricciones**: no modificar backend, endpoints, contratos Refit, code-behind, lógica de carga, `App.razor`, `Routes.razor`, `Error` ni `NotFound`; no usar Interactive WebAssembly ni Auto.

**Escala/Alcance**: dos declaraciones de render mode, pruebas de configuración y validación de cuatro flujos interactivos existentes.

## Constitución Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- La spec está en estado canónico `Aprobada` y el plan prepara la transición posterior a `En implementación` solo al iniciar `speckit.implement`.
- La iniciativa mantiene el stack obligatorio de Blazor Web App y Razor Components.
- La habilitación es granular y no introduce interactividad global en `App.razor` ni `Routes.razor`.
- No se modifican backend, contratos, persistencia ni lógica de negocio.
- `tasks.md` y `quickstart.md` serán requisitos de trazabilidad y evidencia antes de marcar la spec como `Implementada`.

## Estructura del proyecto

### Documentación

```text
specs/015-signalr-interactive-server/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── checklists/
│   └── requirements.md
└── tasks.md             # Se generará en la fase de tareas
```

### Código fuente

```text
app/frontend/src/NetRentManagerWeb/
├── Components/
│   ├── App.razor       # Se verifica que no reciba interactividad global
│   ├── Routes.razor    # Se verifica que permanezca estático
│   └── Pages/
│       ├── Home.razor  # Declara @rendermode InteractiveServer
│       ├── Error.razor # Se verifica Static SSR
│       └── NotFound.razor # Se verifica Static SSR
└── Features/
    └── Properties/
        └── Detail/
            └── PropertyDetailPage.razor # Declara @rendermode InteractiveServer

app/frontend/test/NetRentManagerWeb/
├── InteractiveServerRenderModeTests.cs
└── InteractiveServerBehaviorTests.cs
```

**Decisión estructural**: las declaraciones se colocan en las páginas host reales, no en el enrutador ni en el host global. Los componentes existentes y su code-behind permanecen sin refactorización funcional.

## Investigación y decisiones

Las decisiones se consolidan en `research.md`:

- `InteractiveServer` es el modo correcto porque la aplicación ya registra `AddInteractiveServerComponents()` y `AddInteractiveServerRenderMode()`.
- El ámbito de declaración es cada página host que necesita eventos: `Home.razor` y `Features/Properties/Detail/PropertyDetailPage.razor`.
- No se añade `@rendermode` a `App.razor`, `Routes.razor`, `Error.razor` ni `NotFound.razor`.
- Las pruebas automatizadas verifican la configuración y los handlers existentes; la ausencia de recarga completa se confirma manualmente en navegador.

## Diseño técnico

### Activación granular

1. Añadir `@rendermode InteractiveServer` a `Home.razor`.
2. Añadir `@rendermode InteractiveServer` a `PropertyDetailPage.razor`.
3. Mantener sin esa directiva `App.razor` y `Routes.razor`.
4. Mantener sin esa directiva las páginas `Error` y `NotFound`.

### Flujos que deben quedar habilitados

- En `Home.razor`, los controles existentes de paginación deben ejecutar `Anterior` y `Siguiente`, respetando sus estados deshabilitados.
- En el detalle, el retorno a `/` debe funcionar en contenido cargado, `404` y error.
- Los botones `Reintentar` existentes en listado y detalle deben volver a invocar la carga sin abandonar la página.
- La implementación no debe alterar los contratos Refit ni los code-behind, salvo que una corrección mínima de integración quede explícitamente trazada por `tasks.md`; la spec vigente exige reutilizarlos sin refactor funcional.

### Validación

- Pruebas unitarias o de inspección de archivos para confirmar exactamente dos declaraciones `InteractiveServer`.
- Pruebas de regresión para comprobar que `App.razor`, `Routes.razor`, `Error.razor` y `NotFound.razor` no reciben el modo interactivo.
- Build y tests del frontend.
- Validación manual con navegador y backend local para observar el circuito, paginación, retorno y reintento.

## Complejidad

No existen violaciones constitucionales ni complejidad adicional que justificar.
