using Elastic.Clients.Elasticsearch.Requests;

namespace Demo.Elasticsearch.Common.Errors;

public static class ProductErrors
{
    public static Error NotFound(int id) =>
        new Error("Product.NotFound", $"The product with Id = {id} was not found.", ErrorType.NotFound);

    public static Error ValidationError(string message) =>
        new Error("Product.Validation", message, ErrorType.Validation);
    
    public static Error InvalidSortField(string sortField) =>
        new Error("Product.Validation", $"Invalid sort field: {sortField}", ErrorType.Validation);

    public static Error Conflict(string message) =>
        new Error("Product.Conflict", message, ErrorType.Conflict);

    public static Error Unauthorized(string message = "You are not authorized to perform this action.") =>
        new Error("Product.Unauthorized", message, ErrorType.Unauthorized);

    public static Error Forbidden(string message = "You are forbidden from performing this action.") =>
        new Error("Product.Forbidden", message, ErrorType.Forbidden);

    public static Error ServerError(string message = "An unexpected error occurred.") =>
        new Error("Product.ServerError", message, ErrorType.ServerError);
}
