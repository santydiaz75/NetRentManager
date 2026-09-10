using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Infrastructure.Persistence;

internal static class SeedTestFixture
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string CreateSeedRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "netrentmanager-seed-tests", Guid.NewGuid().ToString("N"));

        var seedDirectory = Path.Combine(root, "support", "seed-data");
        var imagesDirectory = Path.Combine(seedDirectory, "images", "properties");
        var targetImagesDirectory = Path.Combine(root, "wwwroot", "assets", "properties");

        Directory.CreateDirectory(imagesDirectory);
        Directory.CreateDirectory(targetImagesDirectory);

        var manifest = new SeedManifest
        {
            Version = "1.0.0",
            StatusesFile = "support/seed-data/properties-statuses.json",
            PropertiesFile = "support/seed-data/properties.json",
            ImagesSourceDirectory = "support/seed-data/images/properties",
            ImagesTargetDirectory = "wwwroot/assets/properties",
            PublicImageBasePath = "/assets/properties"
        };

        var statuses = new[]
        {
            new SeedStatusItem { Value = nameof(PropertyStatus.Available), Description = "Disponible" },
            new SeedStatusItem { Value = nameof(PropertyStatus.Rented), Description = "Rentada" },
            new SeedStatusItem { Value = nameof(PropertyStatus.Maintenance), Description = "Mantenimiento" }
        };

        var properties = new[]
        {
            new SeedPropertyItem
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                Title = "Casa A",
                Description = "Descripción A",
                Address = "Dirección A",
                Price = 2500m,
                Status = nameof(PropertyStatus.Available),
                BedroomCount = 2,
                BathroomCount = 1,
                AreaSquareMeters = 80m,
                ImageUrl = "1.png",
                CreatedAt = DateTimeOffset.Parse("2026-07-01T00:00:00Z"),
                UpdatedAt = null
            },
            new SeedPropertyItem
            {
                Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                Title = "Casa B",
                Description = "Descripción B",
                Address = "Dirección B",
                Price = 3200m,
                Status = nameof(PropertyStatus.Rented),
                BedroomCount = 3,
                BathroomCount = 2,
                AreaSquareMeters = 100m,
                ImageUrl = "2.png",
                CreatedAt = DateTimeOffset.Parse("2026-07-02T00:00:00Z"),
                UpdatedAt = DateTimeOffset.Parse("2026-07-03T00:00:00Z")
            }
        };

        File.WriteAllText(Path.Combine(seedDirectory, "seed-manifest.json"), JsonSerializer.Serialize(manifest, JsonOptions));
        File.WriteAllText(Path.Combine(seedDirectory, "properties-statuses.json"), JsonSerializer.Serialize(statuses, JsonOptions));
        File.WriteAllText(Path.Combine(seedDirectory, "properties.json"), JsonSerializer.Serialize(properties, JsonOptions));

        File.WriteAllBytes(Path.Combine(imagesDirectory, "1.png"), new byte[] { 1, 2, 3, 4 });
        File.WriteAllBytes(Path.Combine(imagesDirectory, "2.png"), new byte[] { 9, 8, 7, 6 });

        return root;
    }

    public static AppDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new AppDbContext(options);
    }
}
