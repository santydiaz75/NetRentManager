using FluentValidation.TestHelper;
using NetRentManagerApi.Features.Properties.GetPropertyById;

namespace NetRentManagerApiTests.Features.Properties.GetPropertyById;

public sealed class GetPropertyByIdRequestValidatorTests
{
    [Fact]
    public void ValidGuid_PassesWithoutBusinessValidationErrors()
    {
        var validator = new GetPropertyByIdRequestValidator();

        var result = validator.TestValidate(new GetPropertyByIdRequest(Guid.NewGuid()));

        result.ShouldNotHaveAnyValidationErrors();
    }
}