# Modelo de Datos: Propiedades y Estados

## `PropertyStatus`

- **Tipo**: enum cerrado de dominio.
- **Valores**: `Available`, `Rented`, `Maintenance`.
- **Catálogo persistente**: tabla `property_statuses`, con clave textual única y descripción de seed.
- **Conversión**: `Property.Status` se almacena como texto mediante `HasConversion<string>()`.

## `Property`

- **Tabla**: `properties`.
- **Identidad**: `Guid Id` como clave primaria.
- **Título**: string requerido, longitud máxima `200`.
- **Descripción**: string requerido, longitud máxima `2000`.
- **Dirección**: string requerido, longitud máxima `300`.
- **Precio**: `decimal`, requerido, precisión `18,2`, no negativo.
- **Estado**: `PropertyStatus`, requerido, almacenado como texto.
- **Habitaciones**: `int BedroomCount`, requerido, no negativo.
- **Baños**: `int BathroomCount`, requerido, no negativo.
- **Área**: `decimal AreaSquareMeters`, requerido, precisión `10,2`, positivo.
- **Imagen**: string requerido, longitud máxima `500`, ruta pública relativa a `/assets/properties`.
- **Creación**: `DateTimeOffset CreatedAt`, requerido, default UTC.
- **Actualización**: `DateTimeOffset? UpdatedAt`, opcional; permanece nulo para registros nunca actualizados.

## Reglas de persistencia

- La configuración de `Property` vive en `PropertyConfiguration`.
- La configuración del catálogo vive en `PropertyStatusConfiguration` si el catálogo se modela como entidad persistente.
- `AppDbContext` expone `DbSet<Property>` y el conjunto de estados requerido por el seeder.
- `OnModelCreating` solo llama `ApplyConfigurationsFromAssembly`.
- Se crea un índice único sobre el valor textual de estado.
- El identificador de propiedad es la clave de idempotencia del seeder.

## `SeedManifest`

El manifiesto existente contiene:

```json
{
  "version": "1.0.0",
  "statusesFile": "support/seed-data/properties-statuses.json",
  "propertiesFile": "support/seed-data/properties.json",
  "imagesSourceDirectory": "support/seed-data/images/properties",
  "imagesTargetDirectory": "wwwroot/assets/properties",
  "publicImageBasePath": "/assets/properties"
}
```

El seeder debe validar que todas las rutas fuente existan, que las imágenes referidas
por las propiedades estén dentro del directorio fuente y que la URL persistida se
construya exclusivamente desde `publicImageBasePath` y el nombre de archivo.

## Transiciones del seeder

1. Leer y validar manifiesto.
2. Leer estados y verificar el conjunto cerrado.
3. Leer propiedades y validar identificadores, estados, valores e imágenes.
4. Crear o verificar el destino público de imágenes.
5. Copiar imágenes sin introducir rutas físicas en `ImageUrl`.
6. Insertar estados faltantes.
7. Insertar propiedades faltantes por `Id`.
8. Guardar de forma síncrona o asíncrona según la entrada EF Core.

Una entrada inválida detiene la operación antes de persistir una referencia
inconsistente.
