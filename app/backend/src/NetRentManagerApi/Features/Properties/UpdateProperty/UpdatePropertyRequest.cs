namespace NetRentManagerApi.Features.Properties.UpdateProperty;

public sealed record UpdatePropertyRequest
{
    public Guid Id { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public string? Address { get; init; }

    public decimal? Price { get; init; }

    public string? Status { get; init; }

    public int? BedroomCount { get; init; }

    public int? BathroomCount { get; init; }

    public decimal? AreaSquareMeters { get; init; }

    public IFormFile? Image { get; init; }
}
