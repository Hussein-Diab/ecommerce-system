namespace ECommerce.Client.Api.Services.Common;

public enum ServiceErrorType
{
    None,
    Validation,  
    NotFound      
}

public class ServiceResult<T>
{
    public bool Success { get; private init; }
    public T? Data { get; private init; }
    public string? Error { get; private init; }
    public ServiceErrorType ErrorType { get; private init; }

    public static ServiceResult<T> Ok(T data) =>
        new() { Success = true, Data = data };

    public static ServiceResult<T> Fail(ServiceErrorType type, string error) =>
        new() { Success = false, ErrorType = type, Error = error };
}