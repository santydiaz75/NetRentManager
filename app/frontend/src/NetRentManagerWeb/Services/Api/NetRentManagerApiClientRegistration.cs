namespace NetRentManagerWeb.Services.Api;

public static class NetRentManagerApiClientRegistration
{
    public const string HttpClientName = "NetRentManagerApi";

    // Deja el HttpClient nombrado listo para que features futuras registren interfaces Refit sobre él.
    public static IServiceCollection AddNetRentManagerApiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var baseUrl = configuration["ApiSettings:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                "No se encontró 'ApiSettings:BaseUrl'. Configúralo en appsettings.json/appsettings.Development.json con la URL del backend NetRentManagerApi.");
        }

        services.AddHttpClient(HttpClientName, client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        return services;
    }
}
