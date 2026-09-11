using Microsoft.EntityFrameworkCore;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyHandler(
    AppDbContext context,
    IWebHostEnvironment environment,
    ILogger<UpdatePropertyHandler> logger) : IHandler
{
    private const long MaxImageSize = 5 * 1024 * 1024;
    private const string PublicImagePath = "/assets/properties/";

    public async Task<Result<UpdatePropertyResponse>> HandleAsync(
        UpdatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var property = await context.Properties
            .SingleOrDefaultAsync(item => item.Id == request.Id, cancellationToken);

        if (property is null)
        {
            return Result<UpdatePropertyResponse>.Failure(
                Error.NotFound("properties.not_found", "La propiedad no existe."));
        }

        var original = new PropertySnapshot(property);
        string? createdImagePath = null;
        try
        {
            if (request.Image is not null)
            {
                var imageValidation = await ValidateImageAsync(request.Image, cancellationToken);
                if (imageValidation is not null)
                {
                    return Result<UpdatePropertyResponse>.Failure(imageValidation);
                }

                var extension = NormalizeExtension(request.Image.FileName);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var imageDirectory = Path.Combine(environment.WebRootPath, "assets", "properties");
                Directory.CreateDirectory(imageDirectory);
                createdImagePath = Path.Combine(imageDirectory, fileName);

                await using var source = request.Image.OpenReadStream();
                await using var target = new FileStream(
                    createdImagePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    64 * 1024,
                    FileOptions.Asynchronous | FileOptions.SequentialScan);
                await source.CopyToAsync(target, cancellationToken);

                property.ImageUrl = $"{PublicImagePath}{fileName}";
            }

            property.Title = request.Title!.Trim();
            property.Description = request.Description!.Trim();
            property.Address = request.Address!.Trim();
            property.Price = request.Price!.Value;
            property.Status = Enum.Parse<PropertyStatus>(request.Status!, ignoreCase: false);
            property.BedroomCount = request.BedroomCount!.Value;
            property.BathroomCount = request.BathroomCount!.Value;
            property.AreaSquareMeters = request.AreaSquareMeters!.Value;
            property.UpdatedAt = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync(cancellationToken);
            var previousImageUrl = original.ImageUrl;
            if (createdImagePath is not null)
            {
                await DeletePreviousImageAsync(previousImageUrl, cancellationToken);
            }

            createdImagePath = null;
            return Result<UpdatePropertyResponse>.Success(property.ToResponse());
        }
        catch (OperationCanceledException)
        {
            Restore(property, original);
            await DeleteCreatedFileAsync(createdImagePath);
            throw;
        }
        catch (Exception exception)
        {
            Restore(property, original);
            await DeleteCreatedFileAsync(createdImagePath);
            logger.LogError(exception, "Failed to update property {PropertyId}", request.Id);
            return Result<UpdatePropertyResponse>.Failure(
                Error.Internal("properties.update.failed", "No se pudo actualizar la propiedad."));
        }
    }

    private async Task DeletePreviousImageAsync(string? imageUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)
            || !imageUrl.StartsWith(PublicImagePath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var fileName = Path.GetFileName(imageUrl);
        if (string.IsNullOrWhiteSpace(fileName)
            || fileName is "." or ".."
            || fileName.Contains('/')
            || fileName.Contains('\\'))
        {
            return;
        }

        var imagePath = Path.Combine(environment.WebRootPath, "assets", "properties", fileName);
        try
        {
            if (File.Exists(imagePath))
            {
                await Task.Run(() => File.Delete(imagePath), cancellationToken);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to delete previous property image {ImagePath}", imagePath);
        }
    }

    private static async Task<Error?> ValidateImageAsync(IFormFile image, CancellationToken cancellationToken)
    {
        if (image.Length <= 0)
        {
            return Error.UnsupportedMediaType(
                "properties.image.empty",
                "La imagen no puede estar vacía.");
        }

        if (image.Length > MaxImageSize)
        {
            return Error.PayloadTooLarge(
                "properties.image.too_large",
                "La imagen no puede superar 5 MiB.");
        }

        var extension = NormalizeExtension(image.FileName);
        if (extension is not ".png" and not ".jpg"
            || !string.Equals(image.ContentType, "image/png", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(image.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase))
        {
            return Error.UnsupportedMediaType(
                "properties.image.unsupported",
                "La imagen debe ser PNG o JPG/JPEG.");
        }

        await using var stream = image.OpenReadStream();
        var header = new byte[8];
        var read = await stream.ReadAsync(header.AsMemory(), cancellationToken);
        var isPng = extension == ".png"
            && read >= 8
            && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
        var isJpeg = extension == ".jpg"
            && read >= 3
            && header.AsSpan(0, 3).SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF });

        return isPng || isJpeg
            ? null
            : Error.UnsupportedMediaType(
                "properties.image.invalid_content",
                "El contenido de image no coincide con PNG o JPEG.");
    }

    private static string NormalizeExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension == ".jpeg" ? ".jpg" : extension;
    }

    private static void Restore(Property property, PropertySnapshot original)
    {
        property.Title = original.Title;
        property.Description = original.Description;
        property.Address = original.Address;
        property.Price = original.Price;
        property.Status = original.Status;
        property.BedroomCount = original.BedroomCount;
        property.BathroomCount = original.BathroomCount;
        property.AreaSquareMeters = original.AreaSquareMeters;
        property.ImageUrl = original.ImageUrl;
        property.UpdatedAt = original.UpdatedAt;
    }

    private static async Task DeleteCreatedFileAsync(string? imagePath)
    {
        if (imagePath is null)
        {
            return;
        }

        try
        {
            if (File.Exists(imagePath))
            {
                await Task.Run(() => File.Delete(imagePath));
            }
        }
        catch (Exception cleanupException)
        {
            _ = cleanupException;
        }
    }

    private sealed record PropertySnapshot(
        string Title,
        string Description,
        string Address,
        decimal Price,
        PropertyStatus Status,
        int BedroomCount,
        int BathroomCount,
        decimal AreaSquareMeters,
        string? ImageUrl,
        DateTimeOffset? UpdatedAt)
    {
        public PropertySnapshot(Property property)
            : this(
                property.Title,
                property.Description,
                property.Address,
                property.Price,
                property.Status,
                property.BedroomCount,
                property.BathroomCount,
                property.AreaSquareMeters,
                property.ImageUrl,
                property.UpdatedAt)
        {
        }
    }
}
