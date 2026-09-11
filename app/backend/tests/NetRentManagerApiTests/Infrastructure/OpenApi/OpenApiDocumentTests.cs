using System.Text.Json;

namespace NetRentManagerApiTests.Infrastructure.OpenApi;

public sealed class OpenApiDocumentTests
{
    [Fact]
    public void DocumentContainsExpectedOperations()
    {
        using var document = LoadDocument();
        var paths = document.RootElement.GetProperty("paths");

        Assert.Contains("/health", paths.EnumerateObject().Select(item => item.Name));
        Assert.Contains("/api/properties", paths.EnumerateObject().Select(item => item.Name));
        Assert.Contains("/api/properties/{id}", paths.EnumerateObject().Select(item => item.Name));
        Assert.True(paths.GetProperty("/health").TryGetProperty("get", out _));
        Assert.True(paths.GetProperty("/api/properties").TryGetProperty("get", out _));
        Assert.True(paths.GetProperty("/api/properties").TryGetProperty("post", out _));
        Assert.True(paths.GetProperty("/api/properties/{id}").TryGetProperty("get", out _));
        Assert.True(paths.GetProperty("/api/properties/{id}").TryGetProperty("put", out _));
    }

    [Fact]
    public void DocumentContainsStableSharedSchemasAndReferences()
    {
        using var document = LoadDocument();
        var schemas = document.RootElement.GetProperty("components").GetProperty("schemas");

        Assert.True(schemas.TryGetProperty("PropertyStatus", out _));
        Assert.True(schemas.TryGetProperty("ProblemDetails", out _));
        Assert.True(schemas.TryGetProperty("HttpValidationProblemDetails", out _));
        Assert.Equal(
            "#/components/schemas/PropertyStatus",
            schemas.GetProperty("CreatePropertyResponse").GetProperty("properties").GetProperty("status").GetProperty("$ref").GetString());
        Assert.Equal(
            "#/components/schemas/ProblemDetails",
            document.RootElement.GetProperty("paths").GetProperty("/api/properties/{id}").GetProperty("get").GetProperty("responses").GetProperty("404").GetProperty("content").GetProperty("application/problem+json").GetProperty("schema").GetProperty("$ref").GetString());
    }

    [Fact]
    public void DocumentContainsExpectedResponseMatrix()
    {
        using var document = LoadDocument();
        var paths = document.RootElement.GetProperty("paths");

        Assert.Contains("200", paths.GetProperty("/health").GetProperty("get").GetProperty("responses").EnumerateObject().Select(item => item.Name));
        Assert.Contains("200", paths.GetProperty("/api/properties").GetProperty("get").GetProperty("responses").EnumerateObject().Select(item => item.Name));
        Assert.Contains("400", paths.GetProperty("/api/properties").GetProperty("get").GetProperty("responses").EnumerateObject().Select(item => item.Name));
        Assert.Contains("201", paths.GetProperty("/api/properties").GetProperty("post").GetProperty("responses").EnumerateObject().Select(item => item.Name));
        Assert.Contains("500", paths.GetProperty("/api/properties").GetProperty("post").GetProperty("responses").EnumerateObject().Select(item => item.Name));
        Assert.Contains("413", paths.GetProperty("/api/properties/{id}").GetProperty("put").GetProperty("responses").EnumerateObject().Select(item => item.Name));
        Assert.Contains("415", paths.GetProperty("/api/properties/{id}").GetProperty("put").GetProperty("responses").EnumerateObject().Select(item => item.Name));
    }

    [Fact]
    public void MultipartOperationsDescribeImageField()
    {
        using var document = LoadDocument();
        var paths = document.RootElement.GetProperty("paths");
        var create = paths.GetProperty("/api/properties").GetProperty("post");
        var update = paths.GetProperty("/api/properties/{id}").GetProperty("put");

        Assert.True(HasImageProperty(create));
        Assert.True(HasImageProperty(update));
    }

    private static bool HasImageProperty(JsonElement operation)
    {
        var schema = operation.GetProperty("requestBody")
            .GetProperty("content")
            .GetProperty("multipart/form-data")
            .GetProperty("schema");
        return schema.GetProperty("$ref").GetString() is not null;
    }

    private static JsonDocument LoadDocument()
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../../../src/NetRentManagerApi/wwwroot/openapi/v1.json"));
        Assert.True(File.Exists(path), $"OpenAPI document was not found at {path}.");
        return JsonDocument.Parse(File.ReadAllText(path));
    }
}
