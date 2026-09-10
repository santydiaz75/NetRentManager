using System.Text.Json;
using NetRentManagerApi.Infrastructure.DependencyInjection;
using NetRentManagerApi.Infrastructure.Endpoints;

namespace NetRentManagerApiTests.Infrastructure.Endpoints;

public class HealthSliceTests
{
    [Fact]
    public async Task MapSliceEndpoints_Includes_HealthSlice_And_Returns200()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration["ConnectionStrings:DefaultConnection"] =
            "Host=localhost;Port=5432;Database=netrentmanager_tests;Username=postgres;Password=postgres";
        builder.Services.AddInfrastructure(builder.Configuration, typeof(Program).Assembly);

        await using var app = builder.Build();
        app.MapSliceEndpoints();

        var routeBuilder = (IEndpointRouteBuilder)app;
        var endpoint = routeBuilder
            .DataSources
            .SelectMany(dataSource => dataSource.Endpoints)
            .OfType<RouteEndpoint>()
            .Single(x => (x.DisplayName?.Contains("/health", StringComparison.Ordinal) ?? false)
                         || string.Equals(x.RoutePattern.RawText, "/health", StringComparison.Ordinal));

        var httpContext = new DefaultHttpContext
        {
            RequestServices = app.Services,
            Response = { Body = new MemoryStream() }
        };

        Assert.NotNull(endpoint.RequestDelegate);
        await endpoint.RequestDelegate(httpContext);

        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
        httpContext.Response.Body.Position = 0;
        var payload = await JsonDocument.ParseAsync(httpContext.Response.Body);
        Assert.Equal("healthy", payload.RootElement.GetProperty("status").GetString());
    }
}