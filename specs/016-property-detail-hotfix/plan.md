# Plan de Implementación: Hotfix del Detalle de Propiedades

**Rama**: `016-property-detail-hotfix` | **Fecha**: 2026-09-13 | **Spec**: [spec.md](spec.md)

**Entrada**: Especificación de `specs/016-property-detail-hotfix/spec.md`

## Resumen

Este plan documenta retrospectivamente un hotfix ya aplicado sobre el flujo de detalle de propiedades en el frontend Blazor. La evidencia disponible no proviene de un diff funcional activo, sino del estado actual del repositorio, de las pruebas relacionadas y del comportamiento observable de la aplicación. La corrección estabiliza la apertura de `/properties/{id}`, ordena la composición visual del detalle y elimina una señal visual errónea en la navegación principal.

## Contexto técnico

**Lenguaje/Versión**: C# con .NET 10 y Razor Components.

**Dependencias principales**: Blazor Web App, Lucide Icons y pruebas xUnit del frontend.

**Storage**: N/A.

**Pruebas**: pruebas unitarias y de inspección de archivos en `app/frontend/test/NetRentManagerWeb`, más validación visual manual del frontend.

**Plataforma objetivo**: aplicación web frontend `NetRentManagerWeb`.

**Tipo de proyecto**: aplicación web Blazor.

**Objetivos de rendimiento**: mantener una resolución inequívoca de la ruta de detalle sin añadir trabajo extra a otras pantallas.

**Restricciones**: documentación retrospectiva únicamente; no rediseñar la solución, no proponer una implementación nueva y no modificar código como parte de este plan.

**Escala/Alcance**: estabilización del detalle de propiedades y de su navegación visible inmediata.

## Constitution Check

*GATE: Debe pasar antes de continuar con la documentación técnica retrospectiva.*

- La spec declara un estado canónico válido para la fase documental actual.
- El alcance permanece dentro del stack obligatorio: Blazor Web App, Razor Components, CSS propio y Lucide Icons.
- El hotfix documentado afecta solo al frontend y no introduce cambios en backend, persistencia ni contratos de negocio nuevos.
- No se rediseña la arquitectura ni se documentan capacidades fuera del comportamiento ya observable.
- Una futura transición a `Implementada` requerirá `tasks.md` y evidencia adicional en artefactos de validación si el equipo decide completar el ciclo formal completo para esta spec retrospectiva.

## Estructura del proyecto

### Documentación

```text
specs/016-property-detail-hotfix/
├── spec.md
├── plan.md
└── checklists/
    └── requirements.md
```

### Código fuente

```text
app/frontend/src/NetRentManagerWeb/
├── Components/
│   └── Layout/
│       └── NavMenu.razor
├── Features/
│   └── Properties/
│       └── Detail/
│           └── PropertyDetailPage.razor
└── wwwroot/
    └── app.css

app/frontend/test/NetRentManagerWeb/
├── InteractiveServerBehaviorTests.cs
├── InteractiveServerRenderModeTests.cs
└── PropertyDetailPageTests.cs
```

**Decisión estructural**: la documentación retrospectiva se centra en los artefactos del frontend que permiten observar el resultado del hotfix sin reconstruir el historial ni inventar pasos intermedios no verificables.

## Causa raíz

1. La URL de detalle de propiedades llegó a estar asociada a más de una vista funcional, provocando una resolución ambigua al navegar a una propiedad concreta.
2. La composición visual del detalle mezclaba el contenedor general de página con el bloque visual del inmueble, lo que desplazaba la imagen junto a la acción de regreso y degradaba la jerarquía visual.
3. La navegación principal referenciaba un icono que en ejecución no se mostraba correctamente y terminaba representado por una señal de alerta.

## Componentes afectados

- `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor`
- `app/frontend/src/NetRentManagerWeb/Components/Layout/NavMenu.razor`
- `app/frontend/src/NetRentManagerWeb/wwwroot/app.css`
- `app/frontend/test/NetRentManagerWeb/InteractiveServerBehaviorTests.cs`
- `app/frontend/test/NetRentManagerWeb/InteractiveServerRenderModeTests.cs`
- `app/frontend/test/NetRentManagerWeb/PropertyDetailPageTests.cs`

## Solución aplicada

- Se consolidó una única vista canónica para la URL de detalle de propiedades.
- Se separó el encabezado de página del bloque visual principal del inmueble para que la acción de regreso permanezca arriba y el contenido del detalle se renderice en su tarjeta correspondiente.
- Se mantuvo el marcador visual para propiedades sin imagen, preservando una presentación estable.
- Se sustituyó el icono inválido de la navegación principal por un icono válido ya coherente con la interfaz observable.
- Se reforzó la cobertura retrospectiva con pruebas de inspección del marcado relacionadas con la composición del detalle y la presencia del enlace de retorno.

## Decisiones técnicas

- Tomar el estado actual del repositorio como fuente principal de evidencia, ya que el diff funcional activo no está disponible.
- Documentar el hotfix como una estabilización del flujo de detalle de propiedades, agrupando síntomas relacionados de navegación y presentación en una sola iniciativa pequeña.
- Preservar la vista de detalle actualmente observable como implementación canónica, en lugar de proponer refactorizaciones o estructuras alternativas.
- Usar pruebas existentes del frontend como respaldo documental de la corrección, complementadas con validación manual del comportamiento visible.

## Riesgos

- La ausencia de diff activo reduce la trazabilidad exacta del orden histórico de los cambios y obliga a documentar a partir del resultado final.
- Existen pruebas del frontend ajenas a este hotfix que pueden estar desalineadas con el markup actual y generar ruido durante validaciones globales.
- Si en el futuro reaparece una segunda definición funcional para la URL de detalle, el problema de ambigüedad podría reintroducirse sin detectarse de inmediato fuera de las pruebas específicas.

## Pruebas realizadas

- Verificación de que la ruta de detalle queda representada por una única vista funcional.
- Prueba `InteractivePages_DeclareInteractiveServerExactlyOnce` para confirmar el host interactivo esperado del detalle.
- Prueba `PropertyDetail_PreservesReturnLinkToRoot` para comprobar la presencia del regreso al listado.
- Prueba `PropertyDetail_DoesNotApplyDetailGridToPageContainer` para validar que el grid visual del detalle no se aplica al contenedor completo de página.
- Pruebas de `PropertyDetailPageTests` para estados de carga, éxito, error y dato inexistente.
- Revisión manual del comportamiento visual del detalle y del icono visible en la navegación principal.

## Posibles efectos secundarios

- La documentación retrospectiva puede quedarse corta frente a una futura auditoría que requiera el diff exacto del cambio original.
- Cambios posteriores en el layout o en el sistema de iconos podrían invalidar parte de la evidencia si no se actualizan las pruebas de inspección.
- La existencia de pruebas heredadas con expectativas antiguas puede dificultar distinguir entre regresiones reales y deuda de test.

## Complejidad

No se identifican violaciones constitucionales ni complejidad adicional que deba justificarse en esta documentación retrospectiva.
