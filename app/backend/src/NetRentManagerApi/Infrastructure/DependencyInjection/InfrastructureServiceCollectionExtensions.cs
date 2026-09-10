using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;
using NetRentManagerApi.Infrastructure.Persistence;
using NetRentManagerApi.Infrastructure.Validation;

namespace NetRentManagerApi.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(assembly);

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'DefaultConnection' para AppDbContext.");

        services.AddLogging();
        services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSeeding((context, _) => DatabaseSeeder.Seed((AppDbContext)context))
                .UseAsyncSeeding((context, _, cancellationToken) =>
                    DatabaseSeeder.SeedAsync((AppDbContext)context, cancellationToken)));
        services.AddNetRentManagerProblemDetails();
        services.RegisterSlices(assembly);
        services.RegisterHandlers(assembly);
        services.RegisterValidators(assembly);

        return services;
    }
}