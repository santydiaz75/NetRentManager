# Modelo de Datos: Fundación de la Solución NetRentManager

Esta iniciativa no define datos de negocio persistentes.

## Entidades

No se crean entidades de dominio, entidades persistentes, `DbSet`, configuraciones EF Core ni migraciones.

## Relaciones y ciclo de vida

No aplica. La solución y los proyectos son artefactos estructurales, no registros de negocio.

## Validaciones

- La solución debe contener exactamente los proyectos definidos en la spec.
- Cada proyecto debe permanecer en su ruta canónica.
- No deben aparecer archivos de modelo, migración o seed en el alcance foundation.

## Evolución prevista

Las entidades y relaciones se definirán en specs posteriores, junto con sus configuraciones, migraciones y seeds trazados a tareas específicas.
