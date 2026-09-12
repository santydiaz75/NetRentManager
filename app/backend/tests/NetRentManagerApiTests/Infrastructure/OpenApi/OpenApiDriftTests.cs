using System.Text.Json;
using NetRentManagerApi.Infrastructure.DependencyInjection;
using NetRentManagerApi.Infrastructure.Endpoints;

namespace NetRentManagerApiTests.Infrastructure.OpenApi;

public sealed class OpenApiDriftTests
{
    [Fact]
    public void RuntimeEndpointsMatchGeneratedDocument()
    {
        using var app = CreateApplication();
        var actual = GetRuntimeInventory(app);
        var document = LoadDocument();
        var documented = GetDocumentInventory(document);

        Assert.Equal(documented.OrderBy(item => item), actual.OrderBy(item => item));
    }

    [Fact]
    public void InducedDriftIsDetected()
    {
        using var app = CreateApplication();
        var actual = GetRuntimeInventory(app);
        var documented = GetDocumentInventory(LoadDocument());
        documented.Add("GET /api/properties/{induced}");

        Assert.NotEqual(documented.OrderBy(item => item), actual.OrderBy(item => item));
    }

    [Fact]
    public void RuntimeHasNoInteractiveDocumentationRoutes()
    {
        using var app = CreateApplication();
        var routes = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.RoutePattern.RawText)
            .Where(route => route is not null)
            .ToArray();

        Assert.DoesNotContain(routes, route => route!.Contains("swagger", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(routes, route => route!.Contains("redoc", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(routes, route => route!.Contains("scalar", StringComparison.OrdinalIgnoreCase));
    }

    private static WebApplication CreateApplication()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["ConnectionStrings:DefaultConnection"] =
            "Host=localhost;Port=5432;Database=netrentmanager_openapi_tests";
        builder.Configuration["DatabaseCredentials:Username"] = "postgres";
        builder.Configuration["DatabaseCredentials:Password"] = "postgres";
        builder.Services.AddInfrastructure(builder.Configuration, typeof(Program).Assembly);
        var app = builder.Build();
        app.MapSliceEndpoints();
        return app;
    }

    private static HashSet<string> GetRuntimeInventory(IEndpointRouteBuilder app)
        => app.DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .SelectMany(endpoint => endpoint.Metadata.OfType<IHttpMethodMetadata>()
                .SelectMany(metadata => metadata.HttpMethods.Select(method =>
                    $"{method} {Normalize(((RouteEndpoint)endpoint).RoutePattern.RawText!)}")))
            .Where(entry => !entry.EndsWith(" /openapi/v1.json", StringComparison.Ordinal))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static HashSet<string> GetDocumentInventory(JsonDocument document)
        => document.RootElement.GetProperty("paths")
            .EnumerateObject()
            .SelectMany(path => path.Value.EnumerateObject().Select(operation =>
                $"{operation.Name.ToUpperInvariant()} {Normalize(path.Name)}"))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static JsonDocument LoadDocument()
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "../../../../../src/NetRentManagerApi/wwwroot/openapi/v1.json"));
        Assert.True(File.Exists(path), $"OpenAPI document was not found at {path}.");
        return JsonDocument.Parse(File.ReadAllText(path));
    }

    private static string Normalize(string route)
        => System.Text.RegularExpressions.Regex.Replace(route, "\\{([^}:]+):[^}]+\\}", "{$1}");
}
