using FluentValidation;

namespace NetRentManagerApiTests.TestTypes;

public sealed record ValidatedRequest(string Name);

public sealed record NoValidatorRequest(string Value);

public sealed class SpyValidatedRequestValidator : AbstractValidator<ValidatedRequest>
{
    public CancellationToken LastToken { get; private set; }

    public SpyValidatedRequestValidator()
    {
        RuleFor(x => x.Name)
            .CustomAsync((value, context, cancellationToken) =>
            {
                LastToken = cancellationToken;
                if (string.IsNullOrWhiteSpace(value))
                {
                    context.AddFailure(nameof(ValidatedRequest.Name), "Name is required");
                    context.AddFailure(nameof(ValidatedRequest.Name), "Name cannot be empty");
                }

                return Task.CompletedTask;
            });
    }
}

public static class TestEndpointMethods
{
    public static Task<IResult> ValidatedEndpoint(ValidatedRequest request, CancellationToken cancellationToken)
        => Task.FromResult(Results.Ok(request.Name));

    public static Task<IResult> NoValidatorEndpoint(NoValidatorRequest request)
        => Task.FromResult(Results.Ok(request.Value));
}