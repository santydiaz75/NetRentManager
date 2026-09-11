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

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration, typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";
        options.SwaggerEndpoint("/openapi/v1.json", "NetRentManagerApi v1");
    });
}

app.MapSliceEndpoints();

await app.MigrateAsync();

app.Run();

public partial class Program;
