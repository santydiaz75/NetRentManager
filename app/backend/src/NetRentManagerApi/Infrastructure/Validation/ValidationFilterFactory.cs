using System.Reflection;
using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace NetRentManagerApi.Infrastructure.Validation;

public static class ValidationFilterFactory
{
    public static EndpointFilterDelegate Create(EndpointFilterFactoryContext context, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        return CreateForMethod(context.MethodInfo, next);
    }

    public static EndpointFilterDelegate CreateForMethod(MethodInfo methodInfo, EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(methodInfo);
        ArgumentNullException.ThrowIfNull(next);

        var validatableParameter = methodInfo
            .GetParameters()
            .Select((parameter, index) => new { parameter, index })
            .FirstOrDefault(x => IsValidatableParameter(x.parameter));

        if (validatableParameter is null)
        {
            return next;
        }

        var parameterType = validatableParameter.parameter.ParameterType;
        var validatorType = typeof(IValidator<>).MakeGenericType(parameterType);

        return async invocationContext =>
        {
            var services = invocationContext.HttpContext.RequestServices;
            var isServiceProvider = services.GetService<IServiceProviderIsService>();
            var validatorExists = isServiceProvider?.IsService(validatorType) ?? services.GetService(validatorType) is not null;

            if (!validatorExists)
            {
                return await next(invocationContext);
            }

            var argument = invocationContext.Arguments[validatableParameter.index];
            if (argument is null)
            {
                return await next(invocationContext);
            }

            if (services.GetService(validatorType) is not IValidator validator)
            {
                return await next(invocationContext);
            }

            var cancellationToken = ResolveCancellationToken(invocationContext, methodInfo);
            var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument), cancellationToken);

            if (validationResult.IsValid)
            {
                return await next(invocationContext);
            }

            var errors = ToErrorDictionary(validationResult.Errors);
            return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
        };
    }

    private static bool IsValidatableParameter(ParameterInfo parameter)
    {
        return parameter.ParameterType.IsClass
               && parameter.ParameterType != typeof(string)
               && !IsInfrastructureParameter(parameter.ParameterType);
    }

    private static bool IsInfrastructureParameter(Type type)
    {
        return type == typeof(HttpContext)
               || type == typeof(HttpRequest)
               || type == typeof(HttpResponse)
               || type == typeof(CancellationToken)
               || type == typeof(ClaimsPrincipal)
               || type == typeof(IServiceProvider);
    }

    private static CancellationToken ResolveCancellationToken(
        EndpointFilterInvocationContext invocationContext,
        MethodInfo methodInfo)
    {
        var cancellationIndex = methodInfo
            .GetParameters()
            .Select((parameter, index) => new { parameter, index })
            .FirstOrDefault(x => x.parameter.ParameterType == typeof(CancellationToken))
            ?.index;

        if (cancellationIndex.HasValue && invocationContext.Arguments[cancellationIndex.Value] is CancellationToken token)
        {
            return token;
        }

        return invocationContext.HttpContext.RequestAborted;
    }

    private static Dictionary<string, string[]> ToErrorDictionary(IEnumerable<ValidationFailure> failures)
    {
        return failures
            .GroupBy(
                failure => string.IsNullOrWhiteSpace(failure.PropertyName) ? "request" : failure.PropertyName,
                StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).Distinct(StringComparer.Ordinal).ToArray(),
                StringComparer.Ordinal);
    }
}