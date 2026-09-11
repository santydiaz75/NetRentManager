using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.CreateProperty;

namespace NetRentManagerApiTests.Features.Properties.CreateProperty;

public sealed class CreatePropertyMappingTests
{
    [Fact]
    public void Mapping_UsesTextStatusAndNullableImageUrl()
    {
        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Address = "Address",
            Price = 100,
            Status = PropertyStatus.Maintenance,
            BedroomCount = 1,
            BathroomCount = 1,
            AreaSquareMeters = 40,
            ImageUrl = null,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var response = property.ToResponseForTest();

        Assert.Equal("Maintenance", response.Status);
        Assert.Null(response.ImageUrl);
    }
}

internal static class CreatePropertyMappingTestExtensions
{
    public static CreatePropertyResponse ToResponseForTest(this Property property)
        => new(
            property.Id,
            property.Title,
            property.Description,
            property.Address,
            property.Price,
            property.Status.ToString(),
            property.BedroomCount,
            property.BathroomCount,
            property.AreaSquareMeters,
            property.ImageUrl,
            property.CreatedAt);
}
