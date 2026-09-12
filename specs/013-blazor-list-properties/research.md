# Investigación: Listado de Propiedades en Página Principal

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

## Decisión 1: Paginación mediante enlaces de query string, no `@onclick`

**Decisión**: Los controles "Anterior"/"Siguiente" se implementan como
elementos `<a href="/?page=N&pageSize=M">`, y `Home.razor` lee `page`/`pageSize`
mediante `[SupplyParameterFromQuery]`, no como botones con `@onclick` y estado
en memoria del componente.

**Justificación**: `NetRentManagerWeb` renderiza en modo Static SSR por
defecto (sin circuito interactivo); `@onclick` no se ejecuta sin un render
mode interactivo, cuya introducción es responsabilidad de una spec futura
dedicada (`015-signalr-interactive-server`). La navegación por enlaces
(incluida la navegación mejorada de Blazor) funciona de forma nativa bajo
Static SSR, sin depender de esa spec futura ni adelantar su alcance.

**Alternativas consideradas**: usar `@onclick` con estado local del
componente, descartado porque requeriría render mode interactivo fuera del
alcance de esta spec; usar formularios POST para paginar, descartado por ser
más complejo que enlaces `GET` para una operación idempotente de navegación.

## Decisión 2: Registro del cliente Refit sobre el mismo `HttpClient` nombrado

**Decisión**: `AddPropertiesApiClient` registra
`AddRefitClient<IPropertiesApi>(settings, httpClientName: NetRentManagerApiClientRegistration.HttpClientName)`,
reutilizando el `HttpClient` nombrado y su base address ya configurados por
`AddNetRentManagerApiClient` en `012-foundation-blazor-frontend`.

**Justificación**: Cumple exactamente lo previsto en el plan de la fundación
("features futuras agregan `AddRefitClient<TInterface>()` sobre el mismo
`HttpClient` nombrado, sin reconfigurar la base address").

**Alternativas consideradas**: registrar un `HttpClient` nombrado nuevo y
propio para `IPropertiesApi`, descartado por duplicar la lectura de
`ApiSettings:BaseUrl` sin necesidad.

## Decisión 3: Serialización JSON camelCase compatible con el backend

**Decisión**: Configurar `RefitSettings` con
`new SystemTextJsonContentSerializer(new JsonSerializerOptions(JsonSerializerDefaults.Web))`
para que el cliente Refit deserialice el JSON camelCase (`items`, `pageSize`,
`hasNext`, etc.) que produce el backend con las convenciones estándar de
Minimal APIs.

**Justificación**: El backend serializa con las convenciones web estándar de
ASP.NET Core (camelCase, case-insensitive); `JsonSerializerDefaults.Web`
replica exactamente ese comportamiento sin mapeos manuales por propiedad.

**Alternativas consideradas**: anotar cada DTO con `[JsonPropertyName]`,
descartado por ser más verboso y propenso a errores que una única
configuración de serializador.

## Decisión 4: `ApiResponse<T>` como tipo de retorno de Refit

**Decisión**: `IPropertiesApi.GetPropertiesAsync` retorna
`Task<ApiResponse<PagedPropertiesResponse>>` en lugar de
`Task<PagedPropertiesResponse>`.

**Justificación**: Permite distinguir de forma explícita éxito, error de
validación (400) y error interno (500) sin depender de excepciones para el
flujo de control, y sin exponer el cuerpo técnico del error en la UI (FR-009),
cumpliendo el requisito de mostrar un estado de error claro y genérico.

**Alternativas consideradas**: `Task<PagedPropertiesResponse>` con manejo de
`ApiException`, descartado por mezclar control de flujo de errores esperados
(400/500) con manejo de excepciones, menos explícito para los tres caminos
(éxito/vacío/error) que exige la spec.

## Decisión 5: Grilla responsive reutilizando los puntos de quiebre existentes

**Decisión**: Nueva clase `.properties-grid` en `app.css`, con `grid-template-columns`
de 3 columnas por defecto (escritorio), 2 columnas bajo `max-width: 1024px` y
1 columna bajo `max-width: 640px`, reutilizando los mismos puntos de quiebre
definidos en `012-foundation-blazor-frontend`.

**Justificación**: Mantiene consistencia responsive con el resto de la
aplicación sin introducir un nuevo sistema de breakpoints.

**Alternativas consideradas**: usar una librería de grid externa, descartada
por prohibición explícita de frameworks CSS de terceros.
