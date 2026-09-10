using System.Reflection;
using FluentValidation;

namespace NetRentManagerApi.Infrastructure.Validation;

public static class ValidatorRegistrationExtensions
{
    public static IServiceCollection RegisterValidators(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Scoped);

        return services;
    }
}