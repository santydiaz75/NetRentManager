# Guía de Validación: Alta de Propiedades

## Prerrequisitos

- SDK definido por `global.json` (`10.0.400`).
- PostgreSQL disponible mediante la configuración existente para pruebas manuales.
- Backend `app/backend/src/NetRentManagerApi` compilado.
- Assets públicos servidos desde `wwwroot/assets/properties`.

## Preparación

```powershell
dotnet restore .\app\NetRentManager.sln
dotnet build .\app\NetRentManager.sln --no-restore
```

La migración de nulabilidad debe generarse y revisarse antes de aplicar el cambio
contra una base de datos compartida.

## Pruebas automatizadas

```powershell
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
```

La suite debe cubrir:

1. Alta válida sin imagen con `imageUrl: null`.
2. Alta válida con PNG y JPG/JPEG.
3. `status` obligatorio y textual.
4. Límite exacto de 5 MiB y rechazo de un byte adicional.
5. Magic bytes incompatibles, archivo vacío y formatos no permitidos.
6. Nombres seguros, colisiones y URL bajo `/assets/properties/`.
7. Limpieza compensatoria ante fallo de archivo, persistencia y cancelación.
8. Migración de `image_url` nullable y compatibilidad del listado 004.
9. Auto-descubrimiento de `ISlice`, `IHandler` y validator.

## Ejemplos manuales

Crear sin imagen:

```powershell
curl.exe -X POST http://localhost:5023/api/properties `
  -F "title=Apartamento nuevo" `
  -F "description=Descripción de prueba" `
  -F "address=Calle Principal 10" `
  -F "price=1200" `
  -F "status=Available" `
  -F "bedroomCount=2" `
  -F "bathroomCount=1" `
  -F "areaSquareMeters=70"
```

Esperado: HTTP 201 y `imageUrl: null`.

Crear con imagen:

```powershell
curl.exe -X POST http://localhost:5023/api/properties `
  -F "title=Apartamento con imagen" `
  -F "description=Descripción de prueba" `
  -F "address=Calle Principal 11" `
  -F "price=1300" `
  -F "status=Available" `
  -F "bedroomCount=2" `
  -F "bathroomCount=2" `
  -F "areaSquareMeters=80" `
  -F "image=@.\support\seed-data\images\properties\1.png"
```

Esperado: HTTP 201, `status: "Available"`, `imageUrl` bajo
`/assets/properties/` y archivo servido por la API.

## Evidencia de validación

Durante la implementación registrar la fecha, migración generada, comandos, pruebas
correctas, respuestas HTTP 201/400/500, comprobación de `imageUrl: null`, archivos
creados, limpieza ante errores y regresión del listado 004.

## Evidencia de validación

Fecha: 2026-09-11

Comandos ejecutados:

```powershell
dotnet build .\app\NetRentManager.sln --no-restore
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
dotnet ef migrations add AllowNullPropertyImageUrl --project .\app\backend\src\NetRentManagerApi --startup-project .\app\backend\src\NetRentManagerApi --output-dir Infrastructure/Persistence/Migrations --no-build
```

Resultados:

- Build de la solución: correcto, con las advertencias existentes de compatibilidad de paquetes con `net10.0`.
- Tests backend: 72 correctos, 0 errores y 0 omitidos.
- Migración `20260911094906_AllowNullPropertyImageUrl`: altera únicamente `properties.image_url` a nullable.
- Auto-descubrimiento: se mapearon `HealthSlice`, `ListPropertiesSlice` y `CreatePropertySlice`.
- POST multipart sin imagen: HTTP 201, `Location` presente e `imageUrl: null`.
- POST multipart con PNG: HTTP 201, `status: "Available"` y URL `/assets/properties/{guid}.png`.
- Asset PNG subido: HTTP 200 desde `/assets/properties/`.
- Listado posterior: HTTP 200, propiedad sin imagen incluida con `imageUrl: null` y `status` textual.
- La API fue detenida después de las pruebas manuales; no queda proceso local ejecutándose.
