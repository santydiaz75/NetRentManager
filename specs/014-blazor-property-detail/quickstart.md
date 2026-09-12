# Quickstart: Detalle de Propiedad en Blazor

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

Esta guía valida la navegación desde el listado hacia `/properties/{id}`, la carga
del detalle y los estados de carga, no encontrado y error.

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

## Validar el flujo principal

1. Abrir `https://localhost:7208/` y esperar el listado.
2. Confirmar que cada tarjeta contiene el botón `Ver`.
3. Pulsar `Ver` en una tarjeta y confirmar que la URL cambia a
   `https://localhost:7208/properties/{id}`.
4. Confirmar que se muestran título, descripción, dirección, precio, estado,
   dormitorios, baños, superficie e identificador.
5. En una propiedad sin `imageUrl`, confirmar que aparece el marcador visual y
   no una imagen rota.
6. Pulsar `Volver al listado` y confirmar el regreso a `/`.

## Validar estados de carga y error

1. Abrir directamente `/properties/{id}` con un id válido y observar el estado
   `state-loading` mientras la petición está en curso.
2. Abrir `/properties/00000000-0000-0000-0000-000000000000` y confirmar el estado
   explícito `Propiedad no encontrada`.
3. Detener el backend, recargar una ruta de detalle y confirmar el estado de
   error genérico sin stack trace, URL interna ni excepción cruda.
4. Abrir `/properties/id-invalido` y confirmar que la página muestra un estado
   de error de validación sin realizar una llamada al backend.

## Validación automatizada

```powershell
dotnet build app/NetRentManager.sln --no-restore
dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore
```

## Evidencia de validación

**Fecha**: 2026-09-12

- `dotnet build app/frontend/src/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore`: correcto, 0 errores y 0 advertencias después de corregir los imports Razor.
- `dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore`: 39 pruebas correctas, 0 errores.
- Las pruebas cubren carga exitosa, `404`, error de backend, fallo de red, estado de carga, navegación del botón `Ver` y compatibilidad del cliente Refit.
- Backend ejecutándose en `https://localhost:7065` y frontend en `https://localhost:7208` con base de datos y secretos locales disponibles.
- `GET /api/properties?page=1&pageSize=1`: correcto; devolvió la propiedad sembrada `67f779ca-7b79-442b-9d0f-ba765e874b84`.
- `GET /api/properties/67f779ca-7b79-442b-9d0f-ba765e874b84`: `200 OK`; el frontend renderizó `Apartamento manual imagen`, la imagen principal y el enlace `Volver al listado`.
- `GET /api/properties/00000000-0000-0000-0000-000000000000`: `404 Not Found`; el frontend renderizó `Propiedad no encontrada`.
- `/properties/id-invalido`: el frontend renderizó el estado de error sin realizar una llamada Refit.
