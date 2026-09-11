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
            .WithTags("Properties");
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters]
        ListPropertiesRequest request,
        [FromServices]
        IHandler handler,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        if (handler is not ListPropertiesHandler listPropertiesHandler)
        {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError);
        }

        var result = await listPropertiesHandler.HandleAsync(request, httpRequest, cancellationToken);
        return result.ToIResult(Results.Ok);
    }
}
