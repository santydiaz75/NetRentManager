using NetRentManagerApi.Infrastructure.Errors;

namespace NetRentManagerApiTests.Infrastructure.Errors;

public class ResultProblemDetailsMappingTests
{
    [Theory]
    [InlineData(ErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ErrorType.Validation, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorType.Forbidden, StatusCodes.Status403Forbidden)]
    public void ToProblemDetails_MapsStatusCodes(ErrorType type, int expectedStatusCode)
    {
        var error = new Error("code.sample", "sample message", type);

        var problem = error.ToProblemDetails();

        Assert.Equal(expectedStatusCode, problem.Status);
        Assert.Equal("code.sample", problem.Extensions["code"]);
        Assert.Equal("sample message", problem.Detail);
    }

    [Fact]
    public void ToProblemDetails_WithNullOptionalDetails_ProducesValidProblemDetails()
    {
        var error = Error.NotFound("resource.not_found", "Resource was not found");

        var problem = error.ToProblemDetails();

        Assert.NotNull(problem);
        Assert.Equal(StatusCodes.Status404NotFound, problem.Status);
        Assert.False(problem.Extensions.ContainsKey("details"));
        Assert.DoesNotContain("Exception", problem.Detail ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("StackTrace", problem.Detail ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}