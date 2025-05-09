using Demo.Elasticsearch.Common;
using Demo.Elasticsearch.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Elasticsearch.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToProblemDetails(this Result result, HttpContext httpContext)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Cannot convert success result to problem details");

        var problemDetails = new ProblemDetails
        {
            Status = GetStatusCode(result.Error.Type),
            Title = GetErrorTitle(result.Error.Type),
            Detail = result.Error.Message,
            Type = GetErrorTypeUri(result.Error.Type),
            Instance = httpContext.Request?.Path,
            Extensions = { ["errorCode"] = result.Error.Code },
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }

    private static int GetStatusCode(ErrorType errorCode) => errorCode switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.BadRequest => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.TooManyRequests => StatusCodes.Status429TooManyRequests,
        ErrorType.ServiceUnavailable => StatusCodes.Status503ServiceUnavailable,
        ErrorType.Timeout => StatusCodes.Status504GatewayTimeout,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetErrorTitle(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "Validation Error",
        ErrorType.BadRequest => "Bad Request",
        ErrorType.Unauthorized => "Unauthorized Access",
        ErrorType.Forbidden => "Forbidden Access",
        ErrorType.NotFound => "Resource Not Found",
        ErrorType.Conflict => "Conflict Error",
        ErrorType.TooManyRequests => "Too Many Requests",
        ErrorType.ServiceUnavailable => "Service Unavailable",
        ErrorType.Timeout => "Timeout",
        _ => "Internal Server Error"
    };

    private static string GetErrorTypeUri(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
        ErrorType.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
        ErrorType.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
        ErrorType.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
        ErrorType.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
        ErrorType.TooManyRequests => "https://tools.ietf.org/html/rfc6585#section-4",
        ErrorType.ServiceUnavailable => "https://tools.ietf.org/html/rfc7231#section-6.6.4",
        ErrorType.Timeout => "https://tools.ietf.org/html/rfc7231#section-6.6.5",
        _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
    };
}
