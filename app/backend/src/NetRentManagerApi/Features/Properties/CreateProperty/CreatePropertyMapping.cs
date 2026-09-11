using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Features.Properties.CreateProperty;

internal static class CreatePropertyMapping
{
    public static CreatePropertyResponse ToResponse(this Property property)
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
