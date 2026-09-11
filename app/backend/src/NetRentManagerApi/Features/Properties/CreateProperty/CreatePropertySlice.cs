using Microsoft.AspNetCore.Mvc;
using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;

namespace NetRentManagerApi.Features.Properties.CreateProperty;

public sealed class CreatePropertySlice : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/properties", HandleAsync)
            .WithName("CreateProperty")
            .WithTags("Properties")
            .WithSummary("Crear una propiedad")
            .WithDescription("Crea una propiedad con una imagen opcional.")
            .DisableAntiforgery()
            .Accepts<CreatePropertyRequest>("multipart/form-data")
            .Produces<CreatePropertyResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        [FromForm]
        CreatePropertyRequest request,
        [FromServices]
        IEnumerable<IHandler> handlers,
        CancellationToken cancellationToken)
    {
        var createPropertyHandler = handlers.OfType<CreatePropertyHandler>().SingleOrDefault();
        if (createPropertyHandler is null)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        var result = await createPropertyHandler.HandleAsync(request, cancellationToken);
        return result.ToIResult(response => Results.Created($"/api/properties/{response.Id}", response));
    }
}
