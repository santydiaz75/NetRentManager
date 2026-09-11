namespace NetRentManagerApi.Infrastructure.Endpoints;

public sealed class HealthSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new HealthResponse("healthy", "NetRentManagerApi")))
            .WithName("Health")
            .WithTags("NetRentManagerApi")
            .WithSummary("Comprobar el estado de la API")
            .WithDescription("Devuelve el estado operativo del servicio.");
    }

    private sealed record HealthResponse(string Status, string Service);
}