using FluentValidation.TestHelper;
using NetRentManagerApi.Features.Properties.UpdateProperty;

namespace NetRentManagerApiTests.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyRequestValidatorTests
{
    private readonly UpdatePropertyRequestValidator _validator = new();

    [Fact]
    public void ValidRequestWithoutImage_Passes()
    {
        var result = _validator.TestValidate(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void MissingEditableField_Fails()
    {
        var result = _validator.TestValidate(ValidRequest() with { Title = null });

        result.ShouldHaveValidationErrorFor(request => request.Title);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Unknown")]
    public void MissingOrInvalidStatus_Fails(string? status)
    {
        var result = _validator.TestValidate(ValidRequest() with { Status = status });

        result.ShouldHaveValidationErrorFor(request => request.Status);
    }

    private static UpdatePropertyRequest ValidRequest()
        => new()
        {
            Title = "Title",
            Description = "Description",
            Address = "Address",
            Price = 100,
            Status = "Available",
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 60
        };
}
