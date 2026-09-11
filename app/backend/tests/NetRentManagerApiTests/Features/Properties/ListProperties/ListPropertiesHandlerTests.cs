using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.ListProperties;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.ListProperties;

public sealed class ListPropertiesHandlerTests
{
    [Fact]
    public async Task DefaultRequest_ReturnsSixItemsInStableOrder()
    {
        await using var context = CreateContext();
        var properties = Enumerable.Range(1, 8)
            .Select(index => new Property
            {
                Id = Guid.Parse($"00000000-0000-0000-0000-{index:000000000000}"),
                Title = index % 2 == 0 ? "Same title" : $"Title {index:00}",
                Description = $"Description {index}",
                Address = $"Address {index}",
                Price = index,
                Status = PropertyStatus.Available,
                BedroomCount = 1,
                BathroomCount = 1,
                AreaSquareMeters = 50,
                ImageUrl = $"/assets/properties/{index}.png"
            });
        context.Properties.AddRange(properties);
        await context.SaveChangesAsync();

        var handler = new ListPropertiesHandler(context, new LoggerFactory().CreateLogger<ListPropertiesHandler>());
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost", 5023);

        var result = await handler.HandleAsync(new ListPropertiesRequest(), httpContext.Request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(6, result.Value!.Items.Count);
        Assert.Equal(8, result.Value.TotalItems);
        Assert.Equal(2, result.Value.TotalPages);
        Assert.All(result.Value.Items, item => Assert.StartsWith("http://localhost:5023/assets/properties/", item.ImageUrl));
        Assert.Equal(result.Value.Items.OrderBy(item => item.Title).ThenBy(item => item.Id), result.Value.Items);
    }

    [Fact]
    public async Task SecondPage_ReturnsConsistentMetadata()
    {
        await using var context = CreateContext();
        context.Properties.AddRange(CreateProperties(7));
        await context.SaveChangesAsync();
        var handler = new ListPropertiesHandler(context, new LoggerFactory().CreateLogger<ListPropertiesHandler>());
        var request = new DefaultHttpContext().Request;
        request.Scheme = "https";
        request.Host = new HostString("example.test");

        var result = await handler.HandleAsync(new ListPropertiesRequest { Page = 2, PageSize = 6 }, request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Items);
        Assert.True(result.Value.HasPrevious);
        Assert.False(result.Value.HasNext);
    }

    [Fact]
    public async Task InvalidImage_ReturnsInternalError()
    {
        await using var context = CreateContext();
        context.Properties.Add(new Property { Id = Guid.NewGuid(), Title = "Invalid", ImageUrl = "support/image.png" });
        await context.SaveChangesAsync();
        var handler = new ListPropertiesHandler(context, new LoggerFactory().CreateLogger<ListPropertiesHandler>());
        var request = new DefaultHttpContext().Request;
        request.Scheme = "http";
        request.Host = new HostString("localhost");

        var result = await handler.HandleAsync(new ListPropertiesRequest(), request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Internal, result.Error!.Type);
    }

    [Fact]
    public async Task CanceledRequest_PropagatesCancellation()
    {
        await using var context = CreateContext();
        context.Properties.AddRange(CreateProperties(1));
        await context.SaveChangesAsync();
        var handler = new ListPropertiesHandler(context, new LoggerFactory().CreateLogger<ListPropertiesHandler>());
        var request = new DefaultHttpContext().Request;
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.HandleAsync(
            new ListPropertiesRequest(), request, cancellationSource.Token));
    }

    [Fact]
    public async Task PropertyWithoutImage_ReturnsItemWithNullImageUrl()
    {
        await using var context = CreateContext();
        context.Properties.Add(new Property
        {
            Id = Guid.NewGuid(),
            Title = "Without image",
            Description = "Description",
            Address = "Address",
            Price = 1,
            Status = PropertyStatus.Available,
            BedroomCount = 1,
            BathroomCount = 1,
            AreaSquareMeters = 20,
            ImageUrl = null
        });
        await context.SaveChangesAsync();

        var handler = new ListPropertiesHandler(context, new LoggerFactory().CreateLogger<ListPropertiesHandler>());
        var request = new DefaultHttpContext().Request;
        request.Scheme = "http";
        request.Host = new HostString("localhost", 5023);

        var result = await handler.HandleAsync(new ListPropertiesRequest(), request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.Items.Single().ImageUrl);
        Assert.Equal("Available", result.Value.Items.Single().Status);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static IEnumerable<Property> CreateProperties(int count)
        => Enumerable.Range(1, count).Select(index => new Property
        {
            Id = Guid.NewGuid(),
            Title = $"Title {index:00}",
            Description = "Description",
            Address = "Address",
            Price = 1,
            Status = PropertyStatus.Available,
            BedroomCount = 1,
            BathroomCount = 1,
            AreaSquareMeters = 50,
            ImageUrl = $"/assets/properties/{index}.png"
        });
}
