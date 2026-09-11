using NetRentManagerApi.Features.Properties.CreateProperty;

namespace NetRentManagerApiTests.Features.Properties.CreateProperty;

public sealed class CreatePropertyErrorTests
{
    [Fact]
    public void EmptyImage_IsRejectedByValidator()
    {
        var validator = new CreatePropertyRequestValidator();
        var request = new CreatePropertyRequest
        {
            Title = "Title",
            Description = "Description",
            Address = "Address",
            Price = 100,
            Status = "Available",
            BedroomCount = 1,
            BathroomCount = 1,
            AreaSquareMeters = 40,
            Image = new FormFile(new MemoryStream(), 0, 0, "image", "empty.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            }
        };

        var result = validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Image");
    }
}
