namespace HRAPI.Services;

public class ServiceResult
{
    public bool Succeeded { get; protected set; }
    public bool NotFound { get; protected set; }
    public string? ErrorMessage { get; protected set; }

    public static ServiceResult Success() => new() { Succeeded = true };
    public static ServiceResult Failure(string error) => new() { ErrorMessage = error };
    public static ServiceResult Missing() => new() { NotFound = true };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; protected set; }

    public static ServiceResult<T> Success(T data) => new() { Succeeded = true, Data = data };
    public new static ServiceResult<T> Failure(string error) => new() { ErrorMessage = error };
}
