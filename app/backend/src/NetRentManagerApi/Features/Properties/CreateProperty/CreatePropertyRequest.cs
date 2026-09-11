namespace NetRentManagerApi.Features.Properties.CreateProperty;

public sealed record CreatePropertyRequest
{
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
