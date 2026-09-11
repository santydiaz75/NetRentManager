using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.ListProperties;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApiTests.Features.Properties.ListProperties;

public sealed class ListPropertiesErrorTests
{
    [Theory]
    [InlineData("")]
    [InlineData("C:\\private\\image.png")]
    [InlineData("support/image.png")]
    [InlineData("../image.png")]
    public async Task InvalidImage_ReturnsInternalError(string imageUrl)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AppDbContext(options);
        context.Properties.Add(new Property
        {
            Id = Guid.NewGuid(),
            Title = "Invalid",
            ImageUrl = imageUrl
        });
        await context.SaveChangesAsync();

        var handler = new ListPropertiesHandler(context, LoggerFactory.Create(_ => { }).CreateLogger<ListPropertiesHandler>());
        var request = new DefaultHttpContext().Request;
        request.Scheme = "http";
        request.Host = new HostString("localhost");

        var result = await handler.HandleAsync(new ListPropertiesRequest(), request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Internal, result.Error!.Type);
        var problemDetails = result.Error.ToProblemDetails();
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
    }
}
