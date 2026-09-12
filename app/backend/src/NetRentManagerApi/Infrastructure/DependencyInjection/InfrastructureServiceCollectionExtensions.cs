using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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

        var connectionString = BuildConnectionString(configuration);

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

    // Combina la cadena base (sin credenciales) con usuario/contraseña provistos por Secret Manager, variables de entorno o Key Vault.
    public static string BuildConnectionString(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var baseConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "No se encontró la cadena de conexión 'DefaultConnection' para AppDbContext.");

        var username = configuration["DatabaseCredentials:Username"];
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new InvalidOperationException(
                "No se encontró 'DatabaseCredentials:Username'. Configúralo con Secret Manager (dotnet user-secrets) en desarrollo o mediante variables de entorno/Azure Key Vault en producción.");
        }

        var password = configuration["DatabaseCredentials:Password"];
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "No se encontró 'DatabaseCredentials:Password'. Configúralo con Secret Manager (dotnet user-secrets) en desarrollo o mediante variables de entorno/Azure Key Vault en producción.");
        }

        var builder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            Username = username,
            Password = password
        };

        return builder.ConnectionString;
    }
}