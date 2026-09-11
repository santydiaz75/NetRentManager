using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Infrastructure.Errors;

namespace NetRentManagerApi.Features.Properties.ListProperties;

internal sealed record PropertyListProjection(
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

internal static class ListPropertiesMapping
{
    private const string PublicImagePath = "/assets/properties/";

    public static Result<PropertyListItem> ToResponse(this PropertyListProjection projection, HttpRequest request)
    {
        if (request.Host is { HasValue: false }
            || string.IsNullOrWhiteSpace(request.Scheme))
        {
            return Result<PropertyListItem>.Failure(
                Error.Internal("properties.image.invalid", "La propiedad no tiene una imagen pública válida."));
        }

        if (projection.ImageUrl is null)
        {
            return Result<PropertyListItem>.Success(new PropertyListItem(
                projection.Id,
                projection.Title,
                projection.Description,
                projection.Address,
                projection.Price,
                projection.Status.ToString(),
                projection.BedroomCount,
                projection.BathroomCount,
                projection.AreaSquareMeters,
                null));
        }

        if (string.IsNullOrWhiteSpace(projection.ImageUrl))
        {
            return Result<PropertyListItem>.Failure(
                Error.Internal("properties.image.invalid", "La propiedad no tiene una imagen pública válida."));
        }

        var normalizedImageUrl = projection.ImageUrl.Trim().Replace('\\', '/');
        var isPublicImagePath = normalizedImageUrl.StartsWith(PublicImagePath, StringComparison.OrdinalIgnoreCase);
        if (normalizedImageUrl.Contains("support", StringComparison.OrdinalIgnoreCase)
            || normalizedImageUrl.Contains("..", StringComparison.Ordinal)
            || Uri.TryCreate(normalizedImageUrl, UriKind.Absolute, out _)
            || (Path.IsPathRooted(normalizedImageUrl) && !isPublicImagePath))
        {
            return Result<PropertyListItem>.Failure(
                Error.Internal("properties.image.invalid", "La propiedad no tiene una imagen pública válida."));
        }

        var fileName = Path.GetFileName(normalizedImageUrl);
        if (string.IsNullOrWhiteSpace(fileName)
            || fileName is "." or ".."
            || fileName.Contains('/')
            || fileName.Contains('\\'))
        {
            return Result<PropertyListItem>.Failure(
                Error.Internal("properties.image.invalid", "La propiedad no tiene una imagen pública válida."));
        }

        var builder = new UriBuilder(request.Scheme, request.Host.Host, request.Host.Port ?? -1)
        {
            Path = PublicImagePath + Uri.EscapeDataString(fileName)
        };

        return Result<PropertyListItem>.Success(new PropertyListItem(
            projection.Id,
            projection.Title,
            projection.Description,
            projection.Address,
            projection.Price,
            projection.Status.ToString(),
            projection.BedroomCount,
            projection.BathroomCount,
            projection.AreaSquareMeters,
            builder.Uri.AbsoluteUri));
    }
}
