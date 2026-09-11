using System.Reflection;
using NetRentManagerApi.Features.Properties.ListProperties;
using NetRentManagerApi.Infrastructure.Endpoints;

namespace NetRentManagerApiTests.Features.Properties.ListProperties;

public sealed class ListPropertiesSliceTests
{
    [Fact]
    public void SliceIsPublicAndImplementsISlice()
    {
        var type = typeof(ListPropertiesSlice);

        Assert.True(type.IsPublic);
        Assert.True(typeof(ISlice).IsAssignableFrom(type));
    }

    [Fact]
    public void HandlerIsPublicAndImplementsIHandler()
    {
        var type = typeof(ListPropertiesHandler);

        Assert.True(type.IsPublic);
        Assert.Contains(type.GetInterfaces(), implemented => implemented.Name == "IHandler");
    }
}
