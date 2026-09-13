using Microsoft.Extensions.Options;
using Refit;
using NetRentManagerWeb.Services.Api.Properties;
using NetRentManagerWeb.Services.Api.System;

namespace NetRentManagerWeb.Services.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNetRentManagerApiClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<NetRentManagerApiOptions>(configuration.GetSection(NetRentManagerApiOptions.SectionName));

        services.AddRefitClient<IBackendHealthApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                client.BaseAddress = ResolveBaseAddress(sp);
            });

        services.AddRefitClient<IPropertiesApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                client.BaseAddress = ResolveBaseAddress(sp);
            });

        return services;
    }

    private static Uri ResolveBaseAddress(IServiceProvider serviceProvider)
    {
        var options = serviceProvider
            .GetRequiredService<IOptions<NetRentManagerApiOptions>>()
            .Value;

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseAddress))
        {
            throw new InvalidOperationException(
                $"La configuracion '{NetRentManagerApiOptions.SectionName}:BaseUrl' debe ser una URI absoluta valida.");
        }

        return baseAddress;
    }
}
