# Investigación: Fundación de la Solución NetRentManager

## Decisión: usar el SDK fijado por `global.json`

**Razonamiento**: `global.json` declara `10.0.400`, por lo que la fundación debe consumir esa versión como baseline y no modificarla.

**Alternativas consideradas**: seleccionar otra versión durante la creación de proyectos. Se rechaza porque rompería la fuente de verdad del repositorio.

## Decisión: crear una solución fullstack única

**Razonamiento**: la constitución exige una solución compartida y la spec define una única solución en `app/NetRentManager.sln`, con backend, frontend y pruebas separadas por capa.

**Alternativas consideradas**: crear soluciones separadas para backend y frontend. Se rechaza porque produciría arquitecturas paralelas.

## Decisión: backend Minimal API sin funcionalidades

**Razonamiento**: la constitución prohíbe controllers y la spec limita el arranque a configuración base, middleware y mapeo inicial de endpoints.

**Alternativas consideradas**: MVC con controllers o agregar slices de negocio. Se rechazan porque contradicen el stack o exceden el alcance foundation.

## Decisión: frontend Blazor Web App con Razor Components

**Razonamiento**: es la tecnología frontend obligatoria y permite dejar una base ejecutable sin introducir features de producto.

**Alternativas consideradas**: Blazor WebAssembly independiente o un frontend separado de la solución. Se rechazan porque no representan la arquitectura canónica definida.

## Decisión: pruebas unitarias base

**Razonamiento**: la constitución permite unit tests y la spec requiere proyectos de pruebas para ambas capas, sin justificar pruebas de integración.

**Alternativas consideradas**: pruebas de integración o E2E. Se difieren hasta que una spec funcional las justifique.

## Decisión: no crear contratos ni modelo persistente

**Razonamiento**: esta iniciativa no expone comportamiento de negocio ni define entidades, almacenamiento o integraciones externas.

**Alternativas consideradas**: crear contratos placeholder, entidades vacías o migraciones iniciales. Se rechazan para evitar artefactos no trazados y alcance ficticio.
