# Guía de Validación: Actualización de Propiedades

## Prerrequisitos

- SDK definido por `global.json` (`10.0.400`).
- PostgreSQL disponible mediante la configuración existente para pruebas manuales.
- Backend `app/backend/src/NetRentManagerApi` compilado.
- Una propiedad existente con `imageUrl` y otra propiedad existente sin imagen.
- Un PNG y un JPG/JPEG reales disponibles para pruebas.

## Preparación

```powershell
dotnet restore .\app\NetRentManager.sln
dotnet build .\app\NetRentManager.sln --no-restore
```

La iniciativa no genera migración: reutiliza la nulabilidad de `ImageUrl` creada
por `006-properties-create`.

## Pruebas automatizadas

```powershell
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
```

La suite debe cubrir:

1. `PUT` válido sin imagen, reemplazo completo y conservación exacta de `imageUrl`.
2. `PUT` válido con PNG y JPG/JPEG, UUID, URL pública y colisiones.
3. Id inexistente con HTTP 404 y cero I/O.
4. Campos faltantes o inválidos con HTTP 400.
5. Archivo vacío o contenido/formato inválido con HTTP 415.
6. Límite exacto de 5 MiB aceptado y un byte adicional con HTTP 413.
7. Fallo de persistencia/I/O/cancelación con HTTP 500, restauración y limpieza.
8. Limpieza posterior al commit con logging y conservación de HTTP 200.
9. Mapping explícito, auto-descubrimiento y compatibilidad con `imageUrl: null`.
10. Última escritura válida sin columna de concurrencia ni HTTP 409.

## Ejemplos manuales

Actualizar sin imagen:

```powershell
curl.exe -X PUT http://localhost:5023/api/properties/{id} `
  -F "title=Apartamento actualizado" `
  -F "description=Descripción actualizada" `
  -F "address=Calle Principal 10" `
  -F "price=1250" `
  -F "status=Available" `
  -F "bedroomCount=2" `
  -F "bathroomCount=1" `
  -F "areaSquareMeters=70"
```

Esperado: HTTP 200, todos los campos reemplazados y `imageUrl` igual al valor
previo.

Actualizar con imagen:

```powershell
curl.exe -X PUT http://localhost:5023/api/properties/{id} `
  -F "title=Apartamento con imagen nueva" `
  -F "description=Descripción actualizada" `
  -F "address=Calle Principal 11" `
  -F "price=1300" `
  -F "status=Available" `
  -F "bedroomCount=2" `
  -F "bathroomCount=2" `
  -F "areaSquareMeters=80" `
  -F "image=@.\support\seed-data\images\properties\1.png"
```

Esperado: HTTP 200, `imageUrl` bajo `/assets/properties/`, nombre UUID y archivo
público disponible. La imagen anterior se elimina después del commit si la ruta
pertenece al directorio administrado.

## Medición p95

Ejecutar al menos 100 solicitudes válidas, mezclando actualizaciones sin imagen y
con imagen válida, en el entorno local documentado. Registrar latencia de cada
respuesta desde el cliente, ordenar las muestras y tomar el valor de la posición
`ceil(0.95 * N)`. El criterio pasa cuando el valor es menor de 500 ms y todas las
respuestas son HTTP 200.

La medición debe conservar: fecha, SDK, configuración, número de solicitudes,
mezcla de casos, p95, errores y tamaño de los archivos.

## Evidencia de validación

Durante la implementación registrar:

- fecha y comandos ejecutados;
- build y cantidad de pruebas correctas;
- respuestas HTTP 200, 400, 404, 413, 415 y 500;
- conservación de `imageUrl` sin archivo;
- UUID y disponibilidad pública de la nueva imagen;
- ausencia de archivos nuevos tras fallos previos al commit;
- restauración de datos y URL anterior ante fallo de persistencia;
- logging de fallo de limpieza posterior sin revertir el commit;
- resultado p95 de la muestra funcional;
- confirmación de que no se agregó migración ni control de concurrencia.

## Evidencia de validación

Fecha: 2026-09-11

Comandos ejecutados:

```powershell
dotnet build .\app\NetRentManager.sln --no-restore
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
dotnet run --project .\app\backend\src\NetRentManagerApi\NetRentManagerApi.csproj --no-build --urls http://localhost:5077
```

Resultados:

- Build de la solución: correcto; backend y frontend compilados con 6 advertencias existentes de compatibilidad de paquetes con `net10.0`.
- Suite backend: 92 pruebas correctas, 0 errores y 0 omitidas.
- Suite focalizada de actualización: 20 pruebas correctas, 0 errores y 0 omitidas.
- Auto-descubrimiento: la API mapeó 4 slices, incluyendo `UpdatePropertySlice`.
- `PUT` sin imagen: HTTP 200, todos los campos reemplazados y `imageUrl: null` conservado.
- `PUT` con PNG real: HTTP 200, URL `/assets/properties/209965a6e242443e887578fd3f95630d.png` y asset público HTTP 200.
- Id inexistente: HTTP 404 con `properties.not_found`, sin escritura de archivo.
- Campos de negocio inválidos: HTTP 400 mediante `ValidationProblemDetails`, cubierto por las pruebas del validator.
- Contenido real inválido: HTTP 415 con `properties.image.invalid_content`.
- Imagen mayor de 5 MiB: HTTP 413 con `properties.image.too_large`.
- Fallo de almacenamiento simulado: HTTP 500 con `properties.update.failed`, rollback de la entidad y limpieza compensatoria cubiertos por las pruebas del handler.
- Muestra p95: 100 actualizaciones válidas sin imagen, 100 respuestas HTTP 200, p95 de 330.95 ms y máximo de 346.85 ms.
- La API se detuvo después de la validación manual; no queda proceso local ejecutándose.
- No se agregó migración, `rowVersion`, bloqueo de concurrencia ni respuesta HTTP 409.
