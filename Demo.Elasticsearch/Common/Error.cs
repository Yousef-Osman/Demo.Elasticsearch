namespace Demo.Elasticsearch.Common;

public class Error
{
    public string Code { get; }
    public string Message { get; }

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public static readonly Error None = new Error(string.Empty, string.Empty);

    public static Error Validation(string message) => new Error("Validation", message);
    public static Error BadRequest(string message) => new Error("BadRequest", message);
    public static Error NotFound(string message = "Resource Not Found") => new Error("NotFound", message);
    public static Error Unauthorized(string message = "You are not authorized to perform this action.") => new Error("Unauthorized", message);
    public static Error ServerError(string message = "An unexpected error occurred.") => new Error("ServerError", message);
}
