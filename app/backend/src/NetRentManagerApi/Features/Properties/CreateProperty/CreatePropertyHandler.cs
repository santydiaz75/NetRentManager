using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Infrastructure.Errors;
using NetRentManagerApi.Infrastructure.Handlers;
using NetRentManagerApi.Infrastructure.Persistence;

namespace NetRentManagerApi.Features.Properties.CreateProperty;

public sealed class CreatePropertyHandler(
    AppDbContext context,
    IWebHostEnvironment environment,
    ILogger<CreatePropertyHandler> logger) : IHandler
{
    private const long MaxImageSize = 5 * 1024 * 1024;
    private const string PublicImagePath = "/assets/properties/";

    public async Task<Result<CreatePropertyResponse>> HandleAsync(
        CreatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<PropertyStatus>(request.Status, ignoreCase: false, out var status))
        {
            return Validation("status", "status debe ser Available, Rented o Maintenance.");
        }

        string? imagePath = null;
        try
        {
            if (request.Image is not null)
            {
                var imageValidation = await ValidateImageAsync(request.Image, cancellationToken);
                if (imageValidation is not null)
                {
                    return Result<CreatePropertyResponse>.Failure(imageValidation);
                }

                var extension = NormalizeExtension(request.Image.FileName);
                var fileName = $"{Guid.NewGuid():N}{extension}";
                var imageDirectory = Path.Combine(environment.WebRootPath, "assets", "properties");
                Directory.CreateDirectory(imageDirectory);
                imagePath = Path.Combine(imageDirectory, fileName);

                await using var source = request.Image.OpenReadStream();
                await using var target = new FileStream(
                    imagePath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None,
                    64 * 1024,
                    FileOptions.Asynchronous | FileOptions.SequentialScan);
                await source.CopyToAsync(target, cancellationToken);
            }

            var property = new Property
            {
                Id = Guid.NewGuid(),
                Title = request.Title!.Trim(),
                Description = request.Description!.Trim(),
                Address = request.Address!.Trim(),
                Price = request.Price!.Value,
                Status = status,
                BedroomCount = request.BedroomCount!.Value,
                BathroomCount = request.BathroomCount!.Value,
                AreaSquareMeters = request.AreaSquareMeters!.Value,
                ImageUrl = imagePath is null
                    ? null
                    : $"{PublicImagePath}{Path.GetFileName(imagePath)}",
                CreatedAt = DateTimeOffset.UtcNow
            };

            context.Properties.Add(property);
            await context.SaveChangesAsync(cancellationToken);
            return Result<CreatePropertyResponse>.Success(property.ToResponse());
        }
        catch (OperationCanceledException)
        {
            await DeleteCreatedFileAsync(imagePath);
            throw;
        }
        catch (Exception exception)
        {
            await DeleteCreatedFileAsync(imagePath);
            logger.LogError(exception, "Failed to create property with optional image");
            return Result<CreatePropertyResponse>.Failure(
                Error.Internal("properties.create.failed", "No se pudo crear la propiedad."));
        }
    }

    private static async Task<Error?> ValidateImageAsync(IFormFile image, CancellationToken cancellationToken)
    {
        if (image.Length <= 0)
        {
            return Error.Validation(
                "properties.image.empty",
                "La imagen no puede estar vacía.",
                new Dictionary<string, string[]> { ["image"] = ["La imagen no puede estar vacía."] });
        }

        if (image.Length > MaxImageSize)
        {
            return Error.Validation(
                "properties.image.too_large",
                "La imagen no puede superar 5 MiB.",
                new Dictionary<string, string[]> { ["image"] = ["La imagen no puede superar 5 MiB."] });
        }

        await using var stream = image.OpenReadStream();
        var header = new byte[8];
        var read = await stream.ReadAsync(header.AsMemory(), cancellationToken);
        var extension = NormalizeExtension(image.FileName);
        var isPng = extension == ".png"
            && read >= 8
            && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });
        var isJpeg = (extension == ".jpg" || extension == ".jpeg")
            && read >= 3
            && header.AsSpan(0, 3).SequenceEqual(new byte[] { 0xFF, 0xD8, 0xFF });

        if (!isPng && !isJpeg)
        {
            return Error.Validation(
                "properties.image.invalid_content",
                "El contenido de image no coincide con PNG o JPEG.",
                new Dictionary<string, string[]> { ["image"] = ["El contenido no coincide con PNG o JPEG."] });
        }

        return null;
    }

    private static string NormalizeExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension == ".jpeg" ? ".jpg" : extension;
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
        catch
        {
            // The original failure is the actionable error; cleanup failure is logged by the host.
        }
    }

    private static Result<CreatePropertyResponse> Validation(string field, string message)
        => Result<CreatePropertyResponse>.Failure(
            Error.Validation(
                "properties.request.invalid",
                message,
                new Dictionary<string, string[]> { [field] = [message] }));
}
