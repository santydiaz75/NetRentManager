---
name: 014-blazor-property-detail
agent: speckit.specify
---

Crear una nueva spec llamada 014-blazor-property-detail para agregar una página de detalle de una propiedad en el frontend Blazor del proyecto NetRentManagerWeb (app/frontend/src/NetRentManagerWeb).

Objetivo de la feature:
Mostrar el detalle completo de una sola propiedad. La página recibe como parámetro de ruta el id de la propiedad y consume el endpoint GET /api/properties/{id} (operación GetPropertyById, tag GetPropertyByIdSlice) del backend NetRentManagerApi. El contrato está descrito en el archivo OpenAPI app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json.

Contrato del endpoint (según v1.json):
- Método y ruta: GET /api/properties/{id}
- operationId: GetPropertyById
- Parámetro de ruta: id (string)
- Respuesta 200 (application/json): schema PropertyDetailResponse con los campos:
  - id (uuid)
  - title (string)
  - description (string)
  - address (string)
  - price (number)
  - status (string)
  - bedroomCount (int)
  - bathroomCount (int)
  - areaSquareMeters (number)
  - imageUrl (string, puede ser null)
- Respuesta 400 (application/problem+json): HttpValidationProblemDetails cuando el id es inválido.
- Respuesta 404 (application/problem+json): ProblemDetails cuando la propiedad no existe.

Requisitos funcionales:
1. Nueva página Blazor accesible por una ruta con el id como parámetro (por ejemplo /properties/{id}).
2. Al cargar, la página consulta el detalle de la propiedad por su id usando Refit.
3. La página muestra todos los datos públicos de la propiedad: título, imagen, descripción, dirección, precio, estado, dormitorios, baños y metros cuadrados.
4. Debe manejar explícitamente tres estados: cargando, contenido cargado y error.
5. El estado de error debe distinguir claramente el caso de propiedad no encontrada (404) del resto de errores del backend o de red.
6. Cuando imageUrl es null, mostrar un marcador o imagen por defecto consistente con el diseño.
7. Cada tarjeta del listado de propiedades de la página principal (feature 011, componente Features/Properties/List/PropertyCard.razor) DEBE mostrar un botón con el texto "Ver". Al hacer clic en ese botón, el usuario debe ser redirigido a la página de detalle de esa propiedad usando su id. Debe existir una forma de volver al listado desde la página de detalle.

Requisitos técnicos y de arquitectura (obligatorios):
- Frontend con Blazor Web App y Razor Components, siguiendo .github/instructions/frontend.instructions.md.
- La comunicación con el backend DEBE hacerse con Refit mediante una interfaz tipada por feature (IPropertiesApi ya existe en Services/Api/Properties), registrada con IHttpClientFactory. Prohibido usar HttpClient directamente o RestService.For<T>() fuera del registro aprobado.
- Reutilizar el contrato PropertyDetailResponse como modelo tipado del frontend, sin duplicar contratos manualmente fuera de spec.
- Organizar el código por vertical slice de frontend dentro de Features/Properties (por ejemplo Features/Properties/Detail), coherente con Features/Properties/List existente.
- El botón "Ver" de la tarjeta DEBE usar el sistema de navegación de Blazor (NavigationManager o enlace de navegación), no lógica de negocio en el componente.
- Todo el estilo DEBE usar el sistema visual canónico de wwwroot/app.css siguiendo el skill .github/skills/blazor-app-css-design-system/SKILL.md. Prohibido estilos inline, colores hardcodeados o frameworks CSS. El botón "Ver" DEBE reutilizar las clases de botón existentes (por ejemplo btn, btn-primary). Solo crear clases nuevas en app.css cuando sea necesario y trazado en tasks.md.
- Los íconos DEBEN ser Lucide Icons según las instrucciones de frontend.
- Manejar estados de carga, no encontrado y error con las clases de estado definidas (loading-state, error-state, etc.).
- Diseño desktop first, accesible, responsive.

Alcance y restricciones:
- Solo frontend. No modificar el backend ni el contrato del endpoint.
- No implementar edición, eliminación ni cambio de estado desde esta página (solo lectura del detalle).
- El único cambio permitido sobre el listado existente (feature 011) es agregar el botón "Ver" y su navegación; no rediseñar la tarjeta ni cambiar otros datos que ya muestra.
- Respetar el flujo Spec-Driven Development: la spec debe describir historias de usuario priorizadas, escenarios de aceptación y casos límite (id inválido, propiedad inexistente, imagen nula, error de red, clic en "Ver" desde cualquier tarjeta del listado).

Sigue las convenciones del repositorio: documento en español, numeración de spec 012, y consistencia con las specs 010 (foundation frontend) y 011 (listado de propiedades).