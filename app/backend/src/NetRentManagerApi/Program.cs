using Microsoft.OpenApi;
using System.Text.Json.Nodes;
using NetRentManagerApi.Infrastructure.DependencyInjection;
using NetRentManagerApi.Infrastructure.Endpoints;
using NetRentManagerApi.Infrastructure.Persistence;

var runtimeWebRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
Directory.CreateDirectory(runtimeWebRoot);
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = runtimeWebRoot
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        var components = document.Components ??= new OpenApiComponents();
        components.Schemas ??= new Dictionary<string, IOpenApiSchema>();
        components.Schemas["PropertyStatus"] = new OpenApiSchema
        {
            Type = JsonSchemaType.String,
            Enum = [
                JsonValue.Create("Available"),
                JsonValue.Create("Rented"),
                JsonValue.Create("Maintenance")
            ]
        };

        document.Servers = [new OpenApiServer
        {
            Url = "http://localhost:5065",
            Description = "Entorno local"
        }];
        document.Info.License = new OpenApiLicense
        {
            Name = "Uso interno de NetRentManagerApi"
        };
        document.Tags = new HashSet<OpenApiTag>
        {
            new OpenApiTag
            {
                Name = "NetRentManagerApi",
                Description = "Operaciones de salud del servicio"
            },
            new OpenApiTag
            {
                Name = "Properties",
                Description = "Operaciones públicas de propiedades"
            }
        };

        foreach (var schema in components.Schemas.Values)
        {
            if (schema.Properties is not null
                && schema.Properties.TryGetValue("status", out var status)
                && status is OpenApiSchema { Type: JsonSchemaType.String })
            {
                schema.Properties["status"] = new OpenApiSchemaReference(
                    "PropertyStatus",
                    document,
                    null);
            }
        }

        return Task.CompletedTask;
    });
});
builder.Services.AddInfrastructure(builder.Configuration, typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStaticFiles();

app.MapSliceEndpoints();

await app.MigrateAsync();

app.Run();

public partial class Program;
