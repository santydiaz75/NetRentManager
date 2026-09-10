using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApiTests.TestTypes;

namespace NetRentManagerApiTests.Infrastructure.Endpoints;

public class RegisterSlicesTests
{
    [Fact]
    public void RegisterSlices_Discovers_TestSlice_FromProvidedAssembly_OnlyOnce()
    {
        var services = new ServiceCollection();
        var assembly = typeof(TestSlice).Assembly;

        services.RegisterSlices(assembly);
        services.RegisterSlices(assembly);

        using var provider = services.BuildServiceProvider();
        var slices = provider.GetServices<ISlice>().ToArray();

        Assert.Single(slices, x => x.GetType() == typeof(TestSlice));
        Assert.DoesNotContain(slices, x => x.GetType().Name == nameof(HealthSlice));
    }
}