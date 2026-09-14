# Guia del agente de NetRentManager

## Resumen del proyecto

NetRentManager es una solucion web fullstack unica. El repositorio contiene un backend ASP.NET Core Minimal API, un frontend Blazor Web App con Razor Components y proyectos de pruebas para ambas capas. Las iniciativas funcionales se documentan y ejecutan mediante Spec-Driven Development en `specs/`.

La spec activa es `specs/015-signalr-interactive-server/`. Su alcance habilita Interactive Server de forma granular en las paginas de listado y detalle del frontend. `spec.md` declara la iniciativa como implementada, aunque `tasks.md` conserva seis tareas sin marcar que deben reconciliarse.

## Stack y restricciones tecnicas

- .NET SDK 10.0.400, fijado en `global.json`.
- Backend: ASP.NET Core Minimal APIs con Vertical Slice Architecture.
- Frontend: Blazor Web App y Razor Components.
- Persistencia: EF Core, Npgsql y PostgreSQL.
- Comunicacion frontend-backend: Refit con `IHttpClientFactory`.
- Validacion y errores: FluentValidation y ProblemDetails.
- Estilos: CSS propio centralizado en `wwwroot/app.css`; no agregar frameworks CSS.
- Iconos: Lucide Icons como sistema principal.
- No usar controllers.
- No crear soluciones paralelas ni mover las specs fuera de `specs/`.
- No configurar entidades EF Core inline en `OnModelCreating`.
- Todo cambio de codigo, prueba, migracion, template o configuracion debe estar trazado a una tarea de `tasks.md`.
- Todo Markdown propio del repositorio debe estar en espanol. Los nombres tecnicos, APIs, comandos, namespaces y paquetes pueden conservar su forma original.
- La interactividad de servidor de la spec 015 debe permanecer limitada a `Home.razor` y `PropertyDetailPage.razor`; no habilitarla globalmente en `App.razor` o `Routes.razor`.

## Estructura vigente

```text
/
|-- .github/
|   |-- agents/                 # Agentes especializados del flujo de trabajo
|   |-- instructions/           # Instrucciones por dominio
|   |-- prompts/                # Prompts operativos del repositorio
|   |-- skills/                 # Skills del proyecto
|   `-- workflows/              # Workflows auxiliares
|-- .specify/
|   |-- memory/                 # Constitucion y memoria de gobernanza
|   |-- scripts/                # Scripts de Spec Kit
|   |-- templates/              # Plantillas de specs, planes y tareas
|   `-- workflows/              # Workflows de Spec Kit
|-- app/
|   |-- NetRentManager.sln      # Solucion unica
|   |-- backend/
|   |   |-- src/NetRentManagerApi/
|   |   `-- tests/NetRentManagerApiTests/
|   `-- frontend/
|       |-- src/NetRentManagerWeb/
|       `-- test/NetRentManagerWeb/
|-- artifacts/                  # Salidas generadas y clientes smoke
|-- specs/                      # Especificaciones y artefactos trazables
`-- support/scripts/            # Scripts operativos del repositorio
```

Los directorios `bin/`, `obj/`, `.vs/` y `node_modules/` son salidas o dependencias locales y no son superficies de implementacion.

## Modulos clave y responsabilidades

### Backend

- `app/backend/src/NetRentManagerApi/Program.cs`: composicion de servicios, middleware, Swagger UI de desarrollo, archivos estaticos, migracion y mapeo centralizado de slices.
- `app/backend/src/NetRentManagerApi/Infrastructure/`: infraestructura transversal, endpoints, errores, handlers, validacion y persistencia.
- `app/backend/src/NetRentManagerApi/Features/`: slices de funcionalidad de negocio organizados por feature.
- `app/backend/src/NetRentManagerApi/Domain/`: entidades y reglas del dominio.
- `app/backend/tests/NetRentManagerApiTests/`: pruebas unitarias y pruebas aprobadas por las specs para el backend.

### Frontend

- `app/frontend/src/NetRentManagerWeb/Program.cs`: registro de Razor Components, Interactive Server, clientes Refit y pipeline HTTP.
- `app/frontend/src/NetRentManagerWeb/Components/`: host global, rutas, layout y paginas compartidas.
- `app/frontend/src/NetRentManagerWeb/Features/Properties/`: listado y detalle de propiedades, incluyendo sus componentes de carga, paginacion y estados.
- `app/frontend/src/NetRentManagerWeb/Services/Api/`: opciones, registro y contratos de clientes Refit.
- `app/frontend/src/NetRentManagerWeb/wwwroot/app.css`: sistema visual CSS propio.
- `app/frontend/test/NetRentManagerWeb/`: pruebas de composicion, contratos de archivos y comportamiento del frontend.

## Flujo operativo del agente

1. Leer `.github/copilot-instructions.md` y `.specify/memory/constitution.md`.
2. Identificar la spec activa y leer sus `spec.md`, `plan.md` y `tasks.md` antes de escribir codigo.
3. Confirmar que cada cambio propuesto corresponde a una tarea no completada de `tasks.md`.
4. Inspeccionar primero la implementacion y las pruebas cercanas al comportamiento afectado.
5. Mantener los cambios pequenos y dentro del modulo propietario del comportamiento.
6. Ejecutar una validacion enfocada inmediatamente despues de la primera edicion.
7. Ejecutar las pruebas y el build del proyecto afectado; registrar la evidencia en el `quickstart.md` de la spec cuando la tarea lo exija.
8. Marcar una tarea como `[X]` solo despues de completar y verificar su alcance.
9. Revisar que no se hayan modificado backend, contratos, persistencia o configuracion fuera del alcance aprobado.
10. Mantener este archivo sincronizado cuando cambien la estructura, el stack, los flujos o los archivos criticos del repositorio.

Comandos de validacion confirmados para la spec activa:

```powershell
dotnet build app/NetRentManager.sln --no-restore
dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --no-restore
Select-String -Path app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor,app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor -Pattern "@rendermode InteractiveServer"
Select-String -Path app/frontend/src/NetRentManagerWeb/Components/App.razor,app/frontend/src/NetRentManagerWeb/Components/Routes.razor,app/frontend/src/NetRentManagerWeb/Components/Pages/Error.razor,app/frontend/src/NetRentManagerWeb/Components/Pages/NotFound.razor -Pattern "@rendermode|InteractiveServer" -ErrorAction SilentlyContinue
```

Para la validacion manual de la spec 015 se ejecutan backend y frontend con sus perfiles `https` desde las carpetas de proyecto y se comprueban paginacion, retorno, reintento y paginas estaticas. La guia completa esta en `specs/015-signalr-interactive-server/quickstart.md`.

## Reglas de Spec-Driven Development

- Las specs aprobadas en `specs/` son la fuente de verdad.
- El flujo minimo obligatorio es `speckit.specify -> speckit.plan -> speckit.tasks -> speckit.implement`.
- Cada iniciativa debe tener exactamente `spec.md`, `plan.md` y `tasks.md`; los demas documentos son auxiliares trazables.
- Las specs se implementan en orden numerico ascendente.
- No se deben saltar fases ni crear archivos vacios para simular prerrequisitos.
- Toda implementacion debe estar descrita en `spec.md`, explicada tecnicamente en `plan.md` y asignada a una tarea de `tasks.md`.
- Las tareas completadas se marcan `[X]` y las tareas pendientes bloquean el cierre de la iniciativa.
- La evidencia de build, pruebas y validacion manual se registra en `quickstart.md` cuando lo exige el plan.
- La constitucion prevalece sobre preferencias del agente y cualquier divergencia requiere una decision humana explicita.

## Archivos criticos

- `AGENTS.md`: esta guia operativa sincronizada con el repositorio.
- `.github/copilot-instructions.md`: reglas globales del agente y del flujo Spec-Driven.
- `.specify/memory/constitution.md`: constitucion tecnica y de gobernanza con precedencia normativa.
- `global.json`: version exacta del SDK .NET.
- `app/NetRentManager.sln`: composicion de los proyectos de backend, frontend y pruebas.
- `app/backend/src/NetRentManagerApi/Program.cs`: composicion y pipeline del backend.
- `app/frontend/src/NetRentManagerWeb/Program.cs`: composicion y pipeline del frontend.
- `app/frontend/src/NetRentManagerWeb/Components/Pages/Home.razor`: host interactivo de la ruta `/`.
- `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor`: host interactivo de la ruta de detalle.
- `app/frontend/src/NetRentManagerWeb/Components/App.razor`: host global que debe conservarse sin interactividad global.
- `app/frontend/src/NetRentManagerWeb/Components/Routes.razor`: enrutamiento global que debe conservarse estatico.
- `specs/015-signalr-interactive-server/spec.md`: alcance funcional y criterios de aceptacion activos.
- `specs/015-signalr-interactive-server/plan.md`: decisiones tecnicas y estrategia de validacion.
- `specs/015-signalr-interactive-server/tasks.md`: trazabilidad de implementacion y estado de cierre.
- `specs/015-signalr-interactive-server/quickstart.md`: comandos y evidencia de validacion.

## Estado de la spec activa

La spec `015-signalr-interactive-server` declara el estado `Implementada` y `quickstart.md` registra build, pruebas backend/frontend y validacion HTTP/renderizada. Existe una divergencia de cierre: `tasks.md` conserva pendientes T012-T014 y T019-T021, relacionadas con cobertura de reintentos, validacion manual, registro de evidencia y confirmacion final de alcance. El agente debe resolver esa divergencia antes de tratar la iniciativa como completamente cerrada.

## Ultima sincronizacion

**Fecha**: 2026-09-14

Se actualizo el estado de la spec 015: `spec.md` ahora declara `Implementada` y registra evidencia adicional de build, pruebas y validacion HTTP/renderizada. Se mantiene documentada la divergencia con las seis tareas pendientes de `tasks.md`; no se detectaron cambios estructurales en `app`, `.github` o `.specify`.
