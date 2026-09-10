using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static void Seed(AppDbContext context, string? runtimeBasePath = null)
    {
        ArgumentNullException.ThrowIfNull(context);

        var reader = new SeedDataReader();
        var manifest = reader.ReadManifest(runtimeBasePath);
        var statuses = reader.ReadStatuses(manifest, runtimeBasePath);
        var properties = reader.ReadProperties(manifest, runtimeBasePath);

        ValidateStatusSet(statuses);

        var synchronizer = new SeedAssetSynchronizer(reader);
        var publicUrlsByFile = synchronizer.Synchronize(
            manifest,
            properties.Select(property => property.ImageUrl),
            runtimeBasePath);

        SeedStatuses(context, statuses);
        SeedProperties(context, properties, publicUrlsByFile);

        if (context.ChangeTracker.HasChanges())
        {
            context.SaveChanges();
        }
    }

    public static async Task SeedAsync(
        AppDbContext context,
        CancellationToken cancellationToken = default,
        string? runtimeBasePath = null)
    {
        ArgumentNullException.ThrowIfNull(context);

        var reader = new SeedDataReader();
        var manifest = await reader.ReadManifestAsync(runtimeBasePath, cancellationToken);
        var statuses = await reader.ReadStatusesAsync(manifest, runtimeBasePath, cancellationToken);
        var properties = await reader.ReadPropertiesAsync(manifest, runtimeBasePath, cancellationToken);

        ValidateStatusSet(statuses);

        var synchronizer = new SeedAssetSynchronizer(reader);
        var publicUrlsByFile = await synchronizer.SynchronizeAsync(
            manifest,
            properties.Select(property => property.ImageUrl),
            runtimeBasePath,
            cancellationToken);

        await SeedStatusesAsync(context, statuses, cancellationToken);
        await SeedPropertiesAsync(context, properties, publicUrlsByFile, cancellationToken);

        if (context.ChangeTracker.HasChanges())
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static void SeedStatuses(AppDbContext context, IReadOnlyCollection<SeedStatusItem> statuses)
    {
        var existingStatuses = context.PropertyStatuses
            .Select(status => status.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var status in statuses)
        {
            if (existingStatuses.Contains(status.Value))
            {
                continue;
            }

            context.PropertyStatuses.Add(new PropertyStatusCatalogEntry
            {
                Value = status.Value,
                Description = status.Description
            });
        }
    }

    private static async Task SeedStatusesAsync(
        AppDbContext context,
        IReadOnlyCollection<SeedStatusItem> statuses,
        CancellationToken cancellationToken)
    {
        var existingStatuses = (await context.PropertyStatuses
                .Select(status => status.Value)
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var status in statuses)
        {
            if (existingStatuses.Contains(status.Value))
            {
                continue;
            }

            context.PropertyStatuses.Add(new PropertyStatusCatalogEntry
            {
                Value = status.Value,
                Description = status.Description
            });
        }
    }

    private static void SeedProperties(
        AppDbContext context,
        IReadOnlyCollection<SeedPropertyItem> properties,
        IReadOnlyDictionary<string, string> publicUrlsByFile)
    {
        var existingPropertyIds = context.Properties
            .Select(property => property.Id)
            .ToHashSet();

        foreach (var sourceProperty in properties)
        {
            if (existingPropertyIds.Contains(sourceProperty.Id))
            {
                continue;
            }

            if (!Enum.TryParse<PropertyStatus>(sourceProperty.Status, ignoreCase: false, out var parsedStatus))
            {
                throw new InvalidOperationException($"Estado inválido en propiedad {sourceProperty.Id}: {sourceProperty.Status}");
            }

            var imageFile = Path.GetFileName(sourceProperty.ImageUrl);
            if (!publicUrlsByFile.TryGetValue(imageFile, out var publicUrl))
            {
                throw new InvalidOperationException($"No se encontró URL pública para la imagen '{sourceProperty.ImageUrl}'.");
            }

            if (publicUrl.Contains("support", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("ImageUrl no puede contener rutas físicas bajo support.");
            }

            context.Properties.Add(new Property
            {
                Id = sourceProperty.Id,
                Title = sourceProperty.Title,
                Description = sourceProperty.Description,
                Address = sourceProperty.Address,
                Price = sourceProperty.Price,
                Status = parsedStatus,
                BedroomCount = sourceProperty.BedroomCount,
                BathroomCount = sourceProperty.BathroomCount,
                AreaSquareMeters = sourceProperty.AreaSquareMeters,
                ImageUrl = publicUrl,
                CreatedAt = sourceProperty.CreatedAt,
                UpdatedAt = sourceProperty.UpdatedAt
            });
        }
    }

    private static async Task SeedPropertiesAsync(
        AppDbContext context,
        IReadOnlyCollection<SeedPropertyItem> properties,
        IReadOnlyDictionary<string, string> publicUrlsByFile,
        CancellationToken cancellationToken)
    {
        var existingPropertyIds = (await context.Properties
                .Select(property => property.Id)
                .ToListAsync(cancellationToken))
            .ToHashSet();

        foreach (var sourceProperty in properties)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (existingPropertyIds.Contains(sourceProperty.Id))
            {
                continue;
            }

            if (!Enum.TryParse<PropertyStatus>(sourceProperty.Status, ignoreCase: false, out var parsedStatus))
            {
                throw new InvalidOperationException($"Estado inválido en propiedad {sourceProperty.Id}: {sourceProperty.Status}");
            }

            var imageFile = Path.GetFileName(sourceProperty.ImageUrl);
            if (!publicUrlsByFile.TryGetValue(imageFile, out var publicUrl))
            {
                throw new InvalidOperationException($"No se encontró URL pública para la imagen '{sourceProperty.ImageUrl}'.");
            }

            if (publicUrl.Contains("support", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("ImageUrl no puede contener rutas físicas bajo support.");
            }

            context.Properties.Add(new Property
            {
                Id = sourceProperty.Id,
                Title = sourceProperty.Title,
                Description = sourceProperty.Description,
                Address = sourceProperty.Address,
                Price = sourceProperty.Price,
                Status = parsedStatus,
                BedroomCount = sourceProperty.BedroomCount,
                BathroomCount = sourceProperty.BathroomCount,
                AreaSquareMeters = sourceProperty.AreaSquareMeters,
                ImageUrl = publicUrl,
                CreatedAt = sourceProperty.CreatedAt,
                UpdatedAt = sourceProperty.UpdatedAt
            });
        }
    }

    private static void ValidateStatusSet(IEnumerable<SeedStatusItem> statuses)
    {
        var definedStatuses = Enum.GetNames<PropertyStatus>().ToHashSet(StringComparer.Ordinal);
        var seedStatuses = statuses.Select(status => status.Value).ToHashSet(StringComparer.Ordinal);

        if (!definedStatuses.SetEquals(seedStatuses))
        {
            throw new InvalidOperationException(
                "El catálogo de estados no coincide con PropertyStatus (Available, Rented, Maintenance).");
        }
    }
}