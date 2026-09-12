# Modelo de Datos de Presentación: Listado de Propiedades en Página Principal

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

Esta iniciativa no introduce entidades de dominio ni persistencia. Los
"modelos" relevantes son los DTO de presentación del cliente Refit, que
reflejan el contrato JSON real de `GET /api/properties` (ver
`specs/004-properties-list-pagination/contracts/http-contracts.md` y
`app/backend/src/NetRentManagerApi/wwwroot/openapi/v1.json`).

## `PagedPropertiesResponse` (presentación)

| Campo | Tipo | JSON |
|---|---|---|
| `Items` | `IReadOnlyList<PropertyListItem>` | `items` |
| `Page` | `int` | `page` |
| `PageSize` | `int` | `pageSize` |
| `TotalItems` | `int` | `totalItems` |
| `TotalPages` | `int` | `totalPages` |
| `HasNext` | `bool` | `hasNext` |
| `HasPrevious` | `bool` | `hasPrevious` |

## `PropertyListItem` (presentación)

| Campo | Tipo | JSON |
|---|---|---|
| `Id` | `Guid` | `id` |
| `Title` | `string` | `title` |
| `Description` | `string` | `description` |
| `Address` | `string` | `address` |
| `Price` | `decimal` | `price` |
| `Status` | `string` | `status` |
| `BedroomCount` | `int` | `bedroomCount` |
| `BathroomCount` | `int` | `bathroomCount` |
| `AreaSquareMeters` | `decimal` | `areaSquareMeters` |
| `ImageUrl` | `string?` | `imageUrl` (nullable) |

## Parámetros de consulta

| Parámetro | Tipo | Origen | Valor por defecto |
|---|---|---|---|
| `page` | `int` | Query string de la página (`[SupplyParameterFromQuery]`) | `1` |
| `pageSize` | `int` | Query string de la página (`[SupplyParameterFromQuery]`) | `6` |

## Estados de la página (presentación, no persistidos)

| Estado | Condición | Clase CSS |
|---|---|---|
| Cargando | Solicitud en curso, sin resultado previo | `.state-loading` |
| Vacío | Respuesta exitosa con `items` vacío | `.state-empty` |
| Error | Fallo de red o `ApiResponse` sin éxito | `.state-error` |
| Con datos | Respuesta exitosa con `items` no vacío | grilla `.properties-grid` |

## Relación con el contrato del backend

```text
GET /api/properties?page={page}&pageSize={pageSize}
        │
        ▼
IPropertiesApi.GetPropertiesAsync(page, pageSize)  [Refit, camelCase JSON]
        │
        ▼
ApiResponse<PagedPropertiesResponse>  →  Home.razor decide el estado a renderizar
```
