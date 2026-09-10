using System.Reflection;
using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;
using NetRentManagerApi.Infrastructure.Validation;

namespace NetRentManagerApi.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        services.AddLogging();
        services.AddNetRentManagerProblemDetails();
        services.RegisterSlices(assembly);
        services.RegisterHandlers(assembly);
        services.RegisterValidators(assembly);

        return services;
    }
}