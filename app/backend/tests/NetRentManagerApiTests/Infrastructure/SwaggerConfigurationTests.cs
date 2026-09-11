namespace NetRentManagerApiTests.Infrastructure;

public sealed class SwaggerConfigurationTests
{
    [Fact]
    public void Program_Registers_SwaggerUi_Only_In_Development_Block()
    {
        var programPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerApi/Program.cs"));

        var source = File.ReadAllText(programPath);
        var developmentIndex = source.IndexOf("if (app.Environment.IsDevelopment())", StringComparison.Ordinal);
        var mapOpenApiIndex = source.IndexOf("app.MapOpenApi();", StringComparison.Ordinal);
        var swaggerUiIndex = source.IndexOf("app.UseSwaggerUI(", StringComparison.Ordinal);
        var mapSliceEndpointsIndex = source.IndexOf("app.MapSliceEndpoints();", StringComparison.Ordinal);

        Assert.Contains("builder.Services.AddOpenApi();", source, StringComparison.Ordinal);
        Assert.True(developmentIndex >= 0, "Program.cs debe condicionar OpenAPI/Swagger al entorno Development.");
        Assert.True(mapOpenApiIndex > developmentIndex, "Program.cs debe mapear OpenAPI dentro del bloque Development.");
        Assert.True(swaggerUiIndex > mapOpenApiIndex, "Swagger UI debe registrarse dentro del bloque Development y después de OpenAPI.");
        Assert.True(mapSliceEndpointsIndex > swaggerUiIndex, "La configuración de Swagger UI debe completarse antes del mapeo centralizado de slices.");
    }

    [Fact]
    public void Program_Configures_SwaggerUi_To_Use_The_Published_OpenApi_Document()
    {
        var programPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../../src/NetRentManagerApi/Program.cs"));

        var source = File.ReadAllText(programPath);

        Assert.Contains("options.RoutePrefix = \"swagger\";", source, StringComparison.Ordinal);
        Assert.Contains("options.SwaggerEndpoint(\"/openapi/v1.json\", \"NetRentManagerApi v1\");", source, StringComparison.Ordinal);
        Assert.DoesNotContain("MapControllers", source, StringComparison.Ordinal);
    }
}
