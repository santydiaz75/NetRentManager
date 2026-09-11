using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.GetPropertyById;
using NetRentManagerApi.Infrastructure.Errors;
using System.Text.Json;

namespace NetRentManagerApiTests.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdMappingTests
{
    [Fact]
    public void Mapping_UsesAllPublicFieldsAndAbsoluteImageUrl()
    {
        var projection = new GetPropertyByIdProjection(
            Guid.NewGuid(),
            "Title",
            "Description",
            "Address",
            100,
            PropertyStatus.Maintenance,
            2,
            1,
            75,
            "/assets/properties/house image.png");
        var request = new DefaultHttpContext().Request;
        request.Scheme = "https";
        request.Host = new HostString("api.example.test", 8443);

        var result = projection.ToResponse(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("Maintenance", result.Value!.Status);
        Assert.Equal("https://api.example.test:8443/assets/properties/house%20image.png", result.Value.ImageUrl);
    }

    [Fact]
    public void Mapping_WithoutImage_ReturnsNullableImageUrl()
    {
        var projection = new GetPropertyByIdProjection(
            Guid.NewGuid(), "Title", "Description", "Address", 100,
            PropertyStatus.Available, 2, 1, 75, null);

        var result = projection.ToResponse(new DefaultHttpContext().Request);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value!.ImageUrl);
    }

    [Theory]
    [InlineData("support/image.png")]
    [InlineData("C:/private/image.png")]
    [InlineData("/assets/properties/../image.png")]
    [InlineData("https://other.example/image.png")]
    public void Mapping_WithUnsafeImageUrl_ReturnsInternalError(string imageUrl)
    {
        var projection = new GetPropertyByIdProjection(
            Guid.NewGuid(), "Title", "Description", "Address", 100,
            PropertyStatus.Available, 2, 1, 75, imageUrl);
        var request = new DefaultHttpContext().Request;
        request.Scheme = "https";
        request.Host = new HostString("api.example.test");

        var result = projection.ToResponse(request);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Internal, result.Error!.Type);
    }

    [Fact]
    public void ResponseContract_DoesNotContainPaginationMetadata()
    {
        var response = new GetPropertyByIdResponse(
            Guid.NewGuid(), "Title", "Description", "Address", 100,
            "Available", 2, 1, 75, null);

        using var document = JsonDocument.Parse(JsonSerializer.Serialize(response, new JsonSerializerOptions(JsonSerializerDefaults.Web)));

        Assert.False(document.RootElement.TryGetProperty("items", out _));
        Assert.False(document.RootElement.TryGetProperty("page", out _));
        Assert.False(document.RootElement.TryGetProperty("totalItems", out _));
    }
}
