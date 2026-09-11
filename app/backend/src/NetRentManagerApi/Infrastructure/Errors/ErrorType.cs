namespace NetRentManagerApi.Infrastructure.Errors;

public enum ErrorType
{
    Validation,
    UnsupportedMediaType,
    PayloadTooLarge,
    NotFound,
    Conflict,
    Forbidden,
    Internal
}