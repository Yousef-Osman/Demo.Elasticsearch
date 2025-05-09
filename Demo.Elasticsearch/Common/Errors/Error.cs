namespace Demo.Elasticsearch.Common.Errors;

public class Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    internal Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public static readonly Error None = new Error(string.Empty, string.Empty, ErrorType.Failure);

    public static Error Validation(string code, string message) => new Error(code, message, ErrorType.Validation);
    public static Error BadRequest(string code, string message) => new Error(code, message, ErrorType.BadRequest);
    public static Error NotFound(string code, string message) => new Error(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) => new Error(code, message, ErrorType.Conflict);
    public static Error Unauthorized(string code, string message) => new Error(code, message, ErrorType.Unauthorized);
    public static Error Forbidden(string code, string message) => new Error(code, message, ErrorType.Forbidden);
    public static Error ServerError(string code, string message) => new Error(code, message, ErrorType.ServerError);
}
