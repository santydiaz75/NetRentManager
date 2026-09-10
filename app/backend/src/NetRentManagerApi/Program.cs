using NetRentManagerApi.Infrastructure.DependencyInjection;
using NetRentManagerApi.Infrastructure.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapSliceEndpoints();

app.Run();

public partial class Program;
