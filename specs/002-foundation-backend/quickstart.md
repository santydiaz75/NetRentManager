# Guía de Validación: Fundación Interna del Backend

## Prerrequisitos

- .NET SDK definido por `global.json` (`10.0.400`).
- PowerShell desde la raíz del repositorio.
- Solución existente en `app/NetRentManager.sln`.

## Preparación

Restaurar y compilar la solución:

```powershell
dotnet restore .\app\NetRentManager.sln
dotnet build .\app\NetRentManager.sln --no-restore
```

## Ejecución de pruebas

Ejecutar las pruebas unitarias del backend:

```powershell
dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore
```

El resultado esperado es que todas las pruebas de descubrimiento, handlers,
validación, errores y salud pasen sin levantar una base de datos.

## Escenarios de validación

1. Registrar dos veces el assembly de pruebas y comprobar que `IEnumerable<ISlice>`
   contiene una sola instancia del slice de prueba.
2. Registrar dos veces el assembly de pruebas y comprobar que
   `IEnumerable<IHandler>` no contiene duplicados.
3. Ejecutar la `ValidationFilterFactory` con un validator válido, con un validator
   inválido y con un tipo sin validator.
4. Ejecutar el resultado inválido sobre `DefaultHttpContext` y verificar HTTP 400,
   content type de Problem Details y todos los errores agrupados.
5. Convertir un `Result` de error con y sin detalles opcionales y verificar el status
   code y payload de `ProblemDetails`.
6. Mapear `HealthSlice`, comprobar la ruta `GET /health` y ejecutar su resultado para
   verificar una respuesta exitosa sin acceso a persistencia.
7. Inspeccionar `Program.cs` y confirmar que solo compone infraestructura, sin
   registros individuales ni lógica de negocio.

## Evidencia de validación

- **Fecha**: 2026-09-10
- **Comando**: `dotnet restore .\app\NetRentManager.sln`
   - **Resultado**: restauración completada sin errores.
- **Comando**: `dotnet build .\app\NetRentManager.sln --no-restore`
   - **Resultado final**: compilación correcta.
   - Proyectos compilados: `NetRentManagerApi`, `NetRentManagerApiTests`, `NetRentManagerWeb`, `NetRentManagerWebTests`.
- **Comando**: `dotnet test .\app\backend\tests\NetRentManagerApiTests\NetRentManagerApiTests.csproj --no-restore`
   - **Resultado final**: 14 pruebas totales, 14 correctas, 0 con error, 0 omitidas.
- **Confirmación de restricciones negativas**:
   - No se introdujeron entidades de dominio.
   - No se creó `AppDbContext` ni configuraciones/migraciones/seeders de persistencia.
   - No se introdujeron controllers ni `MapControllers`.
   - No se agregaron handlers de negocio ni endpoints de producto.
   - El único endpoint funcional agregado es la sonda de infraestructura `GET /health` como `ISlice`.
