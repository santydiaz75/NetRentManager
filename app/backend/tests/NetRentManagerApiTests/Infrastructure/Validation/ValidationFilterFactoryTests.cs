using FluentValidation;
using NetRentManagerApi.Infrastructure.Validation;
using NetRentManagerApiTests.TestTypes;

namespace NetRentManagerApiTests.Infrastructure.Validation;

public class ValidationFilterFactoryTests
{
    [Fact]
    public async Task CreateForMethod_WithValidator_ExecutesValidationAndPassesThroughWhenValid()
    {
        var validator = new SpyValidatedRequestValidator();
        var services = new ServiceCollection();
        services.AddSingleton<IValidator<ValidatedRequest>>(validator);
        using var provider = services.BuildServiceProvider();

        var executed = false;
        EndpointFilterDelegate next = _ =>
        {
            executed = true;
            return ValueTask.FromResult<object?>(Results.Ok());
        };

        var method = typeof(TestEndpointMethods).GetMethod(nameof(TestEndpointMethods.ValidatedEndpoint))!;
        var filter = ValidationFilterFactory.CreateForMethod(method, next);
        using var cts = new CancellationTokenSource();
        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider,
            RequestAborted = cts.Token
        };
        var invocationContext = new TestEndpointFilterInvocationContext(
            httpContext,
            new ValidatedRequest("ok"),
            cts.Token);

        _ = await filter(invocationContext);

        Assert.True(executed);
        Assert.Equal(cts.Token, validator.LastToken);
    }

    [Fact]
    public async Task CreateForMethod_WithoutValidator_PassesThrough()
    {
        var services = new ServiceCollection();
        using var provider = services.BuildServiceProvider();

        var executed = false;
        EndpointFilterDelegate next = _ =>
        {
            executed = true;
            return ValueTask.FromResult<object?>(Results.Ok());
        };

        var method = typeof(TestEndpointMethods).GetMethod(nameof(TestEndpointMethods.NoValidatorEndpoint))!;
        var filter = ValidationFilterFactory.CreateForMethod(method, next);
        var httpContext = new DefaultHttpContext { RequestServices = provider };
        var invocationContext = new TestEndpointFilterInvocationContext(httpContext, new NoValidatorRequest("ok"));

        _ = await filter(invocationContext);

        Assert.True(executed);
    }

    [Fact]
    public async Task CreateForMethod_WhenValidationFails_ReturnsProblemAndSkipsNext()
    {
        var validator = new SpyValidatedRequestValidator();
        var services = new ServiceCollection();
        services.AddSingleton<IValidator<ValidatedRequest>>(validator);
        using var provider = services.BuildServiceProvider();

        var executed = false;
        EndpointFilterDelegate next = _ =>
        {
            executed = true;
            return ValueTask.FromResult<object?>(Results.Ok());
        };

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

        Assert.False(executed);
        Assert.IsAssignableFrom<IResult>(result);
    }
}