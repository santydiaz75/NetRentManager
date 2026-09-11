using NetRentManagerApi.Features.Properties.CreateProperty;
using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Handlers;

namespace NetRentManagerApiTests.Features.Properties.CreateProperty;

public sealed class CreatePropertySliceTests
{
    [Fact]
    public void SliceAndHandler_AreDiscoverableContracts()
    {
        Assert.True(typeof(ISlice).IsAssignableFrom(typeof(CreatePropertySlice)));
        Assert.True(typeof(IHandler).IsAssignableFrom(typeof(CreatePropertyHandler)));
        Assert.True(typeof(CreatePropertySlice).IsPublic);
        Assert.True(typeof(CreatePropertyHandler).IsPublic);
    }
}
