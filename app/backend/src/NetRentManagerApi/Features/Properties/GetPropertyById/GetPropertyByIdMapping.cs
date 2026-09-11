using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Infrastructure.Errors;

namespace NetRentManagerApi.Features.Properties.GetPropertyById;

public sealed record GetPropertyByIdProjection(
    Guid Id,
    string Title,
    string Description,
    string Address,
    decimal Price,
    PropertyStatus Status,
    int BedroomCount,
    int BathroomCount,
    decimal AreaSquareMeters,
    string? ImageUrl);

public static class GetPropertyByIdMapping
{
    private const string PublicImagePath = "/assets/properties/";

    public static Result<GetPropertyByIdResponse> ToResponse(
        this GetPropertyByIdProjection projection,
        HttpRequest request)
    {
        string? imageUrl = null;
        if (projection.ImageUrl is not null)
        {
            if (request.Host is { HasValue: false } || string.IsNullOrWhiteSpace(request.Scheme))
            {
                return InvalidImage();
            }

            var normalizedImageUrl = projection.ImageUrl.Trim().Replace('\\', '/');
            var isPublicImagePath = normalizedImageUrl.StartsWith(PublicImagePath, StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(normalizedImageUrl)
                || normalizedImageUrl.Contains("support", StringComparison.OrdinalIgnoreCase)
                || normalizedImageUrl.Contains("..", StringComparison.Ordinal)
                || Uri.TryCreate(normalizedImageUrl, UriKind.Absolute, out _)
                || (Path.IsPathRooted(normalizedImageUrl) && !isPublicImagePath))
            {
                return InvalidImage();
            }

            var fileName = Path.GetFileName(normalizedImageUrl);
            if (string.IsNullOrWhiteSpace(fileName)
                || fileName is "." or ".."
                || fileName.Contains('/')
                || fileName.Contains('\\'))
            {
                return InvalidImage();
            }

            var builder = new UriBuilder(request.Scheme, request.Host.Host, request.Host.Port ?? -1)
            {
                Path = PublicImagePath + Uri.EscapeDataString(fileName)
            };
            imageUrl = builder.Uri.AbsoluteUri;
        }

        return Result<GetPropertyByIdResponse>.Success(new GetPropertyByIdResponse(
            projection.Id,
            projection.Title,
            projection.Description,
            projection.Address,
            projection.Price,
            projection.Status.ToString(),
            projection.BedroomCount,
            projection.BathroomCount,
            projection.AreaSquareMeters,
            imageUrl));
    }

    private static Result<GetPropertyByIdResponse> InvalidImage()
        => Result<GetPropertyByIdResponse>.Failure(
            Error.Internal("properties.image.invalid", "La propiedad no tiene una imagen pública válida."));
}
