using System.Text.Json;
using Refit;

namespace NetRentManagerWeb.Services.Api.Properties;

public static class PropertiesApiClientRegistration
{
    // Reutiliza el HttpClient nombrado ya configurado en NetRentManagerApiClientRegistration (base address incluida).
    public static IServiceCollection AddPropertiesApiClient(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        var refitSettings = new RefitSettings(
            new SystemTextJsonContentSerializer(new JsonSerializerOptions(JsonSerializerDefaults.Web)));

        services.AddRefitClient<IPropertiesApi>(
            refitSettings,
            httpClientName: NetRentManagerApiClientRegistration.HttpClientName);

        return services;
    }
}
