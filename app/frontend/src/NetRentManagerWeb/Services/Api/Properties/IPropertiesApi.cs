using Refit;
using NetRentManagerWeb.Features.Properties.Detail;
using NetRentManagerWeb.Features.Properties.List;

namespace NetRentManagerWeb.Services.Api.Properties;

public interface IPropertiesApi
{
    [Get("/api/properties")]
    Task<PagedPropertyListResponse> GetPropertiesAsync(
        [Query("Page")] int page,
        [Query("PageSize")] int pageSize,
        CancellationToken cancellationToken = default);

    [Get("/api/properties/{id}")]
    Task<PropertyDetailResponse> GetPropertyByIdAsync(
        string id,
        CancellationToken cancellationToken = default);
}
