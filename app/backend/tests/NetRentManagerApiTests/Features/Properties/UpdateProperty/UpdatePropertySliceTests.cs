using NetRentManagerApi.Features.Properties.UpdateProperty;
using NetRentManagerApi.Infrastructure.Endpoints;

namespace NetRentManagerApiTests.Features.Properties.UpdateProperty;

public sealed class UpdatePropertySliceTests
{
    [Fact]
    public void SliceIsPublicAndImplementsISlice()
    {
        var type = typeof(UpdatePropertySlice);

        Assert.True(type.IsPublic);
        Assert.True(typeof(ISlice).IsAssignableFrom(type));
    }

    [Fact]
    public void HandlerIsPublicAndImplementsIHandler()
    {
        var type = typeof(UpdatePropertyHandler);

        Assert.True(type.IsPublic);
        Assert.Contains(type.GetInterfaces(), implemented => implemented.Name == "IHandler");
    }
}