using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApi.Features.Properties.ListProperties;

public sealed class ListPropertiesHandler(AppDbContext context, ILogger<ListPropertiesHandler> logger) : IHandler
{
    public async Task<Result<PagedPropertiesResponse>> HandleAsync(
        ListPropertiesRequest request,
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        try
        {
            var page = request.EffectivePage;
            var pageSize = request.EffectivePageSize;
            var query = context.Properties
                .AsNoTracking()
                .OrderBy(property => property.Title)
                .ThenBy(property => property.Id);

            var totalItems = await query.CountAsync(cancellationToken);
            var projections = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(property => new PropertyListProjection(
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
                .ToListAsync(cancellationToken);

            var items = new List<PropertyListItem>(projections.Count);
            foreach (var projection in projections)
            {
                var item = projection.ToResponse(httpRequest);
                if (item.IsFailure)
                {
                    logger.LogError(
                        "Property {PropertyId} has an invalid public image URL",
                        projection.Id);
                    return Result<PagedPropertiesResponse>.Failure(item.Error!);
                }

                items.Add(item.Value!);
            }

            var totalPages = totalItems == 0
                ? 0
                : (int)(((long)totalItems + pageSize - 1) / pageSize);

            return Result<PagedPropertiesResponse>.Success(new PagedPropertiesResponse(
                items,
                page,
                pageSize,
                totalItems,
                totalPages,
                page < totalPages,
                page > 1 && totalItems > 0));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to list properties");
            return Result<PagedPropertiesResponse>.Failure(
                Error.Internal("properties.list.failed", "No se pudo listar las propiedades."));
        }
    }
}
