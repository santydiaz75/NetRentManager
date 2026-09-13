using Refit;

namespace NetRentManagerWeb.Services.Api.System;

public interface IBackendHealthApi
{
    [Get("/health")]
    Task<HttpResponseMessage> GetHealthAsync(CancellationToken cancellationToken = default);
}
