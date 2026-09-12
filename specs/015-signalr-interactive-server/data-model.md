# Modelo de Datos: Interactividad de Servidor en Blazor

Esta iniciativa no agrega entidades persistentes, DTOs ni cambios de contrato HTTP. El modelo relevante es el estado de renderizado de las páginas host.

## Superficies de renderizado

| Superficie | Ruta | Renderizado esperado | Motivo |
|---|---|---|---|
| `Home.razor` | `/` | `InteractiveServer` | Contiene paginación y reintento interactivos. |
| `PropertyDetailPage.razor` | `/properties/{id}` | `InteractiveServer` | Contiene retorno y reintento interactivos. |
| `App.razor` | Host global | Static SSR | No debe propagar interactividad global. |
| `Routes.razor` | Enrutador | Static SSR | No debe convertir todas las páginas en interactivas. |
| `Error.razor` | `/Error` | Static SSR | Página sin acciones de esta iniciativa. |
| `NotFound.razor` | `/not-found` | Static SSR | Página sin acciones de esta iniciativa. |

## Estado interactivo

- **Circuito de servidor**: sesión de Blazor que procesa eventos de las páginas que declaran `InteractiveServer`.
- **Estado de listado**: datos y banderas existentes de `Home` (`IsLoading`, `HasError`, `Result`), sin cambios de modelo.
- **Estado de detalle**: datos y banderas existentes de `PropertyDetailPage` (`IsLoading`, `HasNotFound`, `HasError`, `Property`), sin cambios de modelo.
- **Estado de navegación**: URL de destino `/`, controlada por la navegación existente del frontend.

## Transiciones relevantes

1. Página host Static SSR -> circuito `InteractiveServer` al hidratarse la página declarada.
2. Listado cargado -> evento `Siguiente` o `Anterior` -> nueva carga y actualización del estado.
3. Error del listado o detalle -> evento `Reintentar` -> nueva carga.
4. Cualquier estado del detalle -> evento `Volver al listado` -> navegación a `/`.
5. Páginas `Error` y `NotFound` permanecen Static SSR en todo momento.
