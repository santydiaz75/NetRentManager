using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetRentManagerApi.Infrastructure.Errors;

public static class ProblemDetailsServiceExtensions
{
    public static IServiceCollection AddNetRentManagerProblemDetails(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

                var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger("NetRentManagerApi.Infrastructure.ProblemDetails");
                if (logger is null)
                {
                    return;
                }

                var statusCode = context.ProblemDetails.Status ?? context.HttpContext.Response.StatusCode;
                if (statusCode >= StatusCodes.Status500InternalServerError)
                {
                    logger.LogError(
                        "ProblemDetails generated with status {StatusCode} for {Method} {Path}",
                        statusCode,
                        context.HttpContext.Request.Method,
                        context.HttpContext.Request.Path.Value);
                }
                else
                {
                    logger.LogWarning(
                        "ProblemDetails generated with status {StatusCode} for {Method} {Path}",
                        statusCode,
                        context.HttpContext.Request.Method,
                        context.HttpContext.Request.Path.Value);
                }
            };
        });

        return services;
    }
}