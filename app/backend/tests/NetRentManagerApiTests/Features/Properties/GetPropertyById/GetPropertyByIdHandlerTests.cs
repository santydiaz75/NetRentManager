using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.GetPropertyById;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdHandlerTests
{
    [Fact]
    public async Task ExistingProperty_ReturnsExplicitResponse()
    {
        await using var context = CreateContext();
        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Address = "Address",
            Price = 100,
            Status = PropertyStatus.Available,
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 75,
            ImageUrl = "/assets/properties/house.png"
        };
        context.Properties.Add(property);
        await context.SaveChangesAsync();
        var handler = new GetPropertyByIdHandler(context, NullLogger<GetPropertyByIdHandler>.Instance);
        var request = new DefaultHttpContext().Request;
        request.Scheme = "http";
        request.Host = new HostString("localhost", 5023);

        var result = await handler.HandleAsync(new GetPropertyByIdRequest(property.Id), request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(property.Id, result.Value!.Id);
        Assert.Equal("http://localhost:5023/assets/properties/house.png", result.Value.ImageUrl);
    }

    [Fact]
    public async Task MissingProperty_ReturnsNotFound()
    {
        await using var context = CreateContext();
        var handler = new GetPropertyByIdHandler(context, NullLogger<GetPropertyByIdHandler>.Instance);

        var result = await handler.HandleAsync(
            new GetPropertyByIdRequest(Guid.NewGuid()),
            new DefaultHttpContext().Request,
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
    }

    [Fact]
    public async Task CanceledRequest_PropagatesCancellation()
    {
        await using var context = CreateContext();
        var handler = new GetPropertyByIdHandler(context, NullLogger<GetPropertyByIdHandler>.Instance);
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.HandleAsync(
            new GetPropertyByIdRequest(Guid.NewGuid()),
            new DefaultHttpContext().Request,
            source.Token));
    }

    private static AppDbContext CreateContext()
        => new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}
