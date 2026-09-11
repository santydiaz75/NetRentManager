using FluentValidation;
using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Features.Properties.CreateProperty;

public sealed class CreatePropertyRequestValidator : AbstractValidator<CreatePropertyRequest>
{
    private const long MaxImageSize = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".png", ".jpg", ".jpeg"];
    private static readonly string[] AllowedContentTypes = ["image/png", "image/jpeg"];

    public CreatePropertyRequestValidator()
    {
        RuleFor(request => request.Title)
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(request => request.Description)
            .NotEmpty()
            .MaximumLength(2000);
        RuleFor(request => request.Address)
            .NotEmpty()
            .MaximumLength(300);
        RuleFor(request => request.Price)
            .NotNull()
            .GreaterThanOrEqualTo(0);
        RuleFor(request => request.Status)
            .NotEmpty()
            .Must(status => Enum.TryParse<PropertyStatus>(status, ignoreCase: false, out _))
            .WithMessage("status debe ser Available, Rented o Maintenance.");
        RuleFor(request => request.BedroomCount)
            .NotNull()
            .GreaterThanOrEqualTo(0);
        RuleFor(request => request.BathroomCount)
            .NotNull()
            .GreaterThanOrEqualTo(0);
        RuleFor(request => request.AreaSquareMeters)
            .NotNull()
            .GreaterThan(0);
        RuleFor(request => request.Image)
            .Must(image => image is null || image.Length > 0)
            .WithMessage("image no puede estar vacío.")
            .Must(image => image is null || image.Length <= MaxImageSize)
            .WithMessage("image no puede superar 5 MiB.")
            .Must(image => image is null || AllowedExtensions.Contains(Path.GetExtension(image.FileName), StringComparer.OrdinalIgnoreCase))
            .WithMessage("image debe ser PNG o JPG/JPEG.")
            .Must(image => image is null || AllowedContentTypes.Contains(image.ContentType, StringComparer.OrdinalIgnoreCase))
            .WithMessage("image debe declarar un Content-Type PNG o JPEG.");
    }
}
