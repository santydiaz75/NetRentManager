using System.Text.Json;

namespace NetRentManagerApi.Infrastructure.Persistence;

public sealed class SeedDataReader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string ResolveRuntimePath(string relativePath, string? runtimeBasePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        var rootPath = runtimeBasePath ?? AppContext.BaseDirectory;
        var normalized = relativePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        return Path.GetFullPath(Path.Combine(rootPath, normalized));
    }

    public SeedManifest ReadManifest(string? runtimeBasePath = null)
    {
        var manifestPath = ResolveRuntimePath("support/seed-data/seed-manifest.json", runtimeBasePath);
        EnsureFileExists(manifestPath);

        var manifest = JsonSerializer.Deserialize<SeedManifest>(File.ReadAllText(manifestPath), JsonOptions)
            ?? throw new InvalidOperationException("No se pudo deserializar seed-manifest.json.");

        ValidateManifest(manifest, runtimeBasePath);
        return manifest;
    }

    public async Task<SeedManifest> ReadManifestAsync(string? runtimeBasePath = null, CancellationToken cancellationToken = default)
    {
        var manifestPath = ResolveRuntimePath("support/seed-data/seed-manifest.json", runtimeBasePath);
        EnsureFileExists(manifestPath);

        var json = await File.ReadAllTextAsync(manifestPath, cancellationToken);
        var manifest = JsonSerializer.Deserialize<SeedManifest>(json, JsonOptions)
            ?? throw new InvalidOperationException("No se pudo deserializar seed-manifest.json.");

        ValidateManifest(manifest, runtimeBasePath);
        return manifest;
    }

    public IReadOnlyList<SeedStatusItem> ReadStatuses(SeedManifest manifest, string? runtimeBasePath = null)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var statusesPath = ResolveRuntimePath(manifest.StatusesFile, runtimeBasePath);
        EnsureFileExists(statusesPath);

        var statuses = JsonSerializer.Deserialize<List<SeedStatusItem>>(File.ReadAllText(statusesPath), JsonOptions)
            ?? throw new InvalidOperationException("No se pudo deserializar properties-statuses.json.");

        ValidateStatuses(statuses);
        return statuses;
    }

    public async Task<IReadOnlyList<SeedStatusItem>> ReadStatusesAsync(
        SeedManifest manifest,
        string? runtimeBasePath = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var statusesPath = ResolveRuntimePath(manifest.StatusesFile, runtimeBasePath);
        EnsureFileExists(statusesPath);

        var json = await File.ReadAllTextAsync(statusesPath, cancellationToken);
        var statuses = JsonSerializer.Deserialize<List<SeedStatusItem>>(json, JsonOptions)
            ?? throw new InvalidOperationException("No se pudo deserializar properties-statuses.json.");

        ValidateStatuses(statuses);
        return statuses;
    }

    public IReadOnlyList<SeedPropertyItem> ReadProperties(SeedManifest manifest, string? runtimeBasePath = null)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var propertiesPath = ResolveRuntimePath(manifest.PropertiesFile, runtimeBasePath);
        EnsureFileExists(propertiesPath);

        var properties = JsonSerializer.Deserialize<List<SeedPropertyItem>>(File.ReadAllText(propertiesPath), JsonOptions)
            ?? throw new InvalidOperationException("No se pudo deserializar properties.json.");

        ValidateProperties(properties);
        return properties;
    }

    public async Task<IReadOnlyList<SeedPropertyItem>> ReadPropertiesAsync(
        SeedManifest manifest,
        string? runtimeBasePath = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        var propertiesPath = ResolveRuntimePath(manifest.PropertiesFile, runtimeBasePath);
        EnsureFileExists(propertiesPath);

        var json = await File.ReadAllTextAsync(propertiesPath, cancellationToken);
        var properties = JsonSerializer.Deserialize<List<SeedPropertyItem>>(json, JsonOptions)
            ?? throw new InvalidOperationException("No se pudo deserializar properties.json.");

        ValidateProperties(properties);
        return properties;
    }

    public void ValidateManifest(SeedManifest manifest, string? runtimeBasePath = null)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        if (string.IsNullOrWhiteSpace(manifest.Version)
            || string.IsNullOrWhiteSpace(manifest.StatusesFile)
            || string.IsNullOrWhiteSpace(manifest.PropertiesFile)
            || string.IsNullOrWhiteSpace(manifest.ImagesSourceDirectory)
            || string.IsNullOrWhiteSpace(manifest.ImagesTargetDirectory)
            || string.IsNullOrWhiteSpace(manifest.PublicImageBasePath))
        {
            throw new InvalidOperationException("El manifiesto de seed no contiene todos los campos requeridos.");
        }

        if (!manifest.PublicImageBasePath.StartsWith("/", StringComparison.Ordinal)
            || manifest.PublicImageBasePath.Contains("support", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("publicImageBasePath debe ser público y no puede apuntar a rutas físicas de support.");
        }

        var uniquePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            manifest.StatusesFile,
            manifest.PropertiesFile,
            manifest.ImagesSourceDirectory
        };

        if (uniquePaths.Count != 3)
        {
            throw new InvalidOperationException("El manifiesto contiene rutas duplicadas en las fuentes de datos.");
        }

        EnsureFileExists(ResolveRuntimePath(manifest.StatusesFile, runtimeBasePath));
        EnsureFileExists(ResolveRuntimePath(manifest.PropertiesFile, runtimeBasePath));

        var imagesSourcePath = ResolveRuntimePath(manifest.ImagesSourceDirectory, runtimeBasePath);
        if (!Directory.Exists(imagesSourcePath))
        {
            throw new InvalidOperationException($"No existe el directorio de imágenes: {imagesSourcePath}");
        }
    }

    public void ValidateStatuses(IReadOnlyCollection<SeedStatusItem> statuses)
    {
        if (statuses.Count == 0)
        {
            throw new InvalidOperationException("El catálogo de estados no contiene elementos.");
        }

        var duplicated = statuses
            .GroupBy(status => status.Value, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(duplicated))
        {
            throw new InvalidOperationException($"Estado duplicado en catálogo: {duplicated}");
        }

        if (statuses.Any(status => string.IsNullOrWhiteSpace(status.Value) || string.IsNullOrWhiteSpace(status.Description)))
        {
            throw new InvalidOperationException("Todos los estados deben incluir value y description.");
        }
    }

    public void ValidateProperties(IReadOnlyCollection<SeedPropertyItem> properties)
    {
        if (properties.Count == 0)
        {
            throw new InvalidOperationException("El listado de propiedades semilla no contiene elementos.");
        }

        var duplicated = properties
            .GroupBy(property => property.Id)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .FirstOrDefault();

        if (duplicated != Guid.Empty)
        {
            throw new InvalidOperationException($"La propiedad {duplicated} aparece duplicada en properties.json.");
        }

        if (properties.Any(property =>
                string.IsNullOrWhiteSpace(property.Title)
                || string.IsNullOrWhiteSpace(property.Description)
                || string.IsNullOrWhiteSpace(property.Address)
                || string.IsNullOrWhiteSpace(property.Status)
                || string.IsNullOrWhiteSpace(property.ImageUrl)))
        {
            throw new InvalidOperationException("Todas las propiedades deben contener title, description, address, status e imageUrl.");
        }
    }

    private static void EnsureFileExists(string path)
    {
        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"No existe el archivo requerido de seed: {path}");
        }
    }
}