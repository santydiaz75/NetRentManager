using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class SeedAssetsTests
{
    [Fact]
    public void Synchronize_Copies_Declared_Images_To_Public_Target()
    {
        var runtimeRoot = SeedTestFixture.CreateSeedRoot();
        var reader = new SeedDataReader();
        var manifest = reader.ReadManifest(runtimeRoot);
        var synchronizer = new SeedAssetSynchronizer(reader);

        var map = synchronizer.Synchronize(manifest, new[] { "1.png", "2.png" }, runtimeRoot);

        var targetDirectory = Path.Combine(runtimeRoot, "wwwroot", "assets", "properties");

        Assert.True(File.Exists(Path.Combine(targetDirectory, "1.png")));
        Assert.True(File.Exists(Path.Combine(targetDirectory, "2.png")));
        Assert.Equal("/assets/properties/1.png", map["1.png"]);
        Assert.Equal("/assets/properties/2.png", map["2.png"]);
    }

    [Fact]
    public void ApiProjectFile_Contains_SeedAssets_For_Output_And_Publish()
    {
        var projectPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerApi/NetRentManagerApi.csproj"));

        var source = File.ReadAllText(projectPath);

        Assert.Contains("support/seed-data/properties.json", source, StringComparison.Ordinal);
        Assert.Contains("support/seed-data/properties-statuses.json", source, StringComparison.Ordinal);
        Assert.Contains("support/seed-data/seed-manifest.json", source, StringComparison.Ordinal);
        Assert.Contains("support/seed-data/images/properties", source, StringComparison.Ordinal);
        Assert.Contains("CopyToOutputDirectory", source, StringComparison.Ordinal);
        Assert.Contains("CopyToPublishDirectory", source, StringComparison.Ordinal);
    }

    [Fact]
    public void Repository_Contains_Ten_Seed_Images_In_Source_Directory()
    {
        var repositoryRoot = FindRepositoryRoot();
        var imagesDirectory = Path.Combine(repositoryRoot, "support", "seed-data", "images", "properties");

        Assert.True(Directory.Exists(imagesDirectory));
        Assert.Equal(10, Directory.GetFiles(imagesDirectory, "*.png", SearchOption.TopDirectoryOnly).Length);
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "global.json")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("No se encontró la raíz del repositorio para validar assets.");
    }
}
