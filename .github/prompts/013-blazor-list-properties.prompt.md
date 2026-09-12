---
name: 013-blazor-list-properties
agent: speckit.specify
---

# Spec Prompt - Listado de Propiedades en Página Principal

## Instrucción para Speckit

Quiero crear una nueva spec llamada 013-blazor-list-properties

## Contexto del proyecto

- El frontend es Blazor Web App y el proyecto objetivo es NetRentManagerWeb.csproj.
- La ruta principal actual es Home.razor y debe convertirse en la página de listado de propiedades con paginación.
- El consumo de backend debe usar Refit con clientes tipados, sin uso directo de HttpClient en componentes Razor.
- Debe usarse el endpoint backend definido en ListPropertiesSlice.cs.
- El contrato OpenAPI detallado está en v1.json.
- Deben respetarse las reglas de frontend en frontend.instructions.md.
- Debe aplicarse el skill visual definido en SKILL.md.

## Objetivo funcional

Como usuario, quiero abrir la página principal y ver una lista paginada de propiedades para navegar los resultados por páginas de forma clara y rápida.

## Requisitos funcionales

- La página principal en ruta / debe consumir GET /api/properties.
- Debe enviar parámetros de query obligatorios Page y PageSize.
- Debe renderizar los datos de cada propiedad.
- Debe mostrar metadatos de paginación: página actual, total de páginas, total de elementos.
- Debe incluir controles de paginación: anterior y siguiente.
- Los controles deben deshabilitarse cuando no aplica según HasPrevious y HasNext.
- Debe soportar loading, empty, error y estado con datos.

## Contrato API a respetar

- Endpoint: GET /api/properties
- Query obligatoria: Page, PageSize
- Response 200: PagedPropertyListResponse con Items, Page, PageSize, TotalItems, TotalPages, HasNext, HasPrevious
- Response 400: ValidationProblemDetails

## Requisitos técnicos frontend

- Usar Refit tipado y DI para el cliente de propiedades.
- No usar HttpClient directo en componentes.
- No usar RestService.For fuera del registro con IHttpClientFactory.
- Mantener estilo desktop first.
- Usar sistema visual central en app.css.
- Si se requieren íconos, usar Lucide Icons.
- No introducir frameworks CSS externos.
- No usar estilos inline ni colores hardcodeados.

## Estructura recomendada para implementar esta spec

- Mantener página enroutable en Components/Pages.
- Crear organización por feature para la funcionalidad de listado paginado.
- Propuesta:
- Components/Pages para la ruta principal.
- Features/Properties/List para componentes, modelos de presentación y lógica de UI de la página.

## Criterios de aceptación verificables

1. Al entrar a /, se consulta backend con Page y PageSize y se muestran propiedades.
2. La paginación permite navegar y actualiza correctamente los resultados.
3. Si el backend devuelve error o hay fallo de red, se muestra estado de error claro.
4. Si no hay resultados, se muestra estado empty claro.
5. La UI respeta app.css y la grilla de propiedades usa 3 columnas en desktop, 2 en tablet y 1 en móvil.
6. No hay uso de estilos inline, Bootstrap CSS, Tailwind ni librerías de íconos distintas de Lucide.
7. El consumo HTTP ocurre mediante Refit tipado registrado por DI.

## Fuera de alcance

- CRUD completo de propiedades.
- Filtros avanzados, búsqueda avanzada o sorting complejo.
- Cambios backend, persistencia o dominio.
- Funcionalidades no trazadas en tasks.md de la nueva spec.

## Entregables esperados de Speckit

- spec.md con historias de usuario y criterios de aceptación.
- plan.md con diseño técnico, estructura de carpetas y decisiones de implementación.
- tasks.md con tareas trazables, ordenadas por fases e historias, incluyendo validación final.

Si quieres, también te lo puedo dar en una variante más corta para ejecutar rápido en speckit.specify sin tanto contexto.