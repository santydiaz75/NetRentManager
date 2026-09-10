namespace NetRentManagerApi.Infrastructure.Endpoints;

public sealed class HealthSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new HealthResponse("healthy", "NetRentManagerApi")));
    }

    private sealed record HealthResponse(string Status, string Service);
}