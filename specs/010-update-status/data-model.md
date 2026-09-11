# Modelo de Datos: Actualización de Estado de Propiedad

Esta iniciativa no introduce entidades, columnas ni migraciones. Usa el modelo persistente existente.

## PropertyStatus

- **Tipo**: enum cerrado de dominio convertido a texto en PostgreSQL.
- **Valores canónicos**: `Available`, `Rented`, `Maintenance`.
- **Entrada**: comparación sin distinguir mayúsculas/minúsculas.
- **Persistencia**: siempre se guarda el valor canónico.

## UpdatePropertyStatusRequest

- **Campo**: `status`.
- **Tipo conceptual**: texto recibido en JSON y convertido/validado contra `PropertyStatus`.
- **Obligatorio**: sí.
- **Propiedades adicionales**: rechazadas con HTTP 400.
- **Archivos**: no admite `image` ni multipart.

## Property

- **Identidad**: `Guid Id`.
- **Campo mutable en esta iniciativa**: `Status`.
- **Campos conservados**: `Title`, `Description`, `Address`, `Price`, `BedroomCount`, `BathroomCount`, `AreaSquareMeters`, `ImageUrl`, `CreatedAt` y cualquier otro campo persistido no incluido en el caso de uso.

## UpdatePropertyStatusResponse

- **Contenido**: contrato explícito de propiedad completa.
- **Campos mínimos**: `id`, `title`, `description`, `address`, `price`, `status`, `bedroomCount`, `bathroomCount`, `areaSquareMeters`, `imageUrl`.
- **Regla**: refleja el estado canónico guardado y el resto de valores después de la actualización.

## Relaciones y flujo

```text
UpdatePropertyStatusRequest 1 -> 1 Property (solo Status mutable)
Property 1 -> 1 UpdatePropertyStatusResponse
```
