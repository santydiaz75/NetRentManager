namespace NetRentManagerApi.Features.Properties.ListProperties;

public sealed record PagedPropertiesResponse(
    IReadOnlyList<PropertyListItem> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages,
    bool HasNext,
    bool HasPrevious);
