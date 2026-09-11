using NetRentManagerApi.Features.Properties.ListProperties;

namespace NetRentManagerApiTests.Features.Properties.ListProperties;

public sealed class ListPropertiesRequestValidatorTests
{
    private readonly ListPropertiesRequestValidator validator = new();

    [Fact]
    public void Defaults_AreValid()
    {
        var result = validator.Validate(new ListPropertiesRequest());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0, 6)]
    [InlineData(-1, 6)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    [InlineData(1, 101)]
    public void OutOfRangeValues_AreRejected(int page, int pageSize)
    {
        var result = validator.Validate(new ListPropertiesRequest { Page = page, PageSize = pageSize });

        Assert.False(result.IsValid);
    }
}
