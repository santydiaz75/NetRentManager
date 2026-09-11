using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace NetRentManagerApiTests.Infrastructure.Swagger;

public sealed class SwaggerUiRuntimeTests
{
    [Fact]
    public async Task Development_serves_swagger_ui_and_openapi_document()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseSetting(WebHostDefaults.EnvironmentKey, "Development"));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var swaggerResponse = await client.GetAsync("/swagger");
        using var indexResponse = await client.GetAsync("/swagger/index.html");
        using var openApiResponse = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.MovedPermanently, swaggerResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, indexResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, openApiResponse.StatusCode);
        Assert.Equal("text/html", indexResponse.Content.Headers.ContentType?.MediaType);
        Assert.Equal("application/json", openApiResponse.Content.Headers.ContentType?.MediaType);

        var html = await indexResponse.Content.ReadAsStringAsync();
        var openApiJson = await openApiResponse.Content.ReadAsStringAsync();
        using var openApiDocument = JsonDocument.Parse(openApiJson);

        Assert.Contains("/openapi/v1.json", html, StringComparison.Ordinal);
        Assert.Contains("NetRentManagerApi", html, StringComparison.Ordinal);
        Assert.Equal("/", openApiDocument.RootElement.GetProperty("servers")[0].GetProperty("url").GetString());
        Assert.DoesNotContain("http://localhost:5065", openApiJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Production_does_not_serve_swagger_ui_or_its_assets()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseSetting(WebHostDefaults.EnvironmentKey, "Production"));
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        foreach (var path in new[]
        {
            "/swagger",
            "/swagger/index.html",
            "/swagger/swagger-ui.css",
            "/swagger/swagger-ui-bundle.js",
            "/swagger/swagger-ui-standalone-preset.js"
        })
        {
            using var response = await client.GetAsync(path);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
