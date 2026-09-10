using NetRentManagerApi.Infrastructure.Handlers;
using NetRentManagerApiTests.TestTypes;

namespace NetRentManagerApiTests.Infrastructure.Handlers;

public class RegisterHandlersTests
{
    [Fact]
    public void RegisterHandlers_DiscoversScopedHandlers_WithoutDuplicates()
    {
        var services = new ServiceCollection();
        var assembly = typeof(TestHandler).Assembly;

        services.RegisterHandlers(assembly);
        services.RegisterHandlers(assembly);

        var descriptors = services
            .Where(x => x.ServiceType == typeof(IHandler) && x.ImplementationType == typeof(TestHandler))
            .ToArray();

        Assert.Single(descriptors);
        Assert.All(descriptors, descriptor => Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime));
    }
}