using Microsoft.AspNetCore.Mvc;
using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;

namespace NetRentManagerApi.Features.Properties.ListProperties;

public sealed class ListPropertiesSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties", HandleAsync)
            .WithName("ListProperties")
            .WithTags("Properties")
            .WithSummary("Listar propiedades")
            .WithDescription("Devuelve propiedades paginadas con URLs públicas de imagen.")
            .Produces<PagedPropertiesResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters]
        ListPropertiesRequest request,
        [FromServices]
        IEnumerable<IHandler> handlers,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var listPropertiesHandler = handlers.OfType<ListPropertiesHandler>().SingleOrDefault();
        if (listPropertiesHandler is null)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        var result = await listPropertiesHandler.HandleAsync(request, httpRequest, cancellationToken);
        return result.ToIResult(Results.Ok);
    }
}
