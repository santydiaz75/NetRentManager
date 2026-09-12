# Quickstart: Listado de Propiedades en Página Principal

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

Esta guía valida que la ruta `/` de `NetRentManagerWeb` muestra el listado
paginado de propiedades, con sus cuatro estados y la navegación entre páginas.

## Prerrequisitos

- .NET 10 SDK instalado (según `global.json`).
- Backend `NetRentManagerApi` corriendo localmente con propiedades sembradas
  (ver `003-properties-persistence-seeding`).
- Secret Manager configurado en el backend (ver `011-secret-manager-credentials`).

## Preparar el backend

```powershell
cd app/backend/src/NetRentManagerApi
dotnet run
```

## Ejecutar el frontend

```powershell
cd app/frontend/src/NetRentManagerWeb
dotnet run
```

## Verificar el estado con datos

1. Abrir `/` en el navegador.
2. Confirmar que se muestran propiedades (título, precio, estado, imagen o
   marcador) y los metadatos de paginación (página actual, total de páginas,
   total de elementos).

## Verificar la navegación entre páginas

1. Con más de 6 propiedades sembradas, hacer clic en "Siguiente".
2. Confirmar que la URL cambia a `/?page=2&pageSize=6` (o el `pageSize`
   configurado) y que el listado muestra la segunda página.
3. Hacer clic en "Anterior" y confirmar que regresa a la página 1.
4. En la página 1, confirmar que "Anterior" aparece deshabilitado; en la
   última página, confirmar que "Siguiente" aparece deshabilitado.

## Verificar el estado vacío

1. Navegar a una página fuera de rango, por ejemplo `/?page=999&pageSize=6`.
2. Confirmar que se muestra el estado vacío (`.state-empty`), no un error.

## Verificar el estado de error

1. Detener el backend (`Ctrl+C` en su terminal).
2. Recargar `/` en el frontend.
3. Confirmar que se muestra el estado de error (`.state-error`), sin detalles
   técnicos expuestos (stack traces, URLs internas, excepciones crudas).
4. Reiniciar el backend antes de continuar.

## Verificar la grilla responsive

1. Con el backend corriendo y propiedades visibles, abrir las herramientas de
   desarrollo del navegador.
2. En un viewport de escritorio (≥ 1024px): confirmar 3 columnas.
3. En un viewport de tablet (~768px): confirmar 2 columnas.
4. En un viewport de móvil (~375px): confirmar 1 columna.

## Verificar ausencia de HttpClient directo y de terceros

```powershell
Select-String -Path app/frontend/src/NetRentManagerWeb/Components/**/*.razor,app/frontend/src/NetRentManagerWeb/Components/**/*.razor.cs -Pattern "new HttpClient\(|RestService\.For" -ErrorAction SilentlyContinue
```

**Resultado esperado**: sin coincidencias.

## Evidencia de validación

**Fecha**: 2026-09-12

- `dotnet build app/NetRentManager.sln --no-restore`: correcto, 0 errores.
- `dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj`:
  33 pruebas, 0 con error (incluye `PropertiesListPageTests`,
  `PropertiesGridStylesTests`, `PaginationControlsTests`).
- `dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj`:
  140 pruebas, 0 con error (sin regresiones en el backend).
- Backend (`http://localhost:5252`) + frontend (`http://localhost:5257`,
  `ApiSettings:BaseUrl` apuntando al backend anterior) ejecutados en paralelo:
  `GET /` devolvió 6 tarjetas de propiedad, metadatos "Página 1 de 3 · 13
  propiedades", enlace "Siguiente" habilitado (`href="/?page=2&pageSize=6"`)
  y "Anterior" deshabilitado (sin `href`) en la primera página.
- Íconos Lucide: 4 elementos `<svg>` renderizados (2 en la sidebar, 2 en los
  controles de paginación), sin texto literal `<LucideIcon>` sin renderizar.
- Grilla responsive: `.properties-grid` con `repeat(3, 1fr)` por defecto,
  `repeat(2, 1fr)` bajo `max-width: 1024px` y una columna bajo
  `max-width: 640px`, verificado en `app.css`.
- Ausencia de `HttpClient`/`RestService.For` directos en componentes,
  verificado por `ApiClientRegistrationTests`/`PropertiesListPageTests`.

### Bugs reales detectados y corregidos durante la validación manual

1. **Refit sin builder de reflexión**: `AddRefitClient<IPropertiesApi>` con
   `httpClientName` lanzaba `NotSupportedException` en runtime ("This
   interface needs the reflection request builder..."). Corregido agregando
   la referencia NuGet `Refit.Reflection` al proyecto, siguiendo la
   recomendación oficial del mensaje de error.
2. **Íconos Lucide sin renderizar en `Features/Properties/List/`**: los
   componentes bajo `Features/` no heredan el `@using
   BlazorBlueprint.Icons.Lucide.Components` declarado en
   `Components/_Imports.razor` (carpeta hermana, no ancestro). Corregido
   creando `Features/_Imports.razor` con ese `@using`, para que todos los
   componentes de esa carpeta lo hereden.
