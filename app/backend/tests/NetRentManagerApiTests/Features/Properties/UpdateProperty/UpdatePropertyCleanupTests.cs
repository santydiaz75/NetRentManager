using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.UpdateProperty;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyCleanupTests
{
    [Fact]
    public async Task StorageFailure_RestoresPropertyAndReturnsInternalError()
    {
        var root = Path.Combine(Path.GetTempPath(), "netrentmanager-update-", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        await File.WriteAllTextAsync(Path.Combine(root, "assets"), "not a directory");
        await using var context = CreateContext();
        var property = CreateProperty();
        context.Properties.Add(property);
        await context.SaveChangesAsync();
        var handler = new UpdatePropertyHandler(context, new TestWebHostEnvironment(root), NullLogger<UpdatePropertyHandler>.Instance);

        var result = await handler.HandleAsync(ValidRequest(property.Id) with
        {
            Image = CreateFormFile([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "image.png", "image/png")
        }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Internal, result.Error!.Type);
        Assert.Equal("Old title", context.Properties.Single().Title);
        Assert.Null(context.Properties.Single().ImageUrl);
    }

    [Fact]
    public async Task CanceledRequest_PropagatesCancellationBeforeWriting()
    {
        var root = Path.Combine(Path.GetTempPath(), "netrentmanager-update-", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        await using var context = CreateContext();
        var property = CreateProperty();
        context.Properties.Add(property);
        await context.SaveChangesAsync();
        var handler = new UpdatePropertyHandler(context, new TestWebHostEnvironment(root), NullLogger<UpdatePropertyHandler>.Instance);
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.HandleAsync(
            ValidRequest(property.Id) with
            {
                Image = CreateFormFile([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "image.png", "image/png")
            },
            cancellationSource.Token));
        Assert.False(Directory.Exists(Path.Combine(root, "assets", "properties")));
    }

    private static AppDbContext CreateContext()
        => new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Property CreateProperty()
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
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

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
