using FluentValidation.TestHelper;
using NetRentManagerApi.Features.Properties.UpdatePropertyStatus;

namespace NetRentManagerApiTests.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusRequestValidatorTests
{
    private readonly UpdatePropertyStatusRequestValidator _validator = new();

    [Theory]
    [InlineData("Available")]
    [InlineData("available")]
    [InlineData("RENTED")]
    [InlineData("Maintenance")]
    public void Accepts_valid_status_values_without_case_sensitivity(string status)
    {
        var result = _validator.TestValidate(new UpdatePropertyStatusRequest { Status = status });

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Unknown")]
    public void Rejects_missing_empty_or_invalid_status(string? status)
    {
        var result = _validator.TestValidate(new UpdatePropertyStatusRequest { Status = status });

        result.ShouldHaveValidationErrorFor(request => request.Status);
    }

    [Fact]
    public void Rejects_additional_json_properties()
    {
        var result = _validator.TestValidate(new UpdatePropertyStatusRequest
        {
            Status = "Available",
            AdditionalProperties = new Dictionary<string, System.Text.Json.JsonElement>
            {
                ["imageUrl"] = default
            }
        });

        result.ShouldHaveValidationErrorFor(request => request.AdditionalProperties);
    }
}
