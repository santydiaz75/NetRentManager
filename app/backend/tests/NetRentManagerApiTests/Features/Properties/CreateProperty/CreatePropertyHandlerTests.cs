using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using NetRentManagerApi.Features.Properties.CreateProperty;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.CreateProperty;

public sealed class CreatePropertyHandlerTests
{
    [Fact]
    public async Task ValidRequestWithoutImage_PersistsNullImageUrl()
    {
        var root = CreateRoot();
        await using var context = CreateContext();
        var handler = CreateHandler(context, root);

        var result = await handler.HandleAsync(ValidRequest(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.ImageUrl);
        Assert.Single(context.Properties);
    }

    [Fact]
    public async Task ValidPng_PersistsPublicUrlAndWritesFile()
    {
        var root = CreateRoot();
        await using var context = CreateContext();
        var handler = CreateHandler(context, root);
        var image = new FormFile(new MemoryStream([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]), 0, 8, "image", "original.PNG")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var result = await handler.HandleAsync(ValidRequest() with { Image = image }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.StartsWith("/assets/properties/", result.Value!.ImageUrl);
        Assert.Single(Directory.GetFiles(Path.Combine(root, "assets", "properties")));
    }

    [Fact]
    public async Task InvalidImageContent_ReturnsValidationFailureWithoutProperty()
    {
        var root = CreateRoot();
        await using var context = CreateContext();
        var handler = CreateHandler(context, root);
        var image = new FormFile(new MemoryStream([0x01, 0x02, 0x03]), 0, 3, "image", "image.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var result = await handler.HandleAsync(ValidRequest() with { Image = image }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        Assert.Empty(context.Properties);
    }

    private static CreatePropertyRequest ValidRequest()
        => new()
        {
            Title = "Title",
            Description = "Description",
            Address = "Address",
            Price = 100,
            Status = "Available",
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 60
        };

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static CreatePropertyHandler CreateHandler(AppDbContext context, string root)
    {
        var environment = new TestWebHostEnvironment(root);
        return new CreatePropertyHandler(context, environment, NullLogger<CreatePropertyHandler>.Instance);
    }

    private static string CreateRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "netrentmanager-create-", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

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
