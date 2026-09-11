using FluentValidation;

namespace NetRentManagerApi.Features.Properties.ListProperties;

public sealed class ListPropertiesRequestValidator : AbstractValidator<ListPropertiesRequest>
{
    public ListPropertiesRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("page debe ser mayor o igual que 1.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("pageSize debe estar entre 1 y 100.");
    }
}
