using NetRentManagerApi.Infrastructure.Errors;

namespace NetRentManagerApiTests.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdErrorTests
{
    [Fact]
    public void NotFoundError_MapsToProblemDetails404()
    {
        var problemDetails = Error.NotFound("properties.not_found", "La propiedad no existe.").ToProblemDetails();

        Assert.Equal(StatusCodes.Status404NotFound, problemDetails.Status);
        Assert.Equal("properties.not_found", problemDetails.Extensions["code"]);
    }

    [Fact]
    public void InvalidImageError_MapsToProblemDetails500WithoutPath()
    {
        var problemDetails = Error.Internal(
            "properties.image.invalid",
            "La propiedad no tiene una imagen pública válida.").ToProblemDetails();

        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.DoesNotContain("support", problemDetails.Detail, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("C:\\", problemDetails.Detail, StringComparison.OrdinalIgnoreCase);
    }
}