using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NetRentManagerApi.Infrastructure.Handlers;

public static class HandlerRegistrationExtensions
{
    public static IServiceCollection RegisterHandlers(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        var handlerTypes = assembly
            .GetExportedTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IHandler).IsAssignableFrom(type));

        foreach (var handlerType in handlerTypes)
        {
            services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(IHandler), handlerType));
        }

        return services;
    }
}