using Refit;

namespace NetRentManagerWeb.Services.Api.Properties;

public interface IPropertiesApi
{
    [Get("/api/properties")]
    Task<ApiResponse<PagedPropertiesResponse>> GetPropertiesAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    
    [Get("/api/properties/{id}")]
    Task<ApiResponse<PropertyDetailResponse>> GetPropertyByIdAsync(
        string id,
        CancellationToken cancellationToken = default);
}
