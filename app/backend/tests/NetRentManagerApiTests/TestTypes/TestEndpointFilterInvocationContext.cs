namespace NetRentManagerApiTests.TestTypes;

public sealed class TestEndpointFilterInvocationContext : EndpointFilterInvocationContext
{
    private readonly IList<object?> _arguments;

    public TestEndpointFilterInvocationContext(HttpContext httpContext, params object?[] arguments)
    {
        HttpContext = httpContext;
        _arguments = arguments;
    }

    public override HttpContext HttpContext { get; }

    public override IList<object?> Arguments => _arguments;

    public override T GetArgument<T>(int index)
    {
        return (T)_arguments[index]!;
    }
}