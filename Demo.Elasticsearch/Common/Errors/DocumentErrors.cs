namespace Demo.Elasticsearch.Common.Errors;

public static class DocumentErrors
{
    public static Error NotFound() =>
        new Error("Document.NotFound", $"The requested document was not found", ErrorType.NotFound);

    public static Error ServerError(string message = "An unexpected error occurred.") =>
        new Error("Document.ServerError", message, ErrorType.ServerError);
}
