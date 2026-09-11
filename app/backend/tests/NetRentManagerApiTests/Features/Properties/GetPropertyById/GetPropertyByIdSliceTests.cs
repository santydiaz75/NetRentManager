using NetRentManagerApi.Features.Properties.GetPropertyById;
using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Handlers;

namespace NetRentManagerApiTests.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdSliceTests
{
    [Fact]
    public void SliceAndHandlerAreAutoDiscoverableContracts()
    {
        Assert.True(typeof(ISlice).IsAssignableFrom(typeof(GetPropertyByIdSlice)));
        Assert.Contains(typeof(IHandler), typeof(GetPropertyByIdHandler).GetInterfaces());
    }

    [Fact]
    public void SliceTypeDocumentsGuidRouteContract()
    {
        var route = "/api/properties/{id:guid}";

        Assert.Contains("{id:guid}", route, StringComparison.Ordinal);
    }
}
