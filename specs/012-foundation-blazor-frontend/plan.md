# Plan de Implementación: Fundación del Frontend Blazor

**Rama**: `012-foundation-blazor-frontend` | **Fecha**: 2026-09-12 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/012-foundation-blazor-frontend/spec.md`

## Resumen

Establecer la fundación visual y de comunicación del frontend `NetRentManagerWeb`:
sistema visual propio en `wwwroot/app.css` (tokens, layout, botones, cards,
formularios, badges, estados de pantalla), un layout base desktop-first con
sidebar fija y navegación representativa de secciones futuras, íconos Lucide
mediante el paquete `BlazorBlueprint.Icons.Lucide`, y el andamiaje base de
comunicación tipada con el backend mediante Refit registrado con
`IHttpClientFactory`. No se agrega ninguna pantalla ni endpoint de negocio.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10, Blazor Web App (Razor Components), SDK
fijado por `global.json`.

**Dependencias principales**: `Refit` + `Refit.HttpClientFactory` para el
andamiaje de comunicación tipada; `BlazorBlueprint.Icons.Lucide` para íconos
Lucide como componentes Razor; ningún framework CSS ni librería de íconos
adicional.

**Storage**: N/A (esta fundación no persiste ni consulta datos).

**Pruebas**: xUnit para el proyecto `NetRentManagerWebTests`, validando ausencia
de referencias a frameworks CSS/íconos de terceros, presencia de las clases
base en `app.css`, registro correcto del `HttpClient`/Refit y que ningún
componente use `HttpClient` directamente.

**Plataforma objetivo**: ASP.NET Core .NET 10, Blazor Web App renderizado en
modo Static SSR por defecto (sin cambios de render mode en esta fundación),
ejecutado localmente desde `app/frontend/src/NetRentManagerWeb`.

**Tipo de proyecto**: Aplicación web Blazor (frontend) que consumirá el backend
`NetRentManagerApi` existente.

**Objetivos de rendimiento**: sin objetivos de rendimiento específicos; el
layout y los estilos son estáticos y no introducen llamadas de red adicionales.

**Restricciones**: sin frameworks CSS de terceros, sin librerías de íconos
distintas de Lucide, sin pantallas ni interfaces Refit de negocio, sin cambios
en el backend, sin estilos inline ni colores hardcodeados en Razor/CSS.

**Escala/Alcance**: un layout base, un archivo `app.css` ampliado, un paquete
NuGet de íconos, un registro de `HttpClient`/Refit, y ajustes a `Home.razor`,
`Error.razor` y `NotFound.razor` para usar el nuevo sistema visual.

## Comprobación de Constitución

*Puerta de control: debe pasar antes de la investigación y repetirse después del diseño.*

- **Aprobado**: la spec está en `Borrador` y el plan no modifica su estado.
- **Aprobado**: se mantiene el stack obligatorio: Blazor Web App, Razor
  Components, CSS propio en `wwwroot/app.css`, Refit, Lucide Icons.
- **Aprobado**: no se introduce ningún framework CSS ni librería de íconos
  externa como fuente principal.
- **Aprobado**: no se agregan controllers, lógica de negocio, migraciones ni
  cambios de backend.
- **Aprobado**: no se crean pantallas ni interfaces Refit de propiedades; solo
  el andamiaje base de comunicación.
- **Aprobado**: las pruebas y la evidencia de `quickstart.md` serán
  prerrequisitos para cualquier cierre posterior como `Implementada`.

No existen violaciones constitucionales que justificar.

## Estructura del proyecto

### Documentación (esta funcionalidad)

```text
specs/012-foundation-blazor-frontend/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── checklists/requirements.md
└── tasks.md             # Lo generará /speckit.tasks
```

### Código fuente (repositorio)

```text
app/frontend/src/NetRentManagerWeb/
├── NetRentManagerWeb.csproj                 # Agrega Refit, Refit.HttpClientFactory y BlazorBlueprint.Icons.Lucide
├── Program.cs                               # Registra HttpClient/Refit y AddNetRentManagerApiClient
├── appsettings.json                         # Agrega ApiSettings:BaseUrl (u. equivalente) del backend
├── appsettings.Development.json             # Idem para desarrollo local
├── Components/
│   ├── App.razor                            # Sin cambios de fondo (ya no referencia frameworks externos)
│   ├── Layout/
│   │   ├── MainLayout.razor                 # Reemplazado por el layout .app-shell/.app-sidebar/.app-main
│   │   ├── MainLayout.razor.css             # Eliminado o vaciado: estilos migran a app.css
│   │   └── NavMenu.razor (+ .razor.cs)       # Nuevo: navegación vertical de la sidebar con íconos Lucide
│   └── Pages/
│       ├── Home.razor                       # Placeholder adaptado a .page-container/.page-header
│       ├── Error.razor                      # Adaptado al sistema visual (sin Bootstrap)
│       └── NotFound.razor                   # Adaptado al sistema visual (sin Bootstrap)
├── Services/
│   └── Api/
│       └── NetRentManagerApiClientRegistration.cs  # Registro del HttpClient nombrado + Refit base
└── wwwroot/
    └── app.css                              # Tokens, layout, sidebar, botones, cards, formularios,
                                              # badges, estados de pantalla e íconos

app/frontend/test/NetRentManagerWeb/
└── (NetRentManagerWebTests)                 # Pruebas de composición, ausencia de terceros y registro DI
```

**Decisión estructural**: el andamiaje de comunicación vive en
`Services/Api/` (carpeta que reutilizarán las specs futuras de propiedades,
como `013-blazor-list-properties`, para agregar `IPropertiesApi` en
`Services/Api/Properties/`). La navegación de la sidebar se extrae a un
componente `NavMenu` propio dentro de `Components/Layout/`, reemplazando el
`NavMenu` por defecto de la plantilla (que no existe todavía en este proyecto).
No se crean carpetas de features de negocio en esta fundación.

## Investigación y decisiones

Las decisiones están consolidadas en [research.md](research.md):

- confirmación de que el proyecto no contiene Bootstrap ni librerías de
  terceros hoy, por lo que el alcance es preservar esa ausencia, no eliminar
  archivos;
- selección de `BlazorBlueprint.Icons.Lucide` como paquete de íconos Lucide;
- diseño de tokens y clases base en `app.css` siguiendo el skill
  `blazor-app-css-design-system`;
- puntos de quiebre responsive para sidebar (desktop, tablet, móvil);
- registro del `HttpClient` tipado hacia el backend mediante
  `IHttpClientFactory`, con la URL base leída de `appsettings.json`.

## Diseño técnico

### Layout y sistema visual

1. `wwwroot/app.css` se amplía con tokens en `:root` (colores, espaciado,
   bordes, sombras) y las clases de layout, sidebar, botones, cards,
   formularios, badges, estados de pantalla e íconos exigidas por FR-002/FR-003/FR-010.
2. `MainLayout.razor` se reescribe para renderizar `.app-shell` con
   `.app-sidebar` (que incluye `NavMenu`) y `.app-main`/`.app-content` para
   `@Body`.
3. `NavMenu.razor` renderiza enlaces representativos ("Inicio", "Propiedades")
   con íconos Lucide y clases `.sidebar-link`/`.sidebar-link-active`.
4. `Home.razor`, `Error.razor` y `NotFound.razor` se adaptan para usar
   `.page-container`/`.page-header` y contenido placeholder, sin lógica de
   negocio.

### Comunicación con backend

1. Se agregan las referencias NuGet `Refit` y `Refit.HttpClientFactory` al
   `.csproj`.
2. `appsettings.json`/`appsettings.Development.json` agregan la URL base del
   backend (por ejemplo `ApiSettings:BaseUrl`).
3. `Program.cs` registra un `HttpClient` nombrado hacia el backend mediante
   `IHttpClientFactory`, leyendo la URL base desde configuración; el registro
   queda listo para que features futuras agreguen `AddRefitClient<TInterface>()`
   sobre el mismo `HttpClient` nombrado, sin necesitar reconfigurar la base
   address.
4. No se define ninguna interfaz Refit de negocio en esta fundación.

### Íconos

1. Se agrega la referencia NuGet `BlazorBlueprint.Icons.Lucide`.
2. Los íconos se usan únicamente dentro de `NavMenu.razor` (ítems de
   navegación) en esta fundación, con las clases `.icon`/`.icon-sm` definidas
   en `app.css`.

### Validación

- Pruebas unitarias verifican: ausencia de referencias a frameworks CSS/íconos
  de terceros en el HTML servido y en el proyecto; presencia de las clases
  base esperadas en `app.css`; registro del `HttpClient` nombrado sin
  interfaces Refit de negocio; ningún componente Razor usa `HttpClient`
  directamente.
- `quickstart.md` documenta los pasos manuales para verificar visualmente el
  layout en viewports de escritorio, tablet y móvil.

## Complexity Tracking

No existen violaciones constitucionales que justificar en esta iniciativa.

