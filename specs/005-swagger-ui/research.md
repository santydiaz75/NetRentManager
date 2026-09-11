# Investigación: Swagger UI para el Backend

## Decisión 1: Biblioteca de la interfaz

- **Decisión**: Usar `Swashbuckle.AspNetCore`, ya referenciado por `NetRentManagerApi.csproj` en la versión `10.2.3`.
- **Razonamiento**: Es compatible con ASP.NET Core y proporciona el middleware y los recursos de Swagger UI sin introducir una solución paralela.
- **Alternativas consideradas**: NSwag runtime, descartada porque NSwag se usa como herramienta offline en `009-open-api`; recursos estáticos manuales, descartados porque trasladan mantenimiento de la UI al repositorio.

## Decisión 2: Fuente del documento

- **Decisión**: Configurar Swagger UI para cargar `/openapi/v1.json`, el documento estático generado y versionado por `009-open-api`.
- **Razonamiento**: Mantiene una única fuente de verdad y evita que Swashbuckle genere un segundo contrato dinámico en `/swagger/v1/swagger.json`.
- **Alternativas consideradas**: `UseSwagger()` con su JSON generado, descartada por duplicación contractual; documento embebido en HTML, descartado por deriva y duplicación.

## Decisión 3: Activación por entorno

- **Decisión**: Registrar y mapear Swagger UI únicamente cuando `app.Environment.IsDevelopment()` sea verdadero.
- **Razonamiento**: Cumple la aclaración de la spec y evita publicar accidentalmente documentación interactiva.
- **Alternativas consideradas**: configuración habilitable en staging, descartada; feature flag general, descartado por ampliar innecesariamente la superficie de configuración.

## Decisión 4: Comportamiento fuera de Development

- **Decisión**: No registrar middleware ni endpoints de Swagger UI fuera de `Development`, de modo que `/swagger`, `/swagger/index.html` y sus recursos respondan `404`.
- **Razonamiento**: La ruta no existe en esos entornos y no revela que la UI esté instalada.
- **Alternativas consideradas**: `403 Forbidden`, descartada porque confirma la existencia del recurso; redirección informativa, descartada porque agrega una superficie innecesaria.

## Decisión 5: Validación

- **Decisión**: Añadir pruebas de composición HTTP para desarrollo y no desarrollo, más una comprobación del endpoint OpenAPI referenciado.
- **Razonamiento**: La funcionalidad depende de middleware, entorno y recursos HTTP; una prueba puramente unitaria no detectaría rutas mal registradas.
- **Alternativas consideradas**: solo validación manual, descartada porque no protege regresiones; snapshot del HTML completo, descartado por ser frágil frente a cambios de versión de la UI.
