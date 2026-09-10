using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class SeedManifestTests
{
    [Fact]
    public void ReadManifest_Loads_And_Validates_Manifest()
    {
        var runtimeRoot = SeedTestFixture.CreateSeedRoot();
        var reader = new SeedDataReader();

        var manifest = reader.ReadManifest(runtimeRoot);

        Assert.Equal("1.0.0", manifest.Version);
        Assert.Equal("support/seed-data/properties-statuses.json", manifest.StatusesFile);
        Assert.Equal("support/seed-data/properties.json", manifest.PropertiesFile);
        Assert.Equal("support/seed-data/images/properties", manifest.ImagesSourceDirectory);
        Assert.Equal("wwwroot/assets/properties", manifest.ImagesTargetDirectory);
        Assert.Equal("/assets/properties", manifest.PublicImageBasePath);
    }

    [Fact]
    public void ValidateManifest_Rejects_Duplicate_Source_Routes()
    {
        var runtimeRoot = SeedTestFixture.CreateSeedRoot();
        var reader = new SeedDataReader();

        var invalidManifest = new SeedManifest
        {
            Version = "1.0.0",
            StatusesFile = "support/seed-data/properties.json",
            PropertiesFile = "support/seed-data/properties.json",
            ImagesSourceDirectory = "support/seed-data/images/properties",
            ImagesTargetDirectory = "wwwroot/assets/properties",
            PublicImageBasePath = "/assets/properties"
        };

        Assert.Throws<InvalidOperationException>(() => reader.ValidateManifest(invalidManifest, runtimeRoot));
    }

    [Fact]
    public void ValidateManifest_Rejects_Invalid_Public_Base_Path()
    {
        var runtimeRoot = SeedTestFixture.CreateSeedRoot();
        var reader = new SeedDataReader();

        var invalidManifest = new SeedManifest
        {
            Version = "1.0.0",
            StatusesFile = "support/seed-data/properties-statuses.json",
            PropertiesFile = "support/seed-data/properties.json",
            ImagesSourceDirectory = "support/seed-data/images/properties",
            ImagesTargetDirectory = "wwwroot/assets/properties",
            PublicImageBasePath = "support/seed-data/images/properties"
        };

        Assert.Throws<InvalidOperationException>(() => reader.ValidateManifest(invalidManifest, runtimeRoot));
    }
}
