namespace NetRentManagerWeb.Services.Api.Properties;

public sealed record PropertyDetailResponse(
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
