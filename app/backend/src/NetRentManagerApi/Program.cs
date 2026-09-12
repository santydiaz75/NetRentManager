using Microsoft.OpenApi;
using System.Text;
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
            Url = "/",
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

        if (document.Paths is not null)
        {
            var orderedPaths = document.Paths
                .OrderBy(path => path.Key, StringComparer.Ordinal)
                .ToArray();

            document.Paths.Clear();
            foreach (var (path, pathItem) in orderedPaths)
            {
                document.Paths.Add(path, pathItem);
            }

            foreach (var pathItem in document.Paths.Values)
            {
                var operations = pathItem.Operations;
                if (operations is null || operations.Count == 0)
                {
                    continue;
                }

                var orderedOperations = operations
                    .OrderBy(
                        operation => operation.Key.Method.ToLowerInvariant(),
                        StringComparer.Ordinal)
                    .ToArray();

                operations.Clear();
                foreach (var (operationType, operation) in orderedOperations)
                {
                    operations.Add(operationType, operation);
                }
            }
        }

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
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration, typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "NetRentManagerApi v1");
        options.ConfigObject.Urls =
        [
            new Swashbuckle.AspNetCore.SwaggerUI.UrlDescriptor
            {
                Url = "/openapi/v1.json",
                Name = "NetRentManagerApi v1"
            }
        ];
        options.IndexStream = () => new MemoryStream(Encoding.UTF8.GetBytes("""
            <!doctype html>
            <html lang="es">
            <head>
                <meta charset="utf-8" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <title>NetRentManagerApi - Swagger UI</title>
                <link rel="stylesheet" href="./swagger-ui.css" />
            </head>
            <body>
                <div id="swagger-ui"></div>
                <script src="./swagger-ui-bundle.js"></script>
                <script src="./swagger-ui-standalone-preset.js"></script>
                <script>
                    window.onload = function () {
                        window.ui = SwaggerUIBundle({
                            url: "/openapi/v1.json",
                            dom_id: "#swagger-ui",
                            deepLinking: true,
                            presets: [
                                SwaggerUIBundle.presets.apis,
                                SwaggerUIStandalonePreset
                            ],
                            layout: "StandaloneLayout"
                        });
                    };
                </script>
            </body>
            </html>
            """));
    });
}

app.UseStaticFiles();

app.MapSliceEndpoints();

await app.MigrateAsync();

app.Run();

public partial class Program;
