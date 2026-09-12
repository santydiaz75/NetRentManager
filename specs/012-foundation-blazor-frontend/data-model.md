# Modelo de Configuración y Diseño: Fundación del Frontend Blazor

**Feature**: [spec.md](spec.md) | **Fecha**: 2026-09-12

Esta fundación no introduce entidades de dominio ni datos persistentes. Los
"modelos" relevantes son la configuración de comunicación con el backend y los
tokens del sistema visual.

## Configuración: URL base del backend

**Ubicación**: sección `ApiSettings` en `appsettings.json` y
`appsettings.Development.json` de `NetRentManagerWeb`.

| Campo | Descripción |
|---|---|
| `ApiSettings:BaseUrl` | URL base del backend `NetRentManagerApi` (ej. `https://localhost:7226`) |

**Reglas de validación**:
- DEBE existir un valor no vacío en ambos archivos de configuración.
- El `HttpClient` nombrado registrado en `Program.cs` DEBE fallar de forma
  explícita si el valor falta (mensaje claro, no una URL vacía silenciosa).

## Sistema visual: Tokens en `:root` (`wwwroot/app.css`)

| Categoría | Ejemplos de tokens |
|---|---|
| Color | `--color-primary`, `--color-surface`, `--color-text`, `--color-text-muted`, `--color-success`, `--color-warning`, `--color-danger` |
| Espaciado | `--space-1` … `--space-8` |
| Bordes | `--radius-sm`, `--radius-md`, `--radius-lg` |
| Sombras | `--shadow-sm`, `--shadow-md` |
| Layout | `--sidebar-width`, `--sidebar-width-compact` |

## Sistema visual: Clases de layout

| Clase | Rol |
|---|---|
| `.app-shell` | Contenedor raíz: sidebar + contenido principal |
| `.app-sidebar` | Sidebar fija a la izquierda |
| `.sidebar-brand` | Marca/logo en la parte superior de la sidebar |
| `.sidebar-nav` | Lista de navegación vertical |
| `.sidebar-link` / `.sidebar-link-active` | Ítem de navegación y su estado activo |
| `.app-main` | Contenedor del contenido principal |
| `.app-content` | Área interna con padding, donde se renderiza `@Body` |
| `.page-container` | Contenedor de una página individual |
| `.page-header` / `.page-title` / `.page-subtitle` / `.page-actions` | Encabezado de página |

## Sistema visual: Estados de pantalla

| Clase | Estado |
|---|---|
| `.state-loading` | Cargando |
| `.state-empty` | Sin datos |
| `.state-error` | Error |
| `.state-success` | Éxito con datos (contenedor neutro, no un estado visual exclusivo) |

## Sistema visual: Íconos

| Clase | Tamaño |
|---|---|
| `.icon` / `.icon-md` | 1.25rem |
| `.icon-sm` | 1rem |
| `.icon-lg` | 1.5rem |

Todas heredan color mediante `currentColor` salvo variantes explícitas
(`.icon-muted`, `.icon-primary`, etc.) ya documentadas en
`frontend.instructions.md`.
