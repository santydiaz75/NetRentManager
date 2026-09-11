using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Features.Properties.ListProperties;

public sealed record PropertyListItem(
    Guid Id,
    string Title,
    string Description,
    string Address,
    decimal Price,
    PropertyStatus Status,
    int BedroomCount,
    int BathroomCount,
    decimal AreaSquareMeters,
    string ImageUrl);
