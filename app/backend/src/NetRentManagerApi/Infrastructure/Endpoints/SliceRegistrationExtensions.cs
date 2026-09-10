using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NetRentManagerApi.Infrastructure.Endpoints;

public static class SliceRegistrationExtensions
{
    public static IServiceCollection RegisterSlices(this IServiceCollection services, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(assembly);

        var sliceTypes = assembly
            .GetExportedTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(ISlice).IsAssignableFrom(type));

        foreach (var sliceType in sliceTypes)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ISlice), sliceType));
        }

        return services;
    }
}