# Tareas: Hotfix del Detalle de Propiedades

**Entrada**: Evidencia retrospectiva de `specs/016-property-detail-hotfix/spec.md` y `specs/016-property-detail-hotfix/plan.md`

**Prerequisitos**: `spec.md`, `plan.md`, `checklists/requirements.md` y comprobaciones existentes del frontend.

**Organización**: Las tareas están agrupadas por historia de usuario para permitir validación y evidencia independientes.

## Dependencias y orden de ejecución

- La Fase 1 confirma que el detalle de propiedades y la navegación principal forman la base observable del hotfix.
- US1 bloquea la resolución inequívoca de la URL de detalle y la presencia del enlace de retorno.
- US2 depende de US1 para conservar la composición del detalle sin mezclar el contenedor general con el contenido del inmueble.
- US3 depende de US1 y US2 para confirmar que la navegación principal presenta un icono válido y coherente.
- La validación final y la evidencia manual quedan pendientes si no se ha ejecutado la comprobación del comportamiento visual en el navegador.

## Fase 1: Preparación y comprobación del estado actual

**Propósito**: Confirmar la evidencia disponible del hotfix sin introducir cambios de implementación.

- [X] T001 [P] Confirmar que `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor` define la ruta canónica `/properties/{id}` y el rendermode `InteractiveServer` sin duplicar vistas funcionales.
- [X] T002 [P] Revisar `app/frontend/test/NetRentManagerWeb/InteractiveServerBehaviorTests.cs` para verificar que la vista de detalle conserva el enlace "Volver al listado" y evita aplicar el grid del detalle al contenedor general de la página.
- [X] T003 [P] Revisar `app/frontend/test/NetRentManagerWeb/InteractiveServerRenderModeTests.cs` para confirmar que la página de detalle y la home declaran `@rendermode InteractiveServer` exactamente una vez y sin duplicar el host interactivo.
- [X] T004 [P] Inspeccionar `app/frontend/src/NetRentManagerWeb/Components/Layout/NavMenu.razor` para verificar que el acceso a propiedades usa un icono válido y una etiqueta clara en la navegación principal.

**Punto de control**: La evidencia del repositorio valida los síntomas del hotfix y no sugiere una implementación nueva ni una reestructuración del frontend.

## Fase 2: Historia de Usuario 1 - Abrir el detalle sin conflicto de navegación (Prioridad: P1)

**Objetivo**: Garantizar que la URL de detalle resuelve una sola vista y no bloquea la navegación.

**Prueba independiente**: Abrir una URL válida de detalle y comprobar que la aplicación carga la propiedad sin error de coincidencia ambigua.

### Tareas de implementación y evidencia

- [X] T005 [US1] Validar en `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor` que la vista de detalle sigue siendo la única representación del contenido y conserva el enlace raíz `/`.
- [X] T006 [US1] Revisar `app/frontend/test/NetRentManagerWeb/PropertyDetailPageTests.cs` para confirmar que la carga del detalle maneja éxito, no encontrado y errores sin romper la navegación funcional.
- [X] T007 [US1] Confirmar que `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor.cs` valida `Guid` y evita un estado de detalle ambiguo cuando el identificador no es válido.

**Punto de control**: US1 ofrece una resolución inequívoca del detalle y conserva la navegación de retorno al listado.

## Fase 3: Historia de Usuario 2 - Ver el detalle con composición visual correcta (Prioridad: P1)

**Objetivo**: Separar el encabezado del bloque principal y mantener la jerarquía visual del contenido del inmueble.

**Prueba independiente**: Abrir una propiedad válida y comprobar que la acción de regreso se mantiene en el encabezado superior mientras la imagen y los datos aparecen en el bloque principal.

### Tareas de implementación y evidencia

- [X] T008 [US2] Inspeccionar `app/frontend/src/NetRentManagerWeb/Features/Properties/Detail/PropertyDetailPage.razor` para confirmar la estructura `page-container` + `page-header` + `article class="card property-detail"` y que no se aplica el grid del detalle al contenedor general.
- [X] T009 [US2] Revisar `app/frontend/src/NetRentManagerWeb/wwwroot/app.css` para verificar que `property-detail`, `property-detail-media`, `property-detail-body` y `property-detail-placeholder` separan el contenido visual del detalle del encabezado de la página.
- [X] T010 [US2] Confirmar en `app/frontend/test/NetRentManagerWeb/InteractiveServerBehaviorTests.cs` que la regla del hotfix conserva el enlace de retorno y no reubica la imagen junto al botón de regreso.

**Punto de control**: US2 mantiene la composición visual estable, con el contenido del inmueble separado del encabezado y del retorno al listado.

## Fase 4: Historia de Usuario 3 - Navegar con señales visuales coherentes (Prioridad: P2)

**Objetivo**: Asegurar que la navegación principal no presenta un icono roto o una señal visual errónea.

**Prueba independiente**: Cargar la aplicación y comprobar que el acceso a propiedades usa un icono válido y una etiqueta legible.

### Tareas de implementación y evidencia

- [X] T011 [US3] Revisar `app/frontend/src/NetRentManagerWeb/Components/Layout/NavMenu.razor` para confirmar que el acceso a propiedades renderiza `LucideIcon Name="building-2"` con una navegación visible y coherente.
- [X] T012 [US3] Comprobar `app/frontend/test/NetRentManagerWeb/IconSystemTests.cs` para verificar que la navegación principal referencia un icono del sistema Lucide válido y no un símbolo de advertencia o marcador defectuoso.

**Punto de control**: US3 elimina la señal visual errónea de la navegación principal sin introducir nuevos elementos no previstos.

## Fase 5: Validación y evidencia final

**Propósito**: Completar la evidencia documental del hotfix y dejar pendiente la verificación manual del navegador si no se ha ejecutado.

- [ ] T013 [P] Ejecutar `dotnet test app/frontend/test/NetRentManagerWeb/NetRentManagerWeb.csproj --nologo` y registrar el resultado para confirmar la regresión del hotfix en la suite actual del frontend.
- [ ] T014 [P] Validar manualmente en el navegador que `/properties/{id}` abre el detalle correctamente, el encabezado mantiene el acceso de retorno y la navegación principal muestra el icono correcto.
- [ ] T015 Crear o actualizar `specs/016-property-detail-hotfix/quickstart.md` con la evidencia de pruebas y validación manual para cerrar la trazabilidad documental del hotfix.
- [ ] T016 Revisar `specs/016-property-detail-hotfix/checklists/requirements.md` y confirmar que cada requisito queda cubierto por la evidencia disponible y por las verificaciones pendientes.

## Dependencias de validación

- T013 depende de la estructura actual del proyecto y puede ejecutarse sin cambios funcionales adicionales.
- T014 es una validación manual opcional para cierre documental y no sustituye la evidencia de tests existentes.
- T015 y T016 solo deben cerrarse cuando la evidencia de validación esté registrada y sea consistente con el comportamiento observable.

## Oportunidades de paralelización

- T001-T004 pueden revisarse en paralelo porque inspeccionan archivos distintos del frontend.
- T005-T007 son pruebas y comprobaciones del flujo principal del detalle y pueden validarse en paralelo dentro de US1.
- T008-T010 representan la validación del layout visual del detalle y pueden ejecutarse en paralelo sobre archivos distintos.
- T011-T012 validan la navegación principal y no dependen del detalle visual, por lo que son paralelizables.
- T013-T016 requieren coordinación de evidencia y no deben cerrarse sin la validación final del comportamiento observable.

## Estrategia de implementación

### MVP retrospectivo

1. Confirmar la resolución de la ruta y del enlace de retorno.
2. Validar la separación del encabezado y del bloque del inmueble.
3. Verificar la navegación principal con un icono válido.
4. Dejar la validación automatizada y la evidencia documental como cierre formal si aún no se ejecutaron.

### Entrega incremental

1. US1: resolución inequívoca de la URL y retorno al listado.
2. US2: composición visual y marcado del detalle.
3. US3: señal visual coherente en la navegación principal.
4. Validación final: tests, comprobación manual y registro de evidencia.

## Criterios de prueba independientes

- **US1**: Una URL válida de detalle de propiedad carga una única vista sin errores de coincidencia ambigua.
- **US2**: La pantalla de detalle mantiene el encabezado, la acción de retorno y el contenido principal separados y legibles.
- **US3**: La navegación principal presenta un icono válido y no una señal defectuosa o ambigua.
- **Validación final**: La evidencia disponible en tests y revisión manual queda registrada y es consistente con el comportamiento observable.

## Notas de trazabilidad

- El hotfix documentado se limita a estabilizar el detalle de propiedades y la navegación asociada sin introducir flujos ni entidades nuevas.
- La evidencia existente en tests y fuente del frontend permite verificar la corrección retrospectiva antes de cerrar la documentación formal.
- Los estados pendientes corresponden únicamente a validación manual y registro documental, no a cambios de implementación no evidenciados.
