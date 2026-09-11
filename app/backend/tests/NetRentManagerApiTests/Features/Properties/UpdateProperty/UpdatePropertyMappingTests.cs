using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.UpdateProperty;

namespace NetRentManagerApiTests.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyMappingTests
{
    [Fact]
    public void Mapping_UsesTextStatusNullableImageAndDates()
    {
        var createdAt = DateTimeOffset.UtcNow.AddDays(-1);
        var updatedAt = DateTimeOffset.UtcNow;
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
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var response = property.ToResponse();

        Assert.Equal("Maintenance", response.Status);
        Assert.Null(response.ImageUrl);
        Assert.Equal(createdAt, response.CreatedAt);
        Assert.Equal(updatedAt, response.UpdatedAt);
    }

    [Fact]
    public void Mapping_PreservesOnlyRelativePublicImageUrl()
    {
        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Address = "Address",
            Price = 100,
            Status = PropertyStatus.Available,
            BedroomCount = 1,
            BathroomCount = 1,
            AreaSquareMeters = 40,
            ImageUrl = "/assets/properties/1234567890abcdef1234567890abcdef.jpg",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var response = property.ToResponse();

        Assert.StartsWith("/assets/properties/", response.ImageUrl);
        Assert.DoesNotContain("support", response.ImageUrl, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("\\", response.ImageUrl);
        Assert.DoesNotContain("..", response.ImageUrl);
    }
}
