using FluentValidation.TestHelper;
using NetRentManagerApi.Domain.Properties;
using NetRentManagerApi.Features.Properties.CreateProperty;

namespace NetRentManagerApiTests.Features.Properties.CreateProperty;

public sealed class CreatePropertyRequestValidatorTests
{
    private readonly CreatePropertyRequestValidator _validator = new();

    [Fact]
    public void ValidRequestWithoutImage_Passes()
    {
        var result = _validator.TestValidate(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
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

    private static CreatePropertyRequest ValidRequest()
        => new()
        {
            Title = "Title",
            Description = "Description",
            Address = "Address",
            Price = 100,
            Status = PropertyStatus.Available.ToString(),
            BedroomCount = 2,
            BathroomCount = 1,
            AreaSquareMeters = 60
        };
}
