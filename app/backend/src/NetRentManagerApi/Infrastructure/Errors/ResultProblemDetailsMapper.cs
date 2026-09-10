using Microsoft.AspNetCore.Mvc;

namespace NetRentManagerApi.Infrastructure.Errors;

public static class ResultProblemDetailsMapper
{
    public static IResult ToIResult<T>(this Result<T> result, Func<T, IResult> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(onSuccess);

        if (result.IsSuccess && result.Value is not null)
        {
            return onSuccess(result.Value);
        }

        var error = result.Error ?? Error.Validation("result.error.missing", "Result failed without error information", new Dictionary<string, string[]>());
        return error.ToIResult();
    }

    public static IResult ToIResult(this Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        if (error.Type == ErrorType.Validation)
        {
            var validationErrors = error.Details ?? new Dictionary<string, string[]>();
            return Results.ValidationProblem(
                errors: validationErrors,
                detail: error.Message,
                statusCode: StatusCodes.Status400BadRequest,
                extensions: new Dictionary<string, object?> { ["code"] = error.Code });
        }

        var problemDetails = error.ToProblemDetails();
        return Results.Problem(problemDetails);
    }

    public static ProblemDetails ToProblemDetails(this Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = error.Type.ToString(),
            Type = $"https://httpstatuses.com/{statusCode}",
            Detail = error.Message
        };

        problemDetails.Extensions["code"] = error.Code;

        if (error.Details is not null && error.Details.Count > 0)
        {
            problemDetails.Extensions["details"] = error.Details;
        }

        return problemDetails;
    }
}