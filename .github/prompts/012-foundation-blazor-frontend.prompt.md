---
name: 012-foundation-blazor-frontend
description: "Genera el spec.md de la funcionalidad 012-foundation-blazor-frontend usando speckit.specify"
agent: speckit.specify
---


Ejecuta `/speckit.specify` con la siguiente descripción para crear la especificación
012-foundation-blazor-frontend.


## Descripción para speckit.specify

Crear la especificación 012-foundation-blazor-frontend para establecer la estructura
base interna del proyecto Blazor `NetRentManagerWeb` (`app/frontend/src/NetRentManagerWeb`), sin lógica
de negocio ni pantallas funcionales de propiedades. Es una spec de fundación del
frontend, equivalente en espíritu a la 002-foundation-backend pero aplicada a la capa de
presentación.

### Contexto y motivación

El proyecto `NetRentManagerWeb` fue generado con la plantilla por defecto de Blazor Web App y
actualmente depende de Bootstrap CSS. Esto viola
`.github/instructions/frontend.instructions.md`, el skill
`.github/skills/blazor-app-css-design-system/SKILL.md` y la constitución, que prohíben todo
framework CSS externo (Bootstrap, Tailwind, Bulma) y toda librería de íconos externa como
fuente principal (Bootstrap Icons, Font Awesome, Fluent). Esta spec debe eliminar esa
dependencia y dejar la base visual y de comunicación correctamente cimentada.

### Objetivos y alcance obligatorio

1. Eliminar Bootstrap y cualquier framework CSS externo: quitar el enlace a
   `lib/bootstrap/dist/css/bootstrap.min.css` en `Components/App.razor`, eliminar la carpeta
   `wwwroot/lib/bootstrap/` y cualquier referencia asociada. La aplicación NO debe cargar
   ningún CSS de terceros.
2. Establecer `wwwroot/app.css` como única fuente de verdad del sistema visual, generado
   y mantenido según el skill `blazor-app-css-design-system`. Debe contener los tokens en
   `:root` (colores, espaciado, bordes, sombras, layout) y las clases base de layout
   (`.app-shell`, `.app-sidebar`, `.app-main`, `.app-content`, `.page-container`,
   `.page-header`, etc.), botones, cards, formularios, badges, estados de pantalla e íconos.
   Prohibido usar estilos inline, colores hardcodeados o valores mágicos en `.razor`/`.cs`.
3. **Definir el layout base desktop first**: sidebar fija a la izquierda con navegación
   vertical (estados hover/active) y contenido principal amplio a la derecha, reemplazando el
   `MainLayout`/`NavMenu` por defecto de la plantilla. Comportamiento responsive definido
   (tablet compacta, móvil con navegación colapsada/drawer).
4. **Establecer Lucide Icons como sistema principal de íconos** (SVG o componentes Razor, con
   `currentColor` y clases `.icon`/`.icon-sm`/`.icon-md`/`.icon-lg` definidas en `app.css`).
   Prohibido Bootstrap Icons u otras librerías. La instalación/integración exacta del paquete
   Lucide para Blazor debe quedar descrita en el plan y trazada en tasks.
5. **Establecer la comunicación tipada con backend mediante Refit** siguiendo
   `frontend.instructions.md`: interfaces Refit organizadas por módulo/feature, registradas
   vía `IHttpClientFactory`, sin usar `HttpClient` directamente en componentes ni
   `RestService.For<T>()` fuera del registro. En esta fundación solo se establece el
   andamiaje/registro base de Refit y su configuración de `HttpClient` hacia `NetRentManagerApi`,
   sin endpoints de negocio.
6. **Definir los patrones base de estados de pantalla** (loading, empty, error, success, con
   datos) como clases reutilizables en `app.css`, disponibles para features futuras.
7. **Limpiar la plantilla por defecto**: eliminar/ajustar estilos hardcodeados heredados en
   `app.css`, `MainLayout.razor.css`, `NavMenu.razor.css` y páginas de ejemplo que dependan
   de Bootstrap, dejando la app compilando con el sistema visual propio.

### Fuera de alcance (no implementar)

- Pantallas o CRUD de propiedades, listados, paginación, formularios de creación/edición o
  cualquier feature de negocio (esas viven en specs posteriores).
- Interfaces Refit con endpoints reales de propiedades (solo andamiaje/registro base).
- Cambios en el backend.

### Restricciones y cumplimiento

- Respetar íntegramente `.github/instructions/frontend.instructions.md` y el skill
  `blazor-app-css-design-system`.
- Mantener el stack: Blazor Web App, Razor Components, CSS propio en `wwwroot/app.css`,
  Refit, Lucide Icons.
- No introducir frameworks CSS ni librerías UI/íconos externas.
- Todo cambio debe quedar trazado a una tarea en `tasks.md`; la spec no puede cerrarse en
  "Implementada" sin tasks completadas y evidencia en `quickstart.md`.

### Criterios de éxito esperados

- La aplicación arranca sin cargar ningún CSS de terceros.
- La sidebar fija y el contenido principal se renderizan con el sistema visual propio.
- Los íconos se sirven con Lucide.
- El registro base de Refit está disponible para features futuras.
- El proyecto compila y no quedan referencias a Bootstrap.