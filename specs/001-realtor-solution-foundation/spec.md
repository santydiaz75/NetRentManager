# Especificación de la Funcionalidad: Fundación de la Solución NetRentManager

**Rama de la funcionalidad**: `001-NetRentManager-solution-foundation`

**Creado**: 2026-07-07

**Estado**: Borrador

**Entrada**: Descripción de usuario: "Crear base de la solución NetRentManager para la iniciativa 001, sin lógica de negocio ni features"

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Crear la base única de solución (Prioridad: P1)

Como equipo de desarrollo, necesitamos una solución única con backend y frontend iniciales para arrancar el producto NetRentManager con una estructura coherente desde el día cero.

**Por qué esta prioridad**: Sin una base de solución común no existe un punto de partida estable para planificar e implementar iniciativas posteriores.

**Prueba independiente**: Se valida creando la solución y los proyectos base en las rutas definidas; el resultado entrega una base navegable y compilable sin funcionalidades de negocio.

**Escenarios de aceptación**:

1. **Dado** un repositorio con `global.json` en la raíz, **cuando** se prepara la iniciativa foundation, **entonces** existe una solución principal en `app/NetRentManager.sln`.
2. **Dado** la solución principal creada, **cuando** se revisa su composición, **entonces** incluye un proyecto de backend, un proyecto de frontend y sus respectivos proyectos de pruebas en las rutas esperadas.

---

### Historia de Usuario 2 - Asegurar arranque técnico mínimo sin negocio (Prioridad: P2)

Como responsable técnico, necesito que la configuración inicial de arranque esté limitada a capacidades base para evitar deuda temprana y mantener el enfoque en fundación.

**Por qué esta prioridad**: Un arranque mínimo reduce riesgo de sobre-implementación y mantiene trazabilidad de alcance en una iniciativa fundacional.

**Prueba independiente**: Se valida inspeccionando el archivo de arranque del backend para confirmar que solo contiene configuración base de servicios, middleware y mapeo inicial de endpoints.

**Escenarios de aceptación**:

1. **Dado** el proyecto backend base, **cuando** se inspecciona su arranque, **entonces** solo contiene configuración mínima de plataforma.
2. **Dado** la iniciativa foundation, **cuando** se revisa el código inicial, **entonces** no existen implementaciones de lógica de negocio, features de producto ni entidades de dominio.

---

### Historia de Usuario 3 - Dejar base lista para evolución por specs (Prioridad: P3)

Como equipo de producto y arquitectura, necesitamos que la base fundacional quede preparada para que futuras iniciativas extiendan la solución sin rehacer estructura.

**Por qué esta prioridad**: Una base consistente acelera las siguientes iniciativas y disminuye cambios estructurales costosos.

**Prueba independiente**: Se valida verificando que las rutas, separación de proyectos y restricciones de alcance foundation queden explícitas y auditables.

**Escenarios de aceptación**:

1. **Dado** la estructura foundation completada, **cuando** una nueva iniciativa comience, **entonces** puede partir desde la solución existente sin reestructuración inicial.

### Casos límite

- ¿Qué ocurre si falta `global.json` en la raíz? El proceso de esta iniciativa se bloquea y no se generan artefactos nuevos hasta corregirlo.
- ¿Qué ocurre si ya existe parcial o totalmente la estructura de carpetas/proyectos esperada? Debe evitarse sobreescritura destructiva y conservarse la consistencia de nombres/rutas.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El sistema DEBE disponer de una solución principal en `app/NetRentManager.sln` como punto único de composición.
- **RF-002**: El sistema DEBE disponer de un proyecto de backend en `app/backend/src/NetRentManagerApi/`.
- **RF-003**: El sistema DEBE disponer de un proyecto de pruebas de backend en `app/backend/tests/NetRentManagerApiTests/`.
- **RF-004**: El sistema DEBE disponer de un proyecto de frontend en `app/frontend/src/NetRentManagerWeb/`.
- **RF-005**: El sistema DEBE disponer de un proyecto de pruebas de frontend en `app/frontend/test/NetRentManagerWeb/`.
- **RF-006**: La configuración inicial del arranque del backend DEBE limitarse a servicios, middleware y mapeo inicial de endpoints.
- **RF-007**: La iniciativa foundation NO DEBE introducir lógica de negocio.
- **RF-008**: La iniciativa foundation NO DEBE introducir features funcionales de producto.
- **RF-009**: La iniciativa foundation NO DEBE crear entidades de dominio.
- **RF-010**: La versión de plataforma utilizada en esta iniciativa DEBE derivarse del archivo `global.json` existente en la raíz del repositorio, sin modificar dicho archivo.

### Entidades clave

- **Solución NetRentManager**: Contenedor principal que referencia proyectos de backend, frontend y pruebas.
- **Proyecto Base**: Unidad de aplicación inicial (backend o frontend) sin funcionalidad de negocio.
- **Proyecto de Pruebas Base**: Unidad de pruebas preparada para validar evolución futura de cada capa.
- **Baseline de Versión**: Restricción de versión de plataforma declarada en `global.json` y aplicada a iniciativas .NET futuras.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **CE-001**: El 100% de las rutas de proyectos definidas en esta iniciativa existen y están referenciadas por la solución principal.
- **CE-002**: La solución base compila sin errores de construcción en al menos 1 ejecución completa de verificación.
- **CE-003**: El 100% del código inicial de backend permanece dentro de configuración base de arranque, sin lógica de negocio ni entidades de dominio.
- **CE-004**: El equipo puede iniciar la siguiente iniciativa sin crear una segunda solución ni reestructurar carpetas fundacionales.

## Suposiciones

- Existe y se mantiene `global.json` como fuente de verdad para la versión de .NET del repositorio.
- Esta iniciativa se limita a fundación estructural; los comportamientos de negocio se implementarán en iniciativas posteriores.
- Las rutas objetivo indicadas en la iniciativa son canónicas para esta solución y deben mantenerse estables.
- Las pruebas iniciales en proyectos de test pueden comenzar como esqueleto mínimo y crecer por iniciativas futuras.
