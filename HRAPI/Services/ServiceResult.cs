namespace HRAPI.Services;

// Represents expected service outcomes such as success, validation failure, or missing record.
public class ServiceResult
{
    public bool Succeeded { get; init; }
    public bool NotFound { get; init; }
    public string? ErrorMessage { get; init; }

    public static ServiceResult Success() => new() { Succeeded = true };
    public static ServiceResult Failure(string errorMessage) => new() { ErrorMessage = errorMessage };
    public static ServiceResult Missing() => new() { NotFound = true };
}

// Generic result also carries DTO data for successful create/read-like operations.
public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; init; }

    public static ServiceResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public new static ServiceResult<T> Failure(string errorMessage) => new() { ErrorMessage = errorMessage };
    public new static ServiceResult<T> Missing() => new() { NotFound = true };
}
