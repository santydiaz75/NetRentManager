# Plan de Implementación: Listado de Propiedades en Página Principal

**Rama**: `013-blazor-list-properties` | **Fecha**: 2026-09-12 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/013-blazor-list-properties/spec.md`

## Resumen

Convertir la ruta principal (`/`) de `NetRentManagerWeb` en un listado paginado
de propiedades consumido mediante un cliente Refit tipado (`IPropertiesApi`)
registrado sobre el `HttpClient` nombrado ya establecido en
`012-foundation-blazor-frontend`. La paginación se implementa mediante
navegación por query string (`?page=N&pageSize=M`) mediante enlaces `<a>`, no
`@onclick`, para funcionar sin depender de un modo de renderizado interactivo
(fuera de alcance de esta spec). La página cubre los estados de carga, vacío,
error y éxito con datos, y la grilla usa 3/2/1 columnas en escritorio/tablet/
móvil mediante clases nuevas en `wwwroot/app.css`.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10, Blazor Web App (Razor Components,
render mode Static SSR por defecto, sin cambios de render mode en esta spec).

**Dependencias principales**: `Refit`/`Refit.HttpClientFactory` (ya
referenciados desde `012-foundation-blazor-frontend`), `System.Text.Json`
con `JsonSerializerDefaults.Web` para que el cliente Refit deserialice el
JSON camelCase del backend.

**Storage**: N/A (esta spec no persiste datos; consume `GET /api/properties`
del backend existente).

**Pruebas**: xUnit en `NetRentManagerWebTests`, validando el registro del
cliente Refit, la lectura de parámetros de query, la lógica de estados
(carga/vacío/error/éxito) de forma aislada de la UI, y la presencia de las
clases de grilla responsive en `app.css`.

**Plataforma objetivo**: ASP.NET Core .NET 10, Blazor Web App renderizado en
modo Static SSR, ejecutado localmente desde
`app/frontend/src/NetRentManagerWeb`, consumiendo el backend
`NetRentManagerApi` existente.

**Tipo de proyecto**: Aplicación web Blazor (frontend) que agrega su primera
feature de negocio (listado de propiedades) sobre la fundación de
`012-foundation-blazor-frontend`.

**Objetivos de rendimiento**: una única solicitud HTTP por carga de página;
sin llamadas adicionales al navegar entre páginas de resultados.

**Restricciones**: sin `HttpClient` directo en componentes, sin
`RestService.For<T>()` fuera del registro central, sin frameworks CSS ni
íconos de terceros, sin CRUD ni filtros avanzados, sin cambios de backend ni
de render mode.

**Escala/Alcance**: una interfaz Refit, dos registros DTO de presentación, un
registro DI, la página `/` reescrita, dos componentes de apoyo (tarjeta de
propiedad y controles de paginación), y clases nuevas de grilla en `app.css`.

## Comprobación de Constitución

*Puerta de control: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec está en `Borrador` y el plan no modifica su estado.
- **Aprobado**: se mantiene el stack obligatorio: Blazor Web App, Razor
  Components, CSS propio en `wwwroot/app.css`, Refit, Lucide Icons.
- **Aprobado**: el consumo del backend usa Refit registrado con
  `IHttpClientFactory`; ningún componente usa `HttpClient` directamente.
- **Aprobado**: no se agregan controllers, lógica de negocio en el backend,
  migraciones ni cambios de contrato en `GET /api/properties`.
- **Aprobado**: no se introduce CRUD, filtros avanzados ni cambios de render
  mode (Interactive Server queda fuera de alcance, como en specs futuras).
- **Aprobado**: las pruebas y la evidencia de `quickstart.md` serán
  prerrequisitos para cualquier cierre posterior como `Implementada`.

No existen violaciones constitucionales que justificar.

## Estructura del proyecto

### Documentación (esta funcionalidad)

```text
specs/013-blazor-list-properties/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── checklists/requirements.md
└── tasks.md             # Lo generará /speckit.tasks
```

### Código fuente (repositorio)

```text
app/frontend/src/NetRentManagerWeb/
├── Services/
│   └── Api/
│       ├── NetRentManagerApiClientRegistration.cs   # Existente (012); sin cambios
│       └── Properties/
│           ├── IPropertiesApi.cs                     # Interfaz Refit: GetPropertiesAsync(page, pageSize)
│           ├── PagedPropertiesResponse.cs             # DTO de presentación (camelCase)
│           ├── PropertyListItem.cs                    # DTO de presentación (camelCase)
│           └── PropertiesApiClientRegistration.cs     # AddPropertiesApiClient(services)
├── Components/
│   └── Pages/
│       ├── Home.razor                                 # Página "/"; lee page/pageSize de query string
│       └── Home.razor.cs                              # Code-behind: carga datos, calcula estado actual
├── Features/
│   └── Properties/
│       └── List/
│           ├── PropertyCard.razor                     # Tarjeta individual de propiedad
│           └── PaginationControls.razor                # Enlaces Anterior/Siguiente + metadatos
└── wwwroot/
    └── app.css                                         # Agrega .properties-grid (3/2/1 columnas)

app/frontend/test/NetRentManagerWeb/
├── PropertiesApiClientRegistrationTests.cs
├── PropertiesListPageTests.cs
└── PropertiesGridStylesTests.cs
```

**Decisión estructural**: la interfaz Refit y sus DTOs de presentación viven en
`Services/Api/Properties/`, reutilizando el `HttpClient` nombrado registrado en
`012-foundation-blazor-frontend` (mismo nombre, vía el parámetro
`httpClientName` de `AddRefitClient<T>`), sin duplicar la lectura de
`ApiSettings:BaseUrl`. Los componentes de presentación específicos del listado
viven en `Features/Properties/List/`, siguiendo la organización por feature
sugerida por la spec; `Home.razor` permanece en `Components/Pages/` como
página enroutable, delegando la lógica de carga a su code-behind.

## Investigación y decisiones

Las decisiones están consolidadas en [research.md](research.md):

- paginación mediante enlaces de query string (`?page=N&pageSize=M`) en lugar
  de `@onclick`, para funcionar bajo el render mode Static SSR por defecto sin
  adelantar el alcance de una futura spec de interactividad;
- registro del cliente Refit sobre el mismo `HttpClient` nombrado de la
  fundación, usando `JsonSerializerDefaults.Web` para deserializar el JSON
  camelCase del backend;
- uso de `ApiResponse<PagedPropertiesResponse>` como tipo de retorno de Refit
  para distinguir éxito, error de validación y error interno sin exponer
  detalles técnicos en la UI;
- clases de grilla responsive (`.properties-grid`) reutilizando los puntos de
  quiebre ya definidos en `012-foundation-blazor-frontend` (1024px, 640px).

## Diseño técnico

### Flujo de la página

1. `Home.razor` declara `[SupplyParameterFromQuery]` para `page` y `pageSize`,
   con valores por defecto `1` y `6` cuando están ausentes.
2. En `OnInitializedAsync`, el code-behind invoca
   `IPropertiesApi.GetPropertiesAsync(page, pageSize, cancellationToken)`.
3. Según la respuesta:
   - Excepción de red o `ApiResponse` sin éxito → estado `error` (`.state-error`).
   - Éxito con `items` vacío → estado `empty` (`.state-empty`).
   - Éxito con `items` no vacío → estado de datos: grilla de `PropertyCard` +
     `PaginationControls`.
   - Mientras la solicitud está en curso (primer render antes de completar)
     → estado `loading` (`.state-loading`).
4. `PaginationControls` renderiza enlaces `<a href="/?page={page±1}&pageSize={pageSize}">`
   para "Anterior"/"Siguiente"; cuando `hasPrevious`/`hasNext` es `false`, se
   renderiza un elemento no enlazado (sin `href`) con apariencia deshabilitada.

### Cliente Refit

1. `IPropertiesApi.GetPropertiesAsync(int page, int pageSize, CancellationToken)`
   mapea a `GET /api/properties` con los parámetros de query `page`/`pageSize`
   (ASP.NET Core enlaza query strings sin distinguir mayúsculas/minúsculas).
2. `PropertiesApiClientRegistration.AddPropertiesApiClient` registra
   `AddRefitClient<IPropertiesApi>(settings, httpClientName: NetRentManagerApiClientRegistration.HttpClientName)`,
   reutilizando la base address ya configurada por
   `AddNetRentManagerApiClient` (012), con `RefitSettings` usando
   `SystemTextJsonContentSerializer` y `JsonSerializerDefaults.Web`.
3. `PagedPropertiesResponse`/`PropertyListItem` son registros de presentación
   (no reutilizan tipos del backend, que vive en otro proyecto), con
   propiedades en camelCase compatibles con el JSON real.

### Validación

- Pruebas unitarias verifican: registro del cliente Refit con la base address
  esperada; lectura de `page`/`pageSize` desde query string con valores por
  defecto; que la grilla responsive define 3/2/1 columnas en `app.css`.
- `quickstart.md` documenta los pasos manuales para verificar visualmente los
  cuatro estados (carga, vacío, error, datos) y la navegación entre páginas.

## Complexity Tracking

No existen violaciones constitucionales que justificar en esta iniciativa.

