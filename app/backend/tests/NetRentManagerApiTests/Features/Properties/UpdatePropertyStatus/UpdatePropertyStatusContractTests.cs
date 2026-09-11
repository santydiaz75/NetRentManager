using System.Text.Json;

namespace NetRentManagerApiTests.Features.Properties.UpdatePropertyStatus;

public sealed class UpdatePropertyStatusContractTests
{
    [Fact]
    public void Generated_openapi_contains_status_patch_contract()
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "wwwroot",
            "openapi",
            "v1.json");

        if (!File.Exists(path))
        {
            return;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var operation = document.RootElement
            .GetProperty("paths")
            .GetProperty("/api/properties/{id}/status")
            .GetProperty("patch");

        Assert.True(operation.GetProperty("responses").TryGetProperty("200", out _));
        Assert.True(operation.GetProperty("responses").TryGetProperty("400", out _));
        Assert.True(operation.GetProperty("responses").TryGetProperty("404", out _));
        Assert.True(operation.GetProperty("responses").TryGetProperty("500", out _));

        var requestBody = operation.GetProperty("requestBody");
        Assert.True(requestBody.GetProperty("content").TryGetProperty("application/json", out var jsonContent));
        var schema = jsonContent.GetProperty("schema");
        var properties = schema.GetProperty("properties");
        Assert.True(properties.TryGetProperty("status", out _));
        Assert.False(properties.TryGetProperty("imageUrl", out _));
        Assert.False(properties.TryGetProperty("image", out _));
    }
}
