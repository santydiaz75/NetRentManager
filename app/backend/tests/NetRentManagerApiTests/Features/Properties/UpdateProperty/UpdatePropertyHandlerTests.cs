using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.UpdateProperty;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyHandlerTests
{
    [Fact]
    public async Task ValidPng_ReplacesImageWithUuidUrlAndDeletesPreviousImage()
    {
        var root = CreateRoot();
        var imageDirectory = Path.Combine(root, "assets", "properties");
        Directory.CreateDirectory(imageDirectory);
        var previousPath = Path.Combine(imageDirectory, "old.jpg");
        await File.WriteAllBytesAsync(previousPath, [0x01]);
        await using var context = CreateContext();
        var property = CreateProperty("/assets/properties/old.jpg");
        context.Properties.Add(property);
        await context.SaveChangesAsync();
        var handler = CreateHandler(context, root);
        var image = CreateFormFile(
            [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A],
            "same-name.png",
            "image/png");

        var result = await handler.HandleAsync(ValidRequest(property.Id) with { Image = image }, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Matches("^/assets/properties/[0-9a-f]{32}\\.png$", result.Value!.ImageUrl!);
        Assert.False(File.Exists(previousPath));
        Assert.Single(Directory.GetFiles(imageDirectory));
    }

    [Fact]
    public async Task ValidJpeg_WithRepeatedOriginalName_UsesDifferentUuidFile()
    {
        var root = CreateRoot();
        await using var context = CreateContext();
        var property = CreateProperty(null);
        context.Properties.Add(property);
        await context.SaveChangesAsync();
        var handler = CreateHandler(context, root);
        var firstImage = CreateFormFile([0xFF, 0xD8, 0xFF, 0x00], "same-name.jpeg", "image/jpeg");
        var secondImage = CreateFormFile([0xFF, 0xD8, 0xFF, 0x01], "same-name.jpeg", "image/jpeg");

        var first = await handler.HandleAsync(ValidRequest(property.Id) with { Image = firstImage }, CancellationToken.None);
        var second = await handler.HandleAsync(ValidRequest(property.Id) with { Image = secondImage }, CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.NotEqual(first.Value!.ImageUrl, second.Value!.ImageUrl);
        Assert.Matches("^/assets/properties/[0-9a-f]{32}\\.jpg$", second.Value.ImageUrl!);
    }

    [Fact]
    public async Task ConsecutiveValidUpdates_KeepLastConfirmedValues()
    {
        var root = CreateRoot();
        await using var context = CreateContext();
        var property = CreateProperty(null);
        context.Properties.Add(property);
        await context.SaveChangesAsync();
        var handler = CreateHandler(context, root);

        var first = await handler.HandleAsync(ValidRequest(property.Id) with { Title = "First" }, CancellationToken.None);
        var second = await handler.HandleAsync(ValidRequest(property.Id) with { Title = "Second" }, CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess);
        Assert.Equal("Second", second.Value!.Title);
        Assert.Equal("Second", context.Properties.Single().Title);
    }

    [Fact]
    public async Task ValidRequestWithoutImage_ReplacesFieldsAndPreservesImageUrl()
    {
        var root = CreateRoot();
        await using var context = CreateContext();
        var originalCreatedAt = DateTimeOffset.UtcNow.AddDays(-1);
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
            ImageUrl = "/assets/properties/old.jpg",
            CreatedAt = originalCreatedAt
        };
        context.Properties.Add(property);
        await context.SaveChangesAsync();
        var handler = CreateHandler(context, root);

        var result = await handler.HandleAsync(ValidRequest(property.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("New title", result.Value!.Title);
        Assert.Equal("/assets/properties/old.jpg", result.Value.ImageUrl);
        Assert.Equal(originalCreatedAt, result.Value.CreatedAt);
        Assert.NotNull(result.Value.UpdatedAt);
        Assert.False(Directory.Exists(Path.Combine(root, "assets", "properties")));
    }

    [Fact]
    public async Task ValidRequestForPropertyWithoutImage_PreservesNullImageUrl()
    {
        var root = CreateRoot();
        await using var context = CreateContext();
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
        var handler = CreateHandler(context, root);

        var result = await handler.HandleAsync(ValidRequest(property.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.ImageUrl);
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

    private static Property CreateProperty(string? imageUrl)
        => new()
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
            ImageUrl = imageUrl,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

    private static IFormFile CreateFormFile(byte[] content, string fileName, string contentType)
        => new FormFile(new MemoryStream(content), 0, content.Length, "image", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static UpdatePropertyHandler CreateHandler(AppDbContext context, string root)
        => new(context, new TestWebHostEnvironment(root), NullLogger<UpdatePropertyHandler>.Instance);

    private static string CreateRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "netrentmanager-update-", Guid.NewGuid().ToString("N"));
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
