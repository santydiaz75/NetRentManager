using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Features.Properties.UpdateProperty;

public static class UpdatePropertyMapping
{
    public static UpdatePropertyResponse ToResponse(this Property property)
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
            property.CreatedAt,
            property.UpdatedAt);
}
