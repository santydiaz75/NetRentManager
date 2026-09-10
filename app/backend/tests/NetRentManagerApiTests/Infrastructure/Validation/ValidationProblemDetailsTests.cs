using System.Text.Json;
using FluentValidation;
using NetRentManagerApi.Infrastructure.Validation;
using NetRentManagerApiTests.TestTypes;

namespace NetRentManagerApiTests.Infrastructure.Validation;

public class ValidationProblemDetailsTests
{
    [Fact]
    public async Task InvalidValidationResult_Returns400ProblemDetails_WithGroupedErrors()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IValidator<ValidatedRequest>, SpyValidatedRequestValidator>();
        using var provider = services.BuildServiceProvider();

        EndpointFilterDelegate next = _ => ValueTask.FromResult<object?>(Results.Ok());
        var method = typeof(TestEndpointMethods).GetMethod(nameof(TestEndpointMethods.ValidatedEndpoint))!;
        var filter = ValidationFilterFactory.CreateForMethod(method, next);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider,
            Response = { Body = new MemoryStream() }
        };

        var invocationContext = new TestEndpointFilterInvocationContext(
            httpContext,
            new ValidatedRequest(string.Empty),
            CancellationToken.None);

        var result = await filter(invocationContext);
        var iResult = Assert.IsAssignableFrom<IResult>(result);
        await iResult.ExecuteAsync(httpContext);

        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.Contains("application/problem+json", httpContext.Response.ContentType ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        httpContext.Response.Body.Position = 0;
        var json = await JsonDocument.ParseAsync(httpContext.Response.Body);
        var errors = json.RootElement.GetProperty("errors");
        Assert.True(errors.TryGetProperty("Name", out var nameErrors));
        var messages = nameErrors.EnumerateArray().Select(x => x.GetString()).OfType<string>().ToArray();
        Assert.Contains("Name is required", messages);
        Assert.Contains("Name cannot be empty", messages);
    }
}