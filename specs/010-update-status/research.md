# Investigación: Actualización de Estado de Propiedad

## Decisión 1: Request JSON estricto

- **Decisión**: El request admite únicamente la propiedad `status`; campos adicionales como `title`, `price`, `imageUrl` o `image` producen HTTP 400.
- **Razonamiento**: El endpoint tiene un alcance intencionalmente pequeño y rechazar contratos más amplios evita aceptar silenciosamente una solicitud distinta del caso de uso.
- **Alternativas consideradas**: ignorar campos adicionales, descartado porque oculta errores de integración; aceptar campos si coinciden con persistencia, descartado por complejidad y por ampliar el contrato.

## Decisión 2: Normalización de estados

- **Decisión**: Comparar `status` sin distinguir mayúsculas/minúsculas y persistir exactamente `Available`, `Rented` o `Maintenance`.
- **Razonamiento**: La API es tolerante para clientes, mientras la persistencia mantiene el conjunto textual canónico existente.
- **Alternativas consideradas**: exigir capitalización exacta, descartada por ser innecesariamente frágil; persistir el texto recibido, descartada porque contaminaría el catálogo textual.

## Decisión 3: Persistencia mínima

- **Decisión**: Buscar la entidad por id, modificar solo `Property.Status` y ejecutar `SaveChangesAsync` con cancelación.
- **Razonamiento**: Reutiliza `AppDbContext`, conserva automáticamente todos los campos no asignados y no requiere migración.
- **Alternativas consideradas**: actualizar mediante un DTO con todos los campos, descartada porque facilita sobrescrituras accidentales; SQL manual, descartado porque contradice el patrón EF Core existente.

## Decisión 4: Response explícito

- **Decisión**: Devolver un contrato de propiedad completa con `imageUrl` nullable y estado textual.
- **Razonamiento**: Mantiene consistencia con las respuestas de detalle/listado y permite verificar que solo cambió el estado.
- **Alternativas consideradas**: devolver solo `{ status }`, descartada porque no permite comprobar la conservación de datos; devolver la entidad EF, descartada por la separación de contratos.

## Decisión 5: Pruebas

- **Decisión**: Cubrir validator, handler, mapping, contrato HTTP, errores y cancelación con xUnit y EF Core InMemory cuando sea suficiente.
- **Razonamiento**: El caso de uso no cambia esquema ni requiere infraestructura externa; las pruebas deben verificar el límite de actualización y la forma de errores.
- **Alternativas consideradas**: pruebas de integración con PostgreSQL, descartadas por no aportar valor proporcional a una mutación de un campo existente; validación manual únicamente, descartada por no proteger la regla de conservación.
