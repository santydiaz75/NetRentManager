/speckit.bug.assess slug=property-update-render-error

Se detectó un error durante la prueba manual de la funcionalidad de actualización de propiedades.

Feature relacionada:

`specs/016-blazor-update-property`

Documentos relacionados:

* `specs/016-blazor-update-property/spec.md`
* `specs/016-blazor-update-property/plan.md`
* `specs/016-blazor-update-property/tasks.md`

Pasos para reproducir:

1. Abrir el listado de propiedades.
2. Seleccionar Editar en una propiedad existente.
3. Navegar al formulario de actualización.
4. Observar que la página no puede renderizarse.

Resultado esperado:

El formulario `PropertyUpdateForm` debe renderizarse correctamente, cargar la información actual de la propiedad y permitir su modificación.

Resultado actual:

Se produce una excepción no controlada durante el renderizado:

```text
Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware[1]
      An unhandled exception has occurred while executing the request.
      System.InvalidOperationException: Render output is invalid for component of type 'RealtorWeb.Features.Properties.Update.PropertyUpdateForm'. A frame of type 'Element' was left unclosed. Do not use try/catch inside rendering logic, because partial output cannot be undone.
         at Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder.AssertTreeIsValid(IComponent component)
         at Microsoft.AspNetCore.Components.Rendering.ComponentState.RenderIntoBatch(RenderBatchBuilder batchBuilder, RenderFragment renderFragment, Exception& renderFragmentException)
         at Microsoft.AspNetCore.Components.RenderTree.Renderer.ProcessRenderQueue()
      --- End of stack trace from previous location ---
         at Microsoft.AspNetCore.Components.RenderTree.Renderer.ProcessRenderQueue()
         at Microsoft.AspNetCore.Components.ComponentBase.StateHasChanged()
         at Microsoft.AspNetCore.Components.ComponentBase.CallOnParametersSetAsync()
         at Microsoft.AspNetCore.Components.ComponentBase.RunInitAndSetParametersAsync()
         at Microsoft.AspNetCore.Components.Rendering.ComponentState.SetDirectParameters(ParameterView parameters)
         at Microsoft.AspNetCore.Components.RenderTree.Renderer.RenderRootComponentAsync(Int32 componentId, ParameterView initialParameters)
         at Microsoft.AspNetCore.Components.HtmlRendering.Infrastructure.StaticHtmlRenderer.BeginRenderingComponent(IComponent component, ParameterView initialParameters)
         at Microsoft.AspNetCore.Components.Endpoints.EndpointHtmlRenderer.RenderEndpointComponent(HttpContext httpContext, Type rootComponentType, ParameterView parameters, Boolean waitForQuiescence)
```

Analiza especialmente:

* `PropertyUpdateForm.razor`.
* Los bloques `@if`, `@else`, `@foreach` y `@switch`.
* El uso de `try/catch` dentro del markup.
* El uso de `RenderFragment` o `RenderTreeBuilder`.
* Las llamadas `OpenElement` y `CloseElement`.
* Los estados `loading`, `error`, `not-found` y `loaded`.
* Los componentes hijos utilizados por el formulario.
* Las diferencias entre el comportamiento implementado y los criterios de aceptación de `spec.md`.
* Las tareas relacionadas con la actualización de propiedades en `tasks.md`.

Determina:

1. La causa raíz del problema.
2. Los archivos probablemente afectados.
3. La corrección mínima recomendada.
4. Las pruebas automatizadas que deben agregarse o actualizarse.
5. El escenario de prueba manual que debe repetirse.
6. Si existe un gap en `spec.md`, `plan.md` o `tasks.md`.

No modifiques todavía el código fuente.

Genera únicamente el diagnóstico y la propuesta de remediación correspondiente al bug `property-update-render-error`.