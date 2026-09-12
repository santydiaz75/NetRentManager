# Quickstart: Interactividad de Servidor en Blazor

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

Esta guía valida que las páginas de listado y detalle usan interactividad de servidor de forma granular y que sus botones ejecutan acciones sin recarga completa.

## Prerrequisitos

- .NET 10 SDK instalado según `global.json`.
- Backend `NetRentManagerApi` ejecutándose con propiedades sembradas.
- Secret Manager configurado para el backend.
- `ApiSettings:BaseUrl` del frontend apuntando al backend.

## Ejecutar las aplicaciones

Terminal del backend:

```powershell
cd app/backend/src/NetRentManagerApi
dotnet run --launch-profile https
```

Terminal del frontend:

```powershell
cd app/frontend/src/NetRentManagerWeb
dotnet run --launch-profile https
```

Frontend esperado: `https://localhost:7208`.

## Validación automatizada

```powershell
dotnet build app/NetRentManager.sln --no-restore
dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore
```

Comprobar además que solo las páginas host esperadas contienen la declaración de render interactivo:

```powershell
Select-String -Path app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor,app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor -Pattern "@rendermode InteractiveServer"
Select-String -Path app/frontend/src/NetRentManagerWeb/Components/App.razor,app/frontend/src/NetRentManagerWeb/Components/Routes.razor,app/frontend/src/NetRentManagerWeb/Components/Pages/Error.razor,app/frontend/src/NetRentManagerWeb/Components/Pages/NotFound.razor -Pattern "@rendermode|InteractiveServer" -ErrorAction SilentlyContinue
```

Resultado esperado: dos coincidencias en la primera búsqueda y ninguna en la segunda.

## Validar paginación interactiva

1. Abrir `https://localhost:7208/` con más de una página de propiedades.
2. Confirmar que el documento contiene el circuito interactivo de Blazor.
3. Pulsar `Siguiente` y comprobar que el listado cambia sin recarga completa del navegador.
4. Pulsar `Anterior` y comprobar el mismo comportamiento.
5. Confirmar que los controles permanecen deshabilitados en los límites según sus banderas de navegación.

## Validar detalle, retorno y reintento

1. Desde el listado, abrir una propiedad mediante `Ver`.
2. Pulsar `Volver al listado` y comprobar la navegación a `/` sin recarga completa.
3. Detener el backend, abrir o recargar el listado y confirmar el estado de error.
4. Pulsar `Reintentar`, reiniciar el backend y comprobar que la carga se ejecuta nuevamente.
5. Repetir la comprobación en el detalle, incluyendo un id inexistente, y confirmar que `Volver al listado` sigue disponible.

## Validar páginas estáticas

1. Abrir una ruta inexistente y comprobar que `NotFound` no crea un circuito interactivo propio.
2. Revisar `App.razor` y `Routes.razor`: no deben contener una habilitación global de interactividad.
3. Confirmar que el comportamiento de `Error` y `NotFound` no cambia.

## Evidencia de validación

**Fecha**: 2026-09-12

- `dotnet build app/frontend/src/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore`: correcto, 0 errores.
- `dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore`: 44 pruebas correctas, 0 errores.
- Las páginas `Home.razor` y `PropertyDetailPage.razor` emiten exactamente la declaración `@rendermode InteractiveServer`.
- `App.razor`, `Routes.razor`, `Error.razor` y `NotFound.razor` no contienen habilitación interactiva.
- Smoke check HTTP sobre `http://localhost:5178/`: `HTTP 200`, con `blazor.web.js` y `Blazor-Server-Component-State` presentes en la respuesta.
- La validación completa de navegador para paginación, retorno y reintento queda pendiente: el backend no estaba ejecutándose durante el smoke check y el código actual no contiene botones `Reintentar` ni manejadores `@onclick`; los controles actuales son enlaces HTML.
