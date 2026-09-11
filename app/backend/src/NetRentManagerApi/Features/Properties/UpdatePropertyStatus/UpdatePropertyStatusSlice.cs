using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;

namespace NetRentManagerApi.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusSlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/properties/{id:guid}/status", HandleAsync)
            .WithName("UpdatePropertyStatus")
            .WithTags("Properties")
            .WithSummary("Actualizar el estado de una propiedad")
            .WithDescription("Actualiza únicamente el estado de una propiedad existente.")
            .Accepts<UpdatePropertyStatusRequest>("application/json")
            .Produces<UpdatePropertyStatusResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        UpdatePropertyStatusRequest request,
        IEnumerable<IHandler> handlers,
        CancellationToken cancellationToken)
    {
        var handler = handlers.OfType<UpdatePropertyStatusHandler>().SingleOrDefault();
        if (handler is null)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        var result = await handler.HandleAsync(id, request, cancellationToken);
        return result.ToIResult(Results.Ok);
    }
}