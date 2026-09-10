using NetRentManagerApi.Infrastructure.Endpoints;

namespace NetRentManagerApiTests.TestTypes;

public sealed class TestSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/test-slice", () => Results.Ok(new { status = "ok" }));
    }
}