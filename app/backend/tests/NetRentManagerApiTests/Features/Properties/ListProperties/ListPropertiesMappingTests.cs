using System.Text.Json;
using NetRentManagerApi.Features.Properties.ListProperties;

namespace NetRentManagerApiTests.Features.Properties.ListProperties;

public sealed class ListPropertiesMappingTests
{
    [Fact]
    public void ResponseContract_UsesItemsAndRequiredFields()
    {
        var response = new PagedPropertiesResponse(
            [new PropertyListItem(
                Guid.NewGuid(),
                "Title",
                "Description",
                "Address",
                100,
                "Available",
                2,
                1,
                75,
                "http://localhost:5023/assets/properties/1.png")],
            1,
            6,
            1,
            1,
            false,
            false);

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web)));
        var root = document.RootElement;

        Assert.True(root.TryGetProperty("items", out var items));
        Assert.Equal(1, items.GetArrayLength());
        Assert.True(items[0].TryGetProperty("imageUrl", out _));
        Assert.False(items[0].TryGetProperty("ImageUrl", out _));
    }
}
