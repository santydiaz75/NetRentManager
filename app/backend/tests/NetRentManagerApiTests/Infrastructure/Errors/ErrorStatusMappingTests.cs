using NetRentManagerApi.Infrastructure.Errors;

namespace NetRentManagerApiTests.Infrastructure.Errors;

public sealed class ErrorStatusMappingTests
{
    [Fact]
    public void UnsupportedMediaTypeError_MapsToProblemDetails415()
    {
        var problemDetails = Error.UnsupportedMediaType(
            "properties.image.unsupported",
            "El formato de image no está permitido.").ToProblemDetails();

        Assert.Equal(StatusCodes.Status415UnsupportedMediaType, problemDetails.Status);
        Assert.Equal("properties.image.unsupported", problemDetails.Extensions["code"]);
    }

    [Fact]
    public void PayloadTooLargeError_MapsToProblemDetails413()
    {
        var problemDetails = Error.PayloadTooLarge(
            "properties.image.too_large",
            "La imagen no puede superar 5 MiB.").ToProblemDetails();

        Assert.Equal(StatusCodes.Status413PayloadTooLarge, problemDetails.Status);
        Assert.Equal("properties.image.too_large", problemDetails.Extensions["code"]);
    }
}