# Investigación: OpenAPI v1 Público

## Decisión 1: Generación durante build

- **Decisión**: Configurar la generación de `openapi/v1.json` durante build mediante `Microsoft.AspNetCore.OpenApi` y MSBuild.
- **Razonamiento**: El artefacto se puede revisar, lintar, versionar y servir estáticamente; no añade trabajo al startup ni a cada request.
- **Alternativas consideradas**: generación al iniciar, descartada por efectos de startup; generación bajo demanda, descartada por latencia y por no ser un archivo versionado estable.

## Decisión 2: Publicación estática sin UI

- **Decisión**: Servir `wwwroot/openapi/v1.json` con `UseStaticFiles` y no registrar Swagger UI, ReDoc UI, Scalar UI ni `MapOpenApi()` runtime si añade una ruta dinámica duplicada.
- **Razonamiento**: Cumple la superficie pública mínima: un JSON estable y ninguna interfaz interactiva.
- **Alternativas consideradas**: `MapOpenApi()` en Development, descartado porque condiciona disponibilidad y puede producir un segundo documento; Swagger UI, excluido por la spec.

## Decisión 3: Validación Redocly

- **Decisión**: Mantener `@redocly/cli` en `package.json`, agregar `.redocly.yaml` con `security-defined: off` y `operation-4xx-response: warn`, y ejecutar lint con npx.
- **Razonamiento**: La API no tiene autenticación y Redocly es estricto por defecto; solo se relajan las reglas aprobadas.
- **Alternativas consideradas**: ignorar lint, rechazado porque elimina la garantía contractual; agregar seguridad ficticia, rechazado porque contradice la API pública.

## Decisión 4: NSwag local

- **Decisión**: Registrar `NSwag.ConsoleCore` en `dotnet-tools.json` y generar un cliente C# smoke en `artifacts/openapi-client-smoke/`.
- **Razonamiento**: Verifica que un consumidor real puede interpretar el documento sin convertir NSwag en dependencia runtime.
- **Alternativas consideradas**: paquete NSwag runtime, rechazado por alcance; generación manual de cliente, rechazada porque no valida consumibilidad.

## Decisión 5: Script reproducible

- **Decisión**: Corregir `support/scripts/generate-openapi-v1.ps1` para apuntar a `NetRentManagerApi.csproj`, `wwwroot/openapi/v1.json` y cliente smoke del repositorio, comprobando cada código de salida.
- **Razonamiento**: El script existente apunta a `RealtorApi` y no puede validar este repositorio.
- **Alternativas consideradas**: mantenerlo sin ejecutar, rechazado por requisito obligatorio; script separado, rechazado para evitar dos fuentes de regeneración.

## Decisión 6: Drift

- **Decisión**: Comparar `EndpointDataSource` con paths/métodos del JSON, normalizando constraints de ruta y excluyendo solo el documento estático si aparece en el inventario.
- **Razonamiento**: La implementación real controla el inventario; el drift detecta contratos obsoletos sin editar el documento manualmente.
- **Alternativas consideradas**: comparar texto de código fuente, rechazado porque no representa endpoints finales; comparar solo paths, rechazado porque omite métodos.
