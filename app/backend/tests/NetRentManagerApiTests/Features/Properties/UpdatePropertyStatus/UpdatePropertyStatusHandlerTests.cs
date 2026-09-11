using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.UpdatePropertyStatus;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusHandlerTests
{
    [Fact]
    public async Task Updates_only_status_and_preserves_other_fields()
    {
        await using var context = CreateContext();
        var property = CreateProperty(PropertyStatus.Available);
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var handler = new UpdatePropertyStatusHandler(context, NullLogger<UpdatePropertyStatusHandler>.Instance);
        var result = await handler.HandleAsync(
            property.Id,
            new UpdatePropertyStatusRequest { Status = "rented" },
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Rented", result.Value!.Status);
        Assert.Equal("Apartamento", result.Value.Title);
        Assert.Equal("/assets/properties/original.jpg", result.Value.ImageUrl);
    }

    [Fact]
    public async Task Returns_not_found_for_unknown_property()
    {
        await using var context = CreateContext();
        var handler = new UpdatePropertyStatusHandler(context, NullLogger<UpdatePropertyStatusHandler>.Instance);

        var result = await handler.HandleAsync(
            Guid.NewGuid(),
            new UpdatePropertyStatusRequest { Status = "Available" },
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("properties.not_found", result.Error!.Code);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static Property CreateProperty(PropertyStatus status)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = "Apartamento",
            Description = "Descripción",
            Address = "Calle Principal 10",
            Price = 1200,
            Status = status,
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 70,
            ImageUrl = "/assets/properties/original.jpg",
            CreatedAt = DateTimeOffset.UtcNow
        };
}
