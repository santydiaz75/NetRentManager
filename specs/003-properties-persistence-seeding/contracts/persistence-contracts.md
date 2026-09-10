# Contratos de Persistencia y Assets

## Esquema persistente

### Tabla `property_statuses`

| Campo | Tipo conceptual | Reglas |
|---|---|---|
| `value` | texto | clave primaria, único, valores permitidos del enum |
| `description` | texto | requerido, longitud explícita |

### Tabla `properties`

| Campo | Tipo conceptual | Reglas |
|---|---|---|
| `id` | UUID | clave primaria |
| `title` | texto | requerido, máximo 200 |
| `description` | texto | requerido, máximo 2000 |
| `address` | texto | requerido, máximo 300 |
| `price` | numeric(18,2) | requerido, no negativo |
| `status` | texto | requerido, conversión de `PropertyStatus` |
| `bedroom_count` | entero | requerido, no negativo |
| `bathroom_count` | entero | requerido, no negativo |
| `area_square_meters` | numeric(10,2) | requerido, positivo |
| `image_url` | texto | requerido, máximo 500, ruta pública |
| `created_at` | timestamp con zona | requerido, default UTC |
| `updated_at` | timestamp con zona | opcional |

## Contrato del manifiesto

`support/seed-data/seed-manifest.json` es la fuente única de rutas y debe conservar
las propiedades `version`, `statusesFile`, `propertiesFile`,
`imagesSourceDirectory`, `imagesTargetDirectory` y `publicImageBasePath`.

Las rutas fuente deben existir y ser relativas al repositorio/proyecto según la fase
de ejecución. `publicImageBasePath` debe comenzar por `/` y no puede contener `support`
ni una ruta física.

## Contrato de `ImageUrl`

Para una propiedad cuya fuente declara `1.png`, la URL persistida debe ser:

```text
/assets/properties/1.png
```

Nunca debe ser:

```text
support/seed-data/images/properties/1.png
```

El archivo correspondiente debe existir en `wwwroot/assets/properties/1.png` durante
runtime y publish.

## Contrato de arranque

La secuencia observable debe ser:

```text
crear aplicación
  -> registrar AppDbContext con UseSeeding y UseAsyncSeeding
  -> construir aplicación
  -> await app.MigrateAsync()
  -> app.Run()
```

`MigrationExtensions` no invoca `DatabaseSeeder` directamente. EF Core dispara el
seeding configurado al completar `MigrateAsync`.
