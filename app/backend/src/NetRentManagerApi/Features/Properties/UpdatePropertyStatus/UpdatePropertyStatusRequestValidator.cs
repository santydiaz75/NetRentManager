using FluentValidation;
using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusRequestValidator : AbstractValidator<UpdatePropertyStatusRequest>
{
    public UpdatePropertyStatusRequestValidator()
    {
        RuleFor(request => request.Status)
            .NotEmpty()
            .Must(BeValidStatus)
            .WithMessage("status debe ser Available, Rented o Maintenance.");

        RuleFor(request => request.AdditionalProperties)
            .Must(properties => properties is null or { Count: 0 })
            .WithMessage("El cuerpo solo puede contener la propiedad status.");
    }

    private static bool BeValidStatus(string? status)
        => status is not null
           && Enum.TryParse<PropertyStatus>(status, ignoreCase: true, out _)
           && Enum.IsDefined(typeof(PropertyStatus), Enum.Parse<PropertyStatus>(status, true));
}