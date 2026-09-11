using NetRentManagerApi.Infrastructure.Errors;

namespace NetRentManagerApiTests.Infrastructure.Errors;

public sealed class ResultProblemDetailsMapperTests
{
    [Fact]
    public void InternalError_MapsToProblemDetails500()
    {
        var problemDetails = Error.Internal("properties.image.invalid", "invalid image").ToProblemDetails();

        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("properties.image.invalid", problemDetails.Extensions["code"]);
    }

    [Fact]
    public void ValidationError_StillMapsTo400()
    {
        var problemDetails = Error.Validation(
            "request.invalid",
            "invalid request",
            new Dictionary<string, string[]>()).ToProblemDetails();

        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
    }
}
