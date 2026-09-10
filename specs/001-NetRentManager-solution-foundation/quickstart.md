# Guía de Validación Rápida: Fundación de la Solución NetRentManager

## Prerrequisitos

- Ejecutar los comandos desde la raíz del repositorio.
- Tener instalado el SDK indicado por `global.json` (`10.0.400`).
- No modificar `global.json` durante la validación.

## Validación estructural

1. Confirmar que existe `app/NetRentManager.sln`.
2. Confirmar que existen los cuatro proyectos en las rutas definidas en [spec.md](./spec.md).
3. Confirmar que los cuatro proyectos están referenciados por la solución principal.
4. Confirmar que no se crearon entidades de dominio, migraciones ni features de producto.

## Compilación

Desde la raíz, compilar la solución:

```powershell
dotnet build app/NetRentManager.sln
```

**Resultado esperado**: la solución compila sin errores.

## Pruebas

Ejecutar las pruebas de la solución:

```powershell
dotnet test app/NetRentManager.sln
```

**Resultado esperado**: los proyectos de pruebas base se ejecutan correctamente sin requerir PostgreSQL ni servicios externos.

## Criterios de cierre

- Las rutas y referencias cumplen RF-001..RF-005.
- El arranque cumple RF-006.
- No se incumplen RF-007..RF-010.
- Se cumplen CE-001 y CE-002 mediante las verificaciones anteriores.
