# Guía de Validación: Actualización de Estado de Propiedad

## Prerrequisitos

- SDK .NET fijado por `global.json`.
- PostgreSQL y configuración de `DefaultConnection` disponibles para las pruebas de runtime que arranquen la API.
- Documento OpenAPI generado por el build.
- Una propiedad existente del seeding o un GUID válido creado previamente.

## Validación automatizada

Desde la raíz del repositorio:

```powershell
dotnet build app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj
dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj
```

### Resultado esperado

- El build genera el documento OpenAPI sin errores.
- Las pruebas verifican actualización válida, normalización de estado, conservación de campos, rechazo de propiedades adicionales, 400, 404, ProblemDetails y cancelación.
- No se crean migraciones ni archivos de imagen para esta iniciativa.

## Validación manual HTTP

Iniciar el backend según la configuración local y ejecutar el ejemplo incluido en:

`app/backend/src/NetRentManagerApi/NetRentManagerApi.http`

La solicitud válida debe tener esta forma:

```http
PATCH http://localhost:5065/api/properties/a1f0e2d4-7c81-4d12-9e61-0b7f0b1a1001/status
Content-Type: application/json

{
  "status": "rented"
}
```

### Resultado esperado

- Respuesta HTTP `200 OK`.
- La respuesta contiene `status: "Rented"`.
- `title`, `description`, `address`, `price`, dimensiones e `imageUrl` conservan sus valores previos.

## Escenarios negativos

Validar también:

- Estado omitido o cuerpo vacío: `400 Bad Request`.
- Estado inválido: `400 Bad Request`.
- Propiedad JSON adicional, por ejemplo `imageUrl`: `400 Bad Request`.
- GUID inexistente: `404 Not Found`.
- GUID con formato inválido: `400 Bad Request`.

La evidencia de cierre debe registrar build, pruebas, contrato OpenAPI y ejecución del ejemplo HTTP.

## Evidencia de validación

La evidencia final debe registrar:

- `dotnet build app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj` correcto.
- `dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` correcto, incluyendo las pruebas del slice `UpdatePropertyStatus`.
- `support/scripts/generate-openapi-v1.ps1` correcto y sin drift en `wwwroot/openapi/v1.json`.
- Ejecución del ejemplo PATCH con `200 OK` y confirmación de conservación de los demás campos.

Validación ejecutada el 2026-09-11: build correcto con las tres advertencias
preexistentes de soporte de paquetes .NET 10, suite completa `134/134` pruebas
correctas, OpenAPI generado con Redocly y NSwag correctos, y el contrato contiene
`PATCH /api/properties/{id}/status` con respuestas 200/400/404/500.

La ejecución manual con el GUID semilla
`a1f0e2d4-7c81-4d12-9e61-0b7f0b1a1001` devolvió `200 OK`, normalizó `rented` a
`Rented` y conservó `imageUrl: "/assets/properties/1.png"`.
