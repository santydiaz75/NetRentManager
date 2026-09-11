using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Features.Properties.UpdatePropertyStatus;

public static class UpdatePropertyStatusMapping
{
    public static UpdatePropertyStatusResponse ToResponse(this Property property)
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
            property.ImageUrl);
}