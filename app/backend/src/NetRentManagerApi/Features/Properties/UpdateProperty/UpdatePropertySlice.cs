using Microsoft.AspNetCore.Mvc;
using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;

namespace NetRentManagerApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertySlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/properties/{id:guid}", HandleAsync)
            .WithName("UpdateProperty")
            .WithTags("Properties")
            .DisableAntiforgery()
            .Accepts<UpdatePropertyRequest>("multipart/form-data")
            .Produces<UpdatePropertyResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
            .ProducesProblem(StatusCodes.Status415UnsupportedMediaType)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        Guid id,
        [FromForm] UpdatePropertyRequest request,
        [FromServices] IEnumerable<IHandler> handlers,
        CancellationToken cancellationToken)
    {
        var updatePropertyHandler = handlers.OfType<UpdatePropertyHandler>().SingleOrDefault();
        if (updatePropertyHandler is null)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        var result = await updatePropertyHandler.HandleAsync(request with { Id = id }, cancellationToken);
        return result.ToIResult(Results.Ok);
    }
}
