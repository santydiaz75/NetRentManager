# Plan de Implementación: Detalle de Propiedad en Blazor

**Rama**: `014-blazor-property-detail` | **Fecha**: 2026-09-12 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/014-blazor-property-detail/spec.md`

## Resumen

Implementar una nueva página de detalle de propiedad en `NetRentManagerWeb` accesible en `/properties/{id}`, consumiendo `GET /api/properties/{id}` mediante un cliente Refit tipado ya registrado del proyecto. La pantalla reutiliza el diseño visual existente y la estructura de features del frontend, y añade un botón "Ver" en la tarjeta del listado principal para navegar hacia la propiedad seleccionada. La vista manejará explícitamente carga, contenido, propiedad no encontrada y errores del backend o red, además de permitir volver al listado principal.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10, Blazor Web App usando Razor Components sobre la base ya creada por la fundación del frontend.

**Dependencias principales**: `Refit`, `Refit.HttpClientFactory`, `Microsoft.AspNetCore.Components`, `System.Text.Json` y Lucide Icons si se usan íconos en la vista.

**Storage**: N/A. La feature es de solo lectura y consume el backend existente desde el cliente Refit del frontend.

**Pruebas**: xUnit para el proyecto de frontend, cubriendo la interfaz Refit, el page model de detalle, la navegación del botón “Ver” y la verificación de los estados visuales clave.

**Plataforma objetivo**: sitio web Blazor ejecutado localmente con `NetRentManagerApi` como backend y usando el contrato del OpenAPI ya publicado.

**Tipo de proyecto**: aplicación web frontend Blazor con una nueva pantalla de detalle sobre la base de la feature 012.

**Objetivos de rendimiento**: una sola llamada HTTP por detalle, navegación por ruta y sin llamadas adicionales a APIs en la UI.

**Restricciones**: sin `HttpClient` directo en componentes, sin `RestService.For<T>()` fuera del registro central, sin frameworks CSS ni estilos inline, sin cambios del backend, sin edición ni cambio de estado desde la vista.

**Escala/Alcance**: se añadirá una nueva ruta y página, una extensión del cliente Refit para `GetPropertyById`, un componente de detalle y una modificación mínima del listado principal para incluir el botón “Ver”.

## Constitución Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- La spec declara un estado canónico válido y el plan respeta la transición permitida para la fase actual.
- El plan incluye validación de `tasks.md` y evidencia en `quickstart.md` antes de permitir el estado `Implementada`.
- Se mantiene el stack obligatorio de la constitución: Blazor Web App, Razor Components, CSS propio, Refit e íconos Lucide.
- La feature cumple con el alcance frontend-only y no introduce controllers ni lógica de negocio en componentes UI.

## Estructura del proyecto

### Documentación (esta funcionalidad)

```text
specs/014-blazor-property-detail/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
├── checklists/
│   └── requirements.md
└── tasks.md             # Se generará en la fase de tareas
```

### Código fuente (repositorio)

```text
app/frontend/src/NetRentManagerWeb/
├── Components/
│   └── Pages/
│       ├── Home.razor
│       └── Home.razor.cs
├── Features/
│   └── Properties/
│       ├── Detail/
│       │   ├── PropertyDetailPage.razor
│       │   └── PropertyDetailPage.razor.cs
│       └── List/
│           ├── PropertyCard.razor
│           └── PaginationControls.razor
├── Services/
│   └── Api/
│       ├── NetRentManagerApiClientRegistration.cs
│       └── Properties/
│           ├── IPropertiesApi.cs
│           ├── PropertyDetailResponse.cs
│           ├── PagedPropertiesResponse.cs
│           ├── PropertyListItem.cs
│           └── PropertiesApiClientRegistration.cs
├── wwwroot/
│   └── app.css
└── Program.cs

app/frontend/test/NetRentManagerWeb/
├── PropertiesApiClientRegistrationTests.cs
├── PropertyDetailPageTests.cs
├── PropertyCardNavigationTests.cs
└── PropertyDetailStylesTests.cs
```

**Decisión estructural**: la interfaz Refit y los DTOs del frontend permanecerán bajo `Services/Api/Properties/`, mientras que la vista de detalle y la lógica de carga vivirán bajo `Features/Properties/Detail`. La tarjeta del listado se mantiene en `Features/Properties/List`, con una modificación mínima para añadir el botón “Ver” y la navegación a `/properties/{id}`.

## Investigación y decisiones

Las decisiones quedan consolidadas en `research.md` y se basan en el contrato `PropertyDetailResponse` del backend y la estructura ya creada por la feature 012:

- La ruta canónica es `/properties/{id}`; el id se recibe como parámetro de ruta y se valida por la API.
- La interfaz `IPropertiesApi` se ampliará con `GetPropertyByIdAsync(string id, CancellationToken cancellationToken = default)`, reutilizando la misma DI ya registrada con `IHttpClientFactory`.
- Para distinguir `404` del resto de errores, se evaluará `response.StatusCode` junto con `response.IsSuccessful` antes de renderizar la vista.
- Se usará un estado de detalle propio para “no encontrado”, con una separación clara de error general o red.
- La imagen nula se renderizará con un marcador oplaceholder visual consistente con el diseño del proyecto, sin introducir CSS inline ni otro framework visual.

## Diseño técnico

### Flujo de la página de detalle

1. `PropertyDetailPage.razor` declara la ruta `@page "/properties/{id}"` y recibe el parámetro `Id` desde la URL.
2. En el ciclo de inicialización, el page model llama a `PropertiesApi.GetPropertyByIdAsync(Id)`.
3. Según el resultado:
   - `404 NotFound` -> estado `not-found`.
   - `200 OK` y contenido no nulo -> estado `loaded` con el detalle renderizado.
   - `400`, `500`, red o cualquier otro error -> estado `error`.
   - mientras la solicitud está en curso -> estado `loading`.
4. La vista mostrará el título, la imagen, la descripción, la dirección, el precio, el estado, los dormitorios, los baños y los metros cuadrados.
5. Habrá una forma clara de volver al listado principal, bien a través de un enlace simple o por navegación con `NavigationManager`.

### Cliente Refit

1. La interfaz `IPropertiesApi` se ampliará con el método `Task<ApiResponse<PropertyDetailResponse>> GetPropertyByIdAsync(string id, CancellationToken cancellationToken = default);`.
2. La ruta se mapeará con `[Get("/api/properties/{id}")]` y el parámetro `id` se inyectará por la ruta del endpoint.
3. El DTO `PropertyDetailResponse` reutiliza el esquema ya documentado por OpenAPI y no se duplicará con un segundo contrato en la capa de UI.
4. El registro de DI existente se mantiene intacto y se reutiliza para todas las llamadas Refit del módulo de propiedades.

### Componente de tarjeta del listado

1. `PropertyCard.razor` recibirá el botón "Ver" con navegación a `/properties/{id}`.
2. La navegación se hará con `NavigationManager` o un enlace de navegación, sin lógica de negocio ni acceso directo a `HttpClient` en la tarjeta.
3. No se rediseña la tarjeta ni se alteran los datos que ya se muestran; solo se añade la acción de detalle.
4. La ruta se construye usando el id de la propiedad actual.

### Estilos y diseño

- Las vistas de carga, error y no encontrado usarán los estados definidos en el sistema visual o, si es necesario, clases nuevas añadidas a `wwwroot/app.css` con trazabilidad explícita en `tasks.md`.
- La página de detalle se integra visualmente con el diseño del proyecto sin introducir inline styles ni color hardcodeado.
- Si se usan íconos, se aplicará la convención Lucide con `currentColor` y clases CSS del sistema visual.

### Validación

- Pruebas unitarias para verificar:
  - el registro del cliente Refit con la nueva operación;
  - el manejo de estados `loading`, `loaded`, `error` y `not-found`;
  - la navegación del botón “Ver” desde la tarjeta del listado;
  - la presencia de las clases CSS necesarias para la vista de detalle.
- `quickstart.md` documentará la comprobación manual de una propiedad válida, una inexistente y una vuelta al listado principal.

## Complexity Tracking

No existen violaciones constitucionales que justifiquen una excepción en esta iniciativa.
