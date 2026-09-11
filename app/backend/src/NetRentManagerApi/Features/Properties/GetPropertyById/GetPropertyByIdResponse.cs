namespace NetRentManagerApi.Features.Properties.GetPropertyById;

public sealed record GetPropertyByIdResponse(
    Guid Id,
    string Title,
    string Description,
    string Address,
    decimal Price,
    string Status,
    int BedroomCount,
    int BathroomCount,
    decimal AreaSquareMeters,
    string? ImageUrl);
