using Microsoft.AspNetCore.Mvc;
using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;

namespace NetRentManagerApi.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/properties/{id:guid}", HandleAsync)
            .WithName("GetPropertyById")
            .WithTags("Properties")
            .WithSummary("Consultar una propiedad")
            .WithDescription("Devuelve una propiedad por su identificador.")
            .Produces<GetPropertyByIdResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        [FromServices] IEnumerable<IHandler> handlers,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        var handler = handlers.OfType<GetPropertyByIdHandler>().SingleOrDefault();
        if (handler is null)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        var result = await handler.HandleAsync(
            new GetPropertyByIdRequest(id),
            httpRequest,
            cancellationToken);
        return result.ToIResult(Results.Ok);
    }
}
