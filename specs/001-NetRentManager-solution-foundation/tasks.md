---

description: "Tareas ejecutables para la fundación de la solución NetRentManager"
---

# Tareas: Fundación de la Solución NetRentManager

**Entrada**: Documentos de diseño de `/specs/001-NetRentManager-solution-foundation/`

**Prerequisitos**: `plan.md`, `spec.md`, `research.md`, `data-model.md` y `quickstart.md`

**Alcance**: Crear una solución .NET 10 fullstack mínima, sin lógica de negocio, entidades, migraciones ni features de producto.

## Fase 1: Preparación de la solución

**Objetivo**: Confirmar el baseline y crear la estructura física sin sobrescribir trabajo existente.

- [X] T001 Confirmar en `global.json` que el SDK requerido es `10.0.400` y registrar que el archivo no debe modificarse.
- [X] T002 Inspeccionar `app/`, `app/backend/` y `app/frontend/` antes de crear archivos; detenerse ante conflictos con proyectos existentes en las rutas canónicas.
- [X] T003 Crear la solución única `app/NetRentManager.sln` usando el SDK seleccionado por `global.json`.
- [X] T004 [P] Crear el proyecto backend Minimal API en `app/backend/src/NetRentManagerApi/`.
- [X] T005 [P] Crear el proyecto frontend Blazor Web App con Razor Components en `app/frontend/src/NetRentManagerWeb/`.
- [X] T006 [P] Crear el proyecto de pruebas unitarias del backend en `app/backend/tests/NetRentManagerApiTests/`.
- [X] T007 [P] Crear el proyecto de pruebas unitarias del frontend en `app/frontend/test/NetRentManagerWeb/`.

## Fase 2: Fundamentos bloqueantes

**Objetivo**: Integrar los proyectos y dejar una base compilable antes de completar las historias de usuario.

- [X] T008 Agregar los cuatro proyectos a `app/NetRentManager.sln` y verificar que las rutas relativas sean las canónicas de RF-002..RF-005.
- [X] T009 [P] Configurar `app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj` para referenciar únicamente el backend base, sin dependencias de base de datos ni servicios externos.
- [X] T010 [P] Configurar `app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj` para referenciar únicamente el frontend base, sin dependencias de funcionalidades de producto.
- [X] T011 Ejecutar `dotnet build app/NetRentManager.sln` y resolver únicamente errores introducidos por la creación o composición de la fundación.

**Punto de control**: La solución contiene cuatro proyectos y compila sin introducir persistencia ni lógica de negocio.

## Fase 3: Historia de Usuario 1 - Crear la base única de solución (Prioridad: P1) 🎯 MVP

**Objetivo**: Entregar una solución única navegable y compilable con backend, frontend y sus proyectos de pruebas.

**Prueba independiente**: Verificar que `app/NetRentManager.sln` existe, contiene exactamente los cuatro proyectos canónicos y que `dotnet build app/NetRentManager.sln` finaliza sin errores.

- [X] T012 [US1] Verificar en `app/NetRentManager.sln` la referencia al proyecto backend `app/backend/src/NetRentManagerApi/NetRentManagerApi.csproj`.
- [X] T013 [US1] Verificar en `app/NetRentManager.sln` la referencia al proyecto de pruebas backend `app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj`.
- [X] T014 [US1] Verificar en `app/NetRentManager.sln` la referencia al proyecto frontend `app/frontend/src/NetRentManagerWeb/NetRentManagerWeb.csproj`.
- [X] T015 [US1] Verificar en `app/NetRentManager.sln` la referencia al proyecto de pruebas frontend `app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj`.
- [X] T016 [US1] Ejecutar `dotnet test app/NetRentManager.sln` y confirmar que los proyectos de pruebas base finalizan correctamente.

**Punto de control**: US1 es entregable por sí sola y cumple RF-001..RF-005 y CE-001..CE-002.

## Fase 4: Historia de Usuario 2 - Asegurar arranque técnico mínimo sin negocio (Prioridad: P2)

**Objetivo**: Mantener los arranques dentro de configuración de plataforma, middleware y mapeo inicial, sin comportamiento de producto.

**Prueba independiente**: Revisar los archivos de arranque de backend y frontend y comprobar que no contienen lógica de negocio, entidades, features ni endpoints de producto.

- [X] T017 [US2] Revisar `app/backend/src/NetRentManagerApi/Program.cs` y conservar únicamente configuración base de servicios, middleware y mapeo inicial de endpoints.
- [X] T018 [US2] Revisar `app/frontend/src/NetRentManagerWeb/Program.cs` y conservar únicamente la configuración base de la Blazor Web App.
- [X] T019 [US2] Confirmar que `app/backend/src/NetRentManagerApi/` no contiene controllers, entidades de dominio, migraciones, seeds ni slices de negocio.
- [X] T020 [US2] Confirmar que `app/frontend/src/NetRentManagerWeb/` no contiene páginas, componentes o servicios de funcionalidades de producto.
- [X] T021 [US2] Ejecutar `dotnet build app/NetRentManager.sln` después de la revisión de arranque y documentar cualquier error en el alcance de esta iniciativa.

**Punto de control**: US2 cumple RF-006..RF-009 y CE-003 sin ampliar el alcance fundacional.

## Fase 5: Historia de Usuario 3 - Dejar base lista para evolución por specs (Prioridad: P3)

**Objetivo**: Dejar rutas, referencias y límites de alcance estables para que la siguiente iniciativa pueda evolucionar la solución sin reestructuración.

**Prueba independiente**: Repetir la validación estructural y comprobar que no se creó una segunda solución ni se modificó `global.json`.

- [X] T022 [US3] Comparar la estructura real de `app/` con la estructura documentada en `specs/001-NetRentManager-solution-foundation/plan.md` y corregir únicamente divergencias de la fundación.
- [X] T023 [US3] Verificar que no existe una segunda solución `.sln` dentro de `app/` y que `app/NetRentManager.sln` es el único punto de composición.
- [X] T024 [US3] Verificar que `global.json` conserva exactamente el SDK `10.0.400` y que ningún proyecto introduce una versión de plataforma contradictoria.
- [X] T025 [US3] Ejecutar las verificaciones de `specs/001-NetRentManager-solution-foundation/quickstart.md` y registrar los resultados de estructura, compilación y pruebas.

**Punto de control**: US3 cumple RF-010 y CE-004 y deja la solución preparada para la siguiente spec.

## Fase 6: Pulido y validación transversal

**Objetivo**: Cerrar la iniciativa con trazabilidad, formato consistente y comprobaciones completas.

- [X] T026 [P] Revisar que los artefactos Markdown de `specs/001-NetRentManager-solution-foundation/` estén redactados en español y no contengan placeholders de plantilla.
- [X] T027 [P] Revisar que no existan migraciones, configuraciones EF Core, seeds, entidades persistentes ni scripts SQL asociados a esta iniciativa.
- [X] T028 Ejecutar `dotnet build app/NetRentManager.sln` y `dotnet test app/NetRentManager.sln` como validación final.
- [X] T029 Revisar `git diff --check` y el estado de `global.json`; marcar esta tarea y las anteriores como completadas solo después de verificar sus resultados.

## Dependencias y orden de ejecución

### Dependencias entre fases

- **Fase 1**: No depende de otra fase; establece la estructura inicial.
- **Fase 2**: Depende de T001..T007 y bloquea todas las historias.
- **US1**: Depende de la Fase 2; constituye el MVP.
- **US2**: Depende de la Fase 2 y puede ejecutarse en paralelo con US1 si la composición ya está disponible.
- **US3**: Depende de US1 y US2 porque valida la estructura y los límites finales.
- **Pulido**: Depende de todas las fases anteriores.

### Oportunidades de paralelismo

- T004, T005, T006 y T007 pueden ejecutarse en paralelo después de T001 y T002.
- T009 y T010 pueden ejecutarse en paralelo después de T008.
- T012..T015 son verificaciones independientes sobre referencias distintas de la solución.
- T017..T020 pueden revisarse en paralelo porque afectan proyectos distintos o reglas distintas.
- T026 y T027 pueden ejecutarse en paralelo antes de la validación final.

### Ejemplo de ejecución paralela de US1

```text
T012: verificar referencia del backend en app/NetRentManager.sln
T013: verificar referencia de pruebas backend en app/NetRentManager.sln
T014: verificar referencia del frontend en app/NetRentManager.sln
T015: verificar referencia de pruebas frontend en app/NetRentManager.sln
```

### Ejemplo de ejecución paralela de US2

```text
T017: revisar Program.cs del backend
T018: revisar Program.cs del frontend
T019: revisar ausencia de artefactos de negocio en backend
T020: revisar ausencia de features de producto en frontend
```

## Estrategia de implementación

1. Completar la preparación y los fundamentos para obtener una solución compilable.
2. Entregar US1 como MVP: solución única, cuatro proyectos y pruebas base.
3. Validar US2 para impedir que la fundación acumule lógica de negocio.
4. Validar US3 para asegurar estabilidad estructural antes de iniciar la siguiente spec.
5. Ejecutar el pulido y las verificaciones completas; no marcar tareas como `[X]` antes de comprobarlas.
