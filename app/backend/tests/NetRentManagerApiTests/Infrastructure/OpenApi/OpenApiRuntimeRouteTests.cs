using System.Text.Json;

namespace NetRentManagerApiTests.Infrastructure.OpenApi;

public sealed class OpenApiRuntimeRouteTests
{
    [Fact]
    public void VersionedDocumentExistsAndContainsJsonContract()
    {
        var path = DocumentPath();

        Assert.True(File.Exists(path), $"OpenAPI document was not found at {path}.");
        using var document = JsonDocument.Parse(File.ReadAllText(path));

        Assert.Equal("3.1.1", document.RootElement.GetProperty("openapi").GetString());
        Assert.True(document.RootElement.TryGetProperty("paths", out _));
    }

    [Fact]
    public void NoInteractiveDocumentationPathsAreDeclared()
    {
        using var document = JsonDocument.Parse(File.ReadAllText(DocumentPath()));
        var paths = document.RootElement.GetProperty("paths").EnumerateObject().Select(path => path.Name).ToArray();

        Assert.DoesNotContain(paths, path => path.Contains("swagger", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(paths, path => path.Contains("redoc", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(paths, path => path.Contains("scalar", StringComparison.OrdinalIgnoreCase));
    }

    private static string DocumentPath()
        => Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../../../src/NetRentManagerApi/wwwroot/openapi/v1.json"));
}
