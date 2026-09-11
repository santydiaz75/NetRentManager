using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApi.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusHandler(
    AppDbContext context,
    ILogger<UpdatePropertyStatusHandler> logger) : IHandler
{
    public async Task<Result<UpdatePropertyStatusResponse>> HandleAsync(
        Guid id,
        UpdatePropertyStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var property = await context.Properties
                .SingleOrDefaultAsync(candidate => candidate.Id == id, cancellationToken);

            if (property is null)
            {
                return Result<UpdatePropertyStatusResponse>.Failure(
                    Error.NotFound("properties.not_found", "La propiedad no existe."));
            }

            var status = Enum.Parse<PropertyStatus>(request.Status!, ignoreCase: true);
            property.Status = status;
            await context.SaveChangesAsync(cancellationToken);

            return Result<UpdatePropertyStatusResponse>.Success(property.ToResponse());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to update property status for {PropertyId}", id);
            return Result<UpdatePropertyStatusResponse>.Failure(
                Error.Internal("properties.status_update.failed", "No se pudo actualizar el estado de la propiedad."));
        }
    }
}