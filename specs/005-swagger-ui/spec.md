# Especificación de la Funcionalidad: Interfaz Swagger UI del Backend

**Rama de la funcionalidad**: `005-swagger-ui`

**Creado**: 2026-09-11

**Estado**: Implementada

**Trazabilidad de estado**:

- `Borrador` -> `Aprobada` | Motivo: alcance definido para habilitar Swagger UI con spec, plan y tareas completos | Fecha: 2026-09-11
- `Aprobada` -> `En implementación` | Motivo: inicio de la implementación tras validar prerrequisitos de la iniciativa | Fecha: 2026-09-11
- `En implementación` -> `Implementada` | Motivo: tareas T001-T008 completadas, build correcto, 3 pruebas relevantes en verde y validación manual de `/swagger` y `/openapi/v1.json` registrada en `quickstart.md` | Fecha: 2026-09-11

**Entrada**: Descripción de usuario: "Quiero la interfaz swagger UI"

## Escenarios de Usuario y Pruebas *(obligatorio)*

### Historia de Usuario 1 - Abrir la documentación interactiva del backend (Prioridad: P1)

Como consumidor técnico del backend, necesito abrir una interfaz Swagger UI del API, para explorar endpoints y probar solicitudes desde el navegador.

**Por qué esta prioridad**: La UI interactiva es el objetivo principal del cambio y aporta valor inmediato para desarrollo y validación manual.

**Prueba independiente**: Iniciar el backend en `Development` y abrir `/swagger`, comprobando que la página carga y se conecta al documento OpenAPI publicado por la aplicación.

**Escenarios de aceptación**:

1. **Dado** el backend ejecutándose en `Development`, **cuando** navego a `/swagger`, **entonces** veo la interfaz Swagger UI y no una respuesta 404.
2. **Dado** la interfaz Swagger UI disponible, **cuando** se inicializa la página, **entonces** consume el documento OpenAPI expuesto por el backend y lista los endpoints publicados.

### Historia de Usuario 2 - Mantener una exposición segura por entorno (Prioridad: P1)

Como responsable técnico, necesito que Swagger UI siga las mismas restricciones de entorno que OpenAPI, para no ampliar la superficie expuesta fuera del uso local previsto.

**Por qué esta prioridad**: El proyecto ya limita OpenAPI a `Development`; mantener esa regla evita inconsistencias operativas y exposición accidental.

**Prueba independiente**: Revisar la composición de `Program.cs` y comprobar que el registro de Swagger UI queda dentro de la misma condición de entorno de desarrollo.

**Escenarios de aceptación**:

1. **Dado** el backend en `Development`, **cuando** se construye la canalización HTTP, **entonces** se registran el documento OpenAPI y la interfaz Swagger UI.
2. **Dado** el backend fuera de `Development`, **cuando** se construye la canalización HTTP, **entonces** la interfaz Swagger UI no se registra.

### Casos límite

- Si la dependencia necesaria para Swagger UI no está presente, el proyecto debe fallar en compilación y no quedar configurado parcialmente.
- Si cambia la ruta del documento OpenAPI, la interfaz Swagger UI debe apuntar al documento real publicado por la aplicación.
- Si el backend arranca en un entorno distinto de `Development`, la aplicación no debe exponer `/swagger` por accidente.

## Requisitos *(obligatorio)*

### Requisitos funcionales

- **RF-001**: El backend DEBE exponer una interfaz Swagger UI accesible en la ruta `/swagger` cuando la aplicación se ejecute en `Development`.
- **RF-002**: El backend DEBE mantener la publicación del documento OpenAPI existente para que la interfaz Swagger UI pueda consumirlo.
- **RF-003**: `Program.cs` DEBE seguir actuando como punto de composición de infraestructura, sin introducir controllers ni lógica de negocio.
- **RF-004**: La configuración de Swagger UI DEBE ser compatible con ASP.NET Core Minimal APIs y con la solución actual basada en `AddOpenApi()`.
- **RF-005**: El cambio DEBE quedar cubierto por pruebas automatizadas coherentes con el estilo actual del proyecto.

## Criterios de Éxito *(obligatorio)*

### Resultados medibles

- **CE-001**: En `Development`, la ruta `/swagger` deja de responder 404 y sirve la interfaz Swagger UI.
- **CE-002**: La solución compila correctamente tras añadir la UI sin romper la publicación actual del documento OpenAPI.
- **CE-003**: Las pruebas relevantes del backend validan la composición esperada para Swagger UI y OpenAPI.

## Suposiciones

- La ruta deseada para la UI es `/swagger`.
- La exposición de la UI debe mantenerse solo en `Development`, igual que el documento OpenAPI actual.
- Se reutilizará el documento OpenAPI ya publicado por el proyecto en lugar de definir un contrato paralelo.