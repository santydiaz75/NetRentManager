# Quickstart: Fundación del Frontend Blazor

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

Esta guía valida que `NetRentManagerWeb` arranca sin dependencias visuales de
terceros, con el layout base, los íconos Lucide y el andamiaje de comunicación
con el backend correctamente establecidos.

## Prerrequisitos

- .NET 10 SDK instalado (según `global.json`).
- Backend `NetRentManagerApi` corriendo localmente (opcional para esta
  fundación, ya que no hay endpoints de negocio consumidos todavía).

## Ejecutar la aplicación

```powershell
cd app/frontend/src/NetRentManagerWeb
dotnet run
```

**Resultado esperado**: la aplicación arranca y sirve la página principal con
la sidebar fija a la izquierda y el contenido principal a la derecha.

## Verificar ausencia de dependencias de terceros

```powershell
(Invoke-WebRequest http://localhost:5000/).Content | Select-String -Pattern "bootstrap|font-awesome|fluent-icons" -Quiet
```

**Resultado esperado**: `False` (ninguna coincidencia).

## Verificar el layout en distintos viewports

1. Abrir la aplicación en el navegador con las herramientas de desarrollo.
2. Verificar en un viewport de escritorio (≥ 1024px): sidebar fija a la
   izquierda con navegación completa (íconos + texto) y contenido principal
   amplio a la derecha.
3. Verificar en un viewport de tablet (~768px): la sidebar se muestra en
   versión compacta (íconos, sin texto).
4. Verificar en un viewport de móvil (~375px): la navegación se colapsa (menú
   tipo drawer), sin ocultar el contenido principal de forma irrecuperable.

## Verificar los íconos Lucide

1. Inspeccionar el HTML de la sidebar en las herramientas de desarrollo.
2. Confirmar que cada ítem de navegación renderiza un `<svg>` (vía
   `BlazorBlueprint.Icons.Lucide`) con clase `.icon`/`.icon-sm`, sin ninguna
   clase `bi bi-*` ni `fa fa-*`.

## Verificar el andamiaje de comunicación con el backend

1. Revisar `Program.cs` y confirmar el registro de un `HttpClient` nombrado
   hacia el backend, con la URL base leída desde `ApiSettings:BaseUrl`.
2. Confirmar que ningún archivo `.razor`/`.razor.cs` referencia `HttpClient`
   directamente ni invoca `RestService.For<T>()`.

## Verificar las clases de estados de pantalla

```powershell
Select-String -Path app/frontend/src/NetRentManagerWeb/wwwroot/app.css -Pattern "\.state-loading|\.state-empty|\.state-error|\.state-success"
```

**Resultado esperado**: las cuatro clases existen en `app.css`.

## Evidencia de validación

**Fecha**: 2026-09-12

- `dotnet build app/NetRentManager.sln --no-restore`: correcto, 0 errores.
- `dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj`:
  23 pruebas, 0 con error (incluye `ThirdPartyDependencyTests`,
  `MainLayoutCompositionTests`, `IconSystemTests`, `ApiClientRegistrationTests`,
  `ScreenStateStylesTests`).
- `dotnet test app/backend/tests/NetRentManagerApiTests/NetRentManagerApiTests.csproj`:
  140 pruebas, 0 con error (sin regresiones en el backend).
- `dotnet run` en `NetRentManagerWeb`: la aplicación arrancó
  (`Now listening on: http://localhost:5251`) y sirvió `GET /` con HTTP 200.
- Inspección del HTML servido: sin referencias a `bootstrap`, `font-awesome`
  ni `fluent-icons`; presentes `app-shell`, `app-sidebar`, `app-main` y
  `app-content`.
- Íconos Lucide: 2 elementos `<svg>` renderizados en la sidebar (uno por cada
  enlace de navegación), sin mensajes de "Icon not found" tras corregir el
  nombre de ícono `home` → `house` (nombre válido en la versión del paquete
  usada).
- Puntos de quiebre responsive `max-width: 1024px` y `max-width: 640px`
  verificados en `app.css` (líneas 283 y 300).
- Registro de comunicación: `Program.cs` registra el `HttpClient` nombrado
  `NetRentManagerApi` con `ApiSettings:BaseUrl`; ningún `.razor`/`.razor.cs`
  referencia `HttpClient` ni `RestService.For` directamente (verificado por
  `ApiClientRegistrationTests`).
