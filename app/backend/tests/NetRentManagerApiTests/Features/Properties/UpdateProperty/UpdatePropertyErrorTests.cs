using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.UpdateProperty;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyErrorTests
{
    [Fact]
    public async Task EmptyImage_ReturnsUnsupportedMediaTypeAndPreservesProperty()
    {
        var (context, handler, property) = await CreateHandlerAsync();
        var result = await handler.HandleAsync(ValidRequest(property.Id) with
        {
            Image = CreateFormFile([], "image.png", "image/png")
        }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.UnsupportedMediaType, result.Error!.Type);
        Assert.Equal("Old title", context.Properties.Single().Title);
        Assert.Null(context.Properties.Single().ImageUrl);
    }

    [Fact]
    public async Task InvalidImageContent_ReturnsUnsupportedMediaType()
    {
        var (context, handler, property) = await CreateHandlerAsync();
        var result = await handler.HandleAsync(ValidRequest(property.Id) with
        {
            Image = CreateFormFile([0x01, 0x02, 0x03], "image.png", "image/png")
        }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.UnsupportedMediaType, result.Error!.Type);
        Assert.Equal("Old title", context.Properties.Single().Title);
    }

    [Fact]
    public async Task OversizedImage_ReturnsPayloadTooLarge()
    {
        var (context, handler, property) = await CreateHandlerAsync();
        var content = new byte[5 * 1024 * 1024 + 1];
        var result = await handler.HandleAsync(ValidRequest(property.Id) with
        {
            Image = CreateFormFile(content, "image.png", "image/png")
        }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.PayloadTooLarge, result.Error!.Type);
        Assert.Equal("Old title", context.Properties.Single().Title);
    }

    private static async Task<(AppDbContext Context, UpdatePropertyHandler Handler, Property Property)> CreateHandlerAsync()
    {
        var root = Path.Combine(Path.GetTempPath(), "netrentmanager-update-", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = "Old title",
            Description = "Old description",
            Address = "Old address",
            Price = 100,
            Status = PropertyStatus.Available,
            BedroomCount = 1,
            BathroomCount = 1,
            AreaSquareMeters = 40,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };
        context.Properties.Add(property);
        await context.SaveChangesAsync();
        return (context, new UpdatePropertyHandler(context, new TestWebHostEnvironment(root), NullLogger<UpdatePropertyHandler>.Instance), property);
    }

    private static UpdatePropertyRequest ValidRequest(Guid id)
        => new()
        {
            Id = id,
            Title = "New title",
            Description = "New description",
            Address = "New address",
            Price = 200,
            Status = "Rented",
            BedroomCount = 2,
            BathroomCount = 2,
            AreaSquareMeters = 80
        };

    private static IFormFile CreateFormFile(byte[] content, string fileName, string contentType)
        => new FormFile(new MemoryStream(content), 0, content.Length, "image", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

    private sealed class TestWebHostEnvironment(string root) : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = "Testing";
        public string WebRootPath { get; set; } = root;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
    }
}
