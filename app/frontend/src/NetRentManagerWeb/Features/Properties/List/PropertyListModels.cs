namespace NetRentManagerWeb.Features.Properties.List;

public sealed record PropertyListItem(
    Guid Id,
    string Title,
    string? ImageUrl,
    decimal Price,
    string Status,
    string Address,
    int BedroomCount,
    int BathroomCount,
    decimal AreaSquareMeters);

public sealed record PagedPropertyListResponse(
    IReadOnlyList<PropertyListItem> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);
