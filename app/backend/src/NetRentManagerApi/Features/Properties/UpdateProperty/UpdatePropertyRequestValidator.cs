using FluentValidation;
using NetRentManagerApi.Domain.Properties;

namespace NetRentManagerApi.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyRequestValidator : AbstractValidator<UpdatePropertyRequest>
{
    public UpdatePropertyRequestValidator()
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
    }
}
