---
name: 015-signalr-interactive-server
agent: speckit.specify
---

Crea una spec llamada 015-signalr-interactive-server

/speckit.specify Habilitar interactividad de servidor en las páginas Blazor del frontend NetRentManagerWeb para que los botones de navegación funcionen.

## Contexto del problema

El frontend NetRentManagerWeb es un Blazor Web App. Por defecto, sus páginas se renderizan en modo Static SSR, por lo que los manejadores @onclick nunca se ejecutan: no existe un circuito interactivo (SignalR) que capture los eventos del navegador. Esto NO es un fallo del endpoint del backend ni del cliente Refit; los handlers del code-behind son correctos pero nunca se invocan.

Como consecuencia, hoy no funcionan:
- Los botones "Anterior" y "Siguiente" de la paginación en la página de listado de propiedades (componente PropertyList, alojado en Home.razor, ruta "/").
- El botón "Volver al listado" en la página de detalle de propiedad (componente PropertyDetail, alojado en PropertyDetailPage.razor, ruta "/properties/{Id}").
- El botón "Reintentar" en los estados de error de ambas páginas.

En Program.cs ya está registrada la interactividad de servidor mediante AddInteractiveServerComponents() y AddInteractiveServerRenderMode(), pero ninguna página declara @rendermode, por lo que ninguna opta por el modo interactivo.

## Resultado esperado

Aplicar el modo de render interactivo de servidor de forma granular, únicamente en las páginas host que contienen componentes interactivos, declarando @rendermode InteractiveServer en:
- Components/Pages/Home.razor
- Components/Pages/PropertyDetailPage.razor

No se debe habilitar interactividad global en App.razor ni en Routes. Las páginas que no requieren interactividad (por ejemplo Error y NotFound) deben permanecer en Static SSR.

## Criterios de aceptación

- Al pulsar "Siguiente" en el listado, se navega a la siguiente página de resultados y la UI se actualiza sin recargar el navegador.
- Al pulsar "Anterior" en el listado, se navega a la página anterior y la UI se actualiza sin recargar el navegador.
- Los botones "Anterior"/"Siguiente" respetan su estado disabled según CanGoPrevious/CanGoNext.
- Al pulsar "Volver al listado" en el detalle (incluidos los estados NotFound y Error), se navega a la ruta "/".
- Al pulsar "Reintentar" en los estados de error de listado y detalle, se vuelve a ejecutar la carga de datos.
- Las páginas Error y NotFound siguen sin modo interactivo.
- No se introducen cambios en el backend, en los contratos Refit ni en la lógica de los code-behind existentes.

## Fuera de alcance

- Cambios en el backend o en los endpoints de la API.
- Migrar a InteractiveWebAssembly o Auto.
- Habilitar interactividad global en App.razor o Routes.
- Refactorizar los componentes PropertyList o PropertyDetail más allá de habilitar el render mode en sus páginas host.