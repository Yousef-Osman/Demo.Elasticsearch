namespace Demo.Elasticsearch.Common.Errors;

public enum ErrorType
{
    Failure,
    Validation,
    BadRequest,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    ServerError,
    TooManyRequests,
    ServiceUnavailable,
    Timeout
}
