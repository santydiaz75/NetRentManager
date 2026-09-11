using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.UpdatePropertyStatus;

namespace NetRentManagerApiTests.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusMappingTests
{
    [Fact]
    public void Maps_full_property_response_and_preserves_image_url()
    {
        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = "Apartamento",
            Description = "Descripción",
            Address = "Calle Principal 10",
            Price = 1200,
            Status = PropertyStatus.Rented,
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 70,
            ImageUrl = "/assets/properties/property.jpg"
        };

        var response = property.ToResponse();

        Assert.Equal(property.Id, response.Id);
        Assert.Equal(property.Status.ToString(), response.Status);
        Assert.Equal(property.ImageUrl, response.ImageUrl);
        Assert.Equal(property.Title, response.Title);
    }
}
