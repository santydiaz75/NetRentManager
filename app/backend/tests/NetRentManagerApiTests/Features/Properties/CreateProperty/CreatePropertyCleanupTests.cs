using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using NetRentManagerApi.Features.Properties.CreateProperty;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.CreateProperty;

public sealed class CreatePropertyCleanupTests
{
    [Fact]
    public async Task StorageFailure_ReturnsInternalErrorWithoutProperty()
    {
        var rootFile = Path.Combine(Path.GetTempPath(), $"netrentmanager-file-{Guid.NewGuid():N}");
        await File.WriteAllTextAsync(rootFile, "not a directory");
        await using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        var handler = new CreatePropertyHandler(
            context,
            new TestWebHostEnvironment(rootFile),
            NullLogger<CreatePropertyHandler>.Instance);
        var image = new FormFile(new MemoryStream([0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]), 0, 8, "image", "image.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        var result = await handler.HandleAsync(ValidRequest() with { Image = image }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Internal, result.Error!.Type);
        Assert.Empty(context.Properties);
        File.Delete(rootFile);
    }

    [Fact]
    public async Task CanceledRequest_PropagatesCancellation()
    {
        var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        await using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        var handler = new CreatePropertyHandler(context, new TestWebHostEnvironment(root), NullLogger<CreatePropertyHandler>.Instance);
        using var source = new CancellationTokenSource();
        source.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => handler.HandleAsync(ValidRequest(), source.Token));
    }

    private static CreatePropertyRequest ValidRequest()
        => new()
        {
            Title = "Title",
            Description = "Description",
            Address = "Address",
            Price = 100,
            Status = "Available",
            BedroomCount = 1,
            BathroomCount = 1,
            AreaSquareMeters = 40
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
