namespace NetRentManagerApi.Infrastructure.Persistence;

public sealed class SeedAssetSynchronizer(SeedDataReader reader)
{
    private readonly SeedDataReader _reader = reader;

    public IReadOnlyDictionary<string, string> Synchronize(
        SeedManifest manifest,
        IEnumerable<string> imageFileNames,
        string? runtimeBasePath = null)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(imageFileNames);

        var sourceDirectory = _reader.ResolveRuntimePath(manifest.ImagesSourceDirectory, runtimeBasePath);
        var targetDirectory = _reader.ResolveRuntimePath(manifest.ImagesTargetDirectory, runtimeBasePath);

        if (!Directory.Exists(sourceDirectory))
        {
            throw new InvalidOperationException($"No existe el directorio fuente de imágenes: {sourceDirectory}");
        }

        Directory.CreateDirectory(targetDirectory);

        var normalizedBasePath = manifest.PublicImageBasePath.TrimEnd('/');
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var rawFileName in imageFileNames)
        {
            var fileName = NormalizeFileName(rawFileName);
            if (map.ContainsKey(fileName))
            {
                continue;
            }

            var sourcePath = Path.GetFullPath(Path.Combine(sourceDirectory, fileName));
            if (!sourcePath.StartsWith(sourceDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"La imagen '{fileName}' intenta salir del directorio fuente permitido.");
            }

            if (!File.Exists(sourcePath))
            {
                throw new InvalidOperationException($"No existe la imagen declarada en seed: {fileName}");
            }

            var targetPath = Path.Combine(targetDirectory, fileName);
            File.Copy(sourcePath, targetPath, overwrite: true);

            map[fileName] = $"{normalizedBasePath}/{fileName}";
        }

        return map;
    }

    public async Task<IReadOnlyDictionary<string, string>> SynchronizeAsync(
        SeedManifest manifest,
        IEnumerable<string> imageFileNames,
        string? runtimeBasePath = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        ArgumentNullException.ThrowIfNull(imageFileNames);

        var sourceDirectory = _reader.ResolveRuntimePath(manifest.ImagesSourceDirectory, runtimeBasePath);
        var targetDirectory = _reader.ResolveRuntimePath(manifest.ImagesTargetDirectory, runtimeBasePath);

        if (!Directory.Exists(sourceDirectory))
        {
            throw new InvalidOperationException($"No existe el directorio fuente de imágenes: {sourceDirectory}");
        }

        Directory.CreateDirectory(targetDirectory);

        var normalizedBasePath = manifest.PublicImageBasePath.TrimEnd('/');
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var rawFileName in imageFileNames)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fileName = NormalizeFileName(rawFileName);
            if (map.ContainsKey(fileName))
            {
                continue;
            }

            var sourcePath = Path.GetFullPath(Path.Combine(sourceDirectory, fileName));
            if (!sourcePath.StartsWith(sourceDirectory, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"La imagen '{fileName}' intenta salir del directorio fuente permitido.");
            }

            if (!File.Exists(sourcePath))
            {
                throw new InvalidOperationException($"No existe la imagen declarada en seed: {fileName}");
            }

            var targetPath = Path.Combine(targetDirectory, fileName);

            await using var source = File.OpenRead(sourcePath);
            await using var target = File.Create(targetPath);
            await source.CopyToAsync(target, cancellationToken);

            map[fileName] = $"{normalizedBasePath}/{fileName}";
        }

        return map;
    }

    private static string NormalizeFileName(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
        {
            throw new InvalidOperationException("imageUrl no puede estar vacío en properties.json.");
        }

        var fileName = Path.GetFileName(imagePath);
        if (!string.Equals(fileName, imagePath, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"La imagen '{imagePath}' no es válida; debe ser solo nombre de archivo.");
        }

        return fileName;
    }
}