using Microsoft.Extensions.Logging;
using NetRentManagerApi.Infrastructure.Validation;

namespace NetRentManagerApi.Infrastructure.Endpoints;

public static class SliceEndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapSliceEndpoints(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var loggerFactory = app.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("NetRentManagerApi.Infrastructure.Endpoints");

        var group = app.MapGroup(string.Empty);
        group.AddEndpointFilterFactory(ValidationFilterFactory.Create);

        var slices = app.ServiceProvider.GetServices<ISlice>().ToArray();
        logger.LogInformation("Mapping {SliceCount} slices", slices.Length);

        foreach (var slice in slices)
        {
            logger.LogInformation("Mapping slice {SliceType}", slice.GetType().FullName);
            slice.AddEndpoint(group);
        }

        return app;
    }
}