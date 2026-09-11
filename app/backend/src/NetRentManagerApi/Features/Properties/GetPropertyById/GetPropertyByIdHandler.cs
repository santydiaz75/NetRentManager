using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApi.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdHandler(
    AppDbContext context,
    ILogger<GetPropertyByIdHandler> logger) : IHandler
{
    public async Task<Result<GetPropertyByIdResponse>> HandleAsync(
        GetPropertyByIdRequest request,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var projection = await context.Properties
                .AsNoTracking()
                .Where(property => property.Id == request.Id)
                .Select(property => new GetPropertyByIdProjection(
                    property.Id,
                    property.Title,
                    property.Description,
                    property.Address,
                    property.Price,
                    property.Status,
                    property.BedroomCount,
                    property.BathroomCount,
                    property.AreaSquareMeters,
                    property.ImageUrl))
                .SingleOrDefaultAsync(cancellationToken);

            if (projection is null)
            {
                return Result<GetPropertyByIdResponse>.Failure(
                    Error.NotFound("properties.not_found", "La propiedad no existe."));
            }

            var response = projection.ToResponse(httpRequest);
            if (response.IsFailure)
            {
                logger.LogError("Property {PropertyId} has an invalid public image URL", request.Id);
            }

            return response;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to get property {PropertyId}", request.Id);
            return Result<GetPropertyByIdResponse>.Failure(
                Error.Internal("properties.get_by_id.failed", "No se pudo consultar la propiedad."));
        }
    }
}
