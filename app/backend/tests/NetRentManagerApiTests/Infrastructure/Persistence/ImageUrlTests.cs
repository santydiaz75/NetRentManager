using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class ImageUrlTests
{
    [Fact]
    public async Task Seeded_Properties_Use_Public_ImageUrl_And_Files_Exist()
    {
        var runtimeRoot = SeedTestFixture.CreateSeedRoot();

        await using var context = SeedTestFixture.CreateContext($"image-url-{Guid.NewGuid():N}");
        await context.Database.EnsureCreatedAsync();

        await DatabaseSeeder.SeedAsync(context, runtimeBasePath: runtimeRoot);

        var properties = await context.Properties.OrderBy(property => property.Id).ToListAsync();
        Assert.NotEmpty(properties);

        var targetDirectory = Path.Combine(runtimeRoot, "wwwroot", "assets", "properties");

        foreach (var property in properties)
        {
            Assert.StartsWith("/assets/properties/", property.ImageUrl, StringComparison.Ordinal);
            Assert.DoesNotContain("support", property.ImageUrl, StringComparison.OrdinalIgnoreCase);

            var fileName = Path.GetFileName(property.ImageUrl);
            Assert.True(File.Exists(Path.Combine(targetDirectory, fileName)));
        }
    }
}
