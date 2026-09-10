# Plan de Implementación: Fundación de la Solución NetRentManager

**Rama**: `001-NetRentManager-solution-foundation` | **Fecha**: 2026-09-10 | **Spec**: [spec.md](./spec.md)

**Entrada**: Especificación de funcionalidad de `/specs/001-NetRentManager-solution-foundation/spec.md`

## Resumen

Crear la solución única de NetRentManager con backend, frontend y sus proyectos de pruebas en las rutas canónicas. La implementación usará la versión de SDK fijada en `global.json`, una aplicación backend Minimal API sin lógica de negocio y una aplicación frontend Blazor Web App sin funcionalidades de producto.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET SDK 10.0.400, fijado por `global.json`.

**Dependencias principales**: ASP.NET Core Minimal APIs para el backend; Blazor Web App y Razor Components para el frontend. EF Core, Npgsql, Refit, FluentValidation, ProblemDetails, logging estructurado y Lucide Icons quedan habilitados por la constitución para las iniciativas que los necesiten, pero no se incorporan capacidades de negocio en esta fundación.

**Almacenamiento**: No se crea modelo persistente ni migraciones en esta iniciativa; PostgreSQL/Npgsql quedan reservados para futuras specs con requisitos de persistencia.

**Pruebas**: Proyectos de pruebas unitarias para backend y frontend. La solución completa debe compilar y las pruebas base deben ejecutarse sin introducir pruebas de integración no descritas por la spec.

**Plataforma objetivo**: Aplicación web .NET 10 ejecutable en los entornos soportados por el SDK, con desarrollo local como validación inicial.

**Tipo de proyecto**: Solución web fullstack con un proyecto backend, un proyecto frontend y dos proyectos de pruebas.

**Objetivos de rendimiento**: Arranque y compilación correctos de la base, sin objetivos de carga de negocio en esta iniciativa.

**Restricciones**: No modificar `global.json`; no crear lógica de negocio, entidades, features funcionales, migraciones ni endpoints de producto; conservar las rutas canónicas y evitar sobrescritura destructiva.

**Escala/Alcance**: Cuatro proyectos y una solución principal; solo esqueleto técnico para evolución posterior.

## Verificación de la constitución

*Puerta: debe pasar antes de la investigación y volver a verificarse después del diseño.*

- **Solución única**: PASS. Todos los proyectos se agregan a `app/NetRentManager.sln`.
- **Spec-Driven Development**: PASS. El alcance se limita a RF-001..RF-010 y CE-001..CE-004.
- **Arquitectura backend**: PASS. Se crea un backend Minimal API; no se crean controllers ni lógica de dominio.
- **Arquitectura frontend**: PASS. Se crea una Blazor Web App con Razor Components.
- **Stack y versión**: PASS. Se usa `global.json` sin modificarlo y el SDK 10.0.400.
- **Persistencia**: PASS. No se agregan entidades, DbSet, configuraciones ni migraciones.
- **Calidad y pruebas**: PASS. Se crean proyectos de pruebas unitarias base y se valida compilación.
- **Idioma documental**: PASS. Los artefactos de esta iniciativa se redactan en español.

## Investigación

Las decisiones de plataforma y estructura están resueltas por la spec y la constitución. La investigación de plantillas, estructura y validación queda documentada en [research.md](./research.md).

## Estructura del proyecto

### Documentación de esta funcionalidad

```text
specs/001-NetRentManager-solution-foundation/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── checklists/requirements.md
└── tasks.md             # Se creará mediante /speckit-tasks
```

### Código fuente

```text
app/
├── NetRentManager.sln
├── backend/
│   ├── src/NetRentManagerApi/
│   └── tests/NetRentManagerApiTests/
└── frontend/
    ├── src/NetRentManagerWeb/
    └── test/NetRentManagerWeb/
```

**Decisión de estructura**: Se adopta una solución fullstack única con separación física entre backend y frontend, manteniendo proyectos de pruebas junto a cada capa. Los nombres y rutas coinciden exactamente con RF-001..RF-005.

### Contratos

No se genera `contracts/`: la fundación no expone endpoints, comandos ni contratos de integración de negocio. Los contratos se definirán en una spec posterior cuando exista una feature funcional.

## Validación posterior al diseño

- Las cuatro rutas de proyecto se crearán antes de agregarlas a la solución.
- La solución se compilará con el SDK seleccionado por `global.json`.
- Los proyectos de pruebas se ejecutarán sin requerir base de datos, servicios externos ni lógica de negocio.
- Se comprobará que `global.json` no cambió y que no existen entidades ni migraciones en el alcance foundation.
