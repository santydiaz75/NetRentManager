# Investigación: Interactividad de Servidor en Blazor

## Decisión 1: Habilitación granular del render interactivo

**Decisión**: Declarar `@rendermode InteractiveServer` únicamente en `Home.razor` y `PropertyDetailPage.razor`.

**Razonamiento**: La interactividad ya está registrada mediante `AddInteractiveServerComponents()` y `AddInteractiveServerRenderMode()`. El problema observado es que las páginas host no optan por el circuito interactivo. Aplicar el modo en esas dos páginas permite que sus eventos funcionen sin convertir toda la aplicación en interactiva.

**Alternativas consideradas**:

- Habilitar el modo en `Routes.razor`: descartado porque propagaría interactividad a todas las páginas enrutadas.
- Habilitar el modo en `App.razor`: descartado porque convertiría el host global y páginas sin acciones en componentes interactivos.
- Migrar a `InteractiveWebAssembly` o `InteractiveAuto`: descartado porque la spec exige servidor y el problema no requiere cambiar de modelo de ejecución.

## Decisión 2: Mantener la configuración global existente

**Decisión**: No modificar `Program.cs`, `App.razor`, `Routes.razor`, los contratos Refit ni los code-behind.

**Razonamiento**: La infraestructura de servidor ya existe y los manejadores de carga y navegación ya están implementados. El alcance aprobado consiste en activar el circuito en los hosts correctos y verificar el comportamiento.

**Alternativas consideradas**:

- Reconfigurar `Program.cs`: descartado porque no resuelve la ausencia de `@rendermode` en las páginas.
- Cambiar la lógica de paginación o carga: descartado por RF-009 y RF-010; introduciría cambios funcionales fuera del alcance.

## Decisión 3: Validación de interactividad

**Decisión**: Combinar pruebas estáticas de los hosts y una validación manual en navegador con el frontend y backend locales.

**Razonamiento**: Una prueba de texto puede confirmar que la declaración granular está presente y que la habilitación global no se añadió, pero solo la validación de navegador confirma la conexión del circuito y el comportamiento sin recarga completa.

**Alternativas consideradas**:

- Validar solo con `dotnet build`: descartado porque la compilación no demuestra que el circuito SignalR se conecte ni que los eventos se ejecuten.
- Modificar o añadir backend: descartado porque la feature es frontend-only.
