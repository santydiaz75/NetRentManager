using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Infrastructure.Persistence;

public sealed class DatabaseSeederTests
{
    [Fact]
    public void Seed_Is_Idempotent_For_Statuses_And_Properties()
    {
        var runtimeRoot = SeedTestFixture.CreateSeedRoot();
        using var context = SeedTestFixture.CreateContext($"seed-sync-{Guid.NewGuid():N}");

        context.Database.EnsureCreated();

        DatabaseSeeder.Seed(context, runtimeRoot);
        DatabaseSeeder.Seed(context, runtimeRoot);

        Assert.Equal(3, context.PropertyStatuses.Count());
        Assert.Equal(2, context.Properties.Count());
        Assert.All(context.Properties, property =>
        {
            Assert.StartsWith("/assets/properties/", property.ImageUrl, StringComparison.Ordinal);
            Assert.DoesNotContain("support", property.ImageUrl, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Fact]
    public async Task SeedAsync_Is_Idempotent_For_Statuses_And_Properties()
    {
        var runtimeRoot = SeedTestFixture.CreateSeedRoot();
        await using var context = SeedTestFixture.CreateContext($"seed-async-{Guid.NewGuid():N}");

        await context.Database.EnsureCreatedAsync();

        await DatabaseSeeder.SeedAsync(context, runtimeBasePath: runtimeRoot);
        await DatabaseSeeder.SeedAsync(context, runtimeBasePath: runtimeRoot);

        Assert.Equal(3, await context.PropertyStatuses.CountAsync());
        Assert.Equal(2, await context.Properties.CountAsync());
    }

    [Fact]
    public async Task Seed_And_SeedAsync_Produce_Equivalent_Results()
    {
        var runtimeRoot = SeedTestFixture.CreateSeedRoot();

        await using var syncContext = SeedTestFixture.CreateContext($"seed-eq-sync-{Guid.NewGuid():N}");
        syncContext.Database.EnsureCreated();
        DatabaseSeeder.Seed(syncContext, runtimeRoot);

        await using var asyncContext = SeedTestFixture.CreateContext($"seed-eq-async-{Guid.NewGuid():N}");
        await asyncContext.Database.EnsureCreatedAsync();
        await DatabaseSeeder.SeedAsync(asyncContext, runtimeBasePath: runtimeRoot);

        var syncStatuses = syncContext.PropertyStatuses.OrderBy(x => x.Value).Select(x => x.Value).ToList();
        var asyncStatuses = await asyncContext.PropertyStatuses.OrderBy(x => x.Value).Select(x => x.Value).ToListAsync();
        Assert.Equal(syncStatuses, asyncStatuses);

        var syncProperties = syncContext.Properties.OrderBy(x => x.Id).Select(x => new { x.Id, x.ImageUrl }).ToList();
        var asyncProperties = await asyncContext.Properties.OrderBy(x => x.Id).Select(x => new { x.Id, x.ImageUrl }).ToListAsync();
        Assert.Equal(syncProperties, asyncProperties);
    }
}
