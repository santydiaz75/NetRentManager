using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using NetRentManagerApi.Features.Properties.UpdateProperty;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyNotFoundTests
{
    [Fact]
    public async Task UnknownId_ReturnsNotFoundWithoutWritingFiles()
    {
        var root = Path.Combine(Path.GetTempPath(), "netrentmanager-update-", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        await using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
        var handler = new UpdatePropertyHandler(
            context,
            new TestWebHostEnvironment(root),
            NullLogger<UpdatePropertyHandler>.Instance);

        var result = await handler.HandleAsync(new UpdatePropertyRequest
        {
            Id = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Address = "Address",
            Price = 100,
            Status = "Available",
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 60
        }, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        Assert.False(Directory.Exists(Path.Combine(root, "assets", "properties")));
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
