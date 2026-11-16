namespace ECommerce.Client.Web.ApiClients;

public class ApiResult<T>
{
    public bool Success { get; private init; }
    public T? Data { get; private init; }
    public int StatusCode { get; private init; }
    public List<string> Errors { get; private init; } = new();

    public string ErrorMessage => string.Join(" ", Errors);
    public bool IsUnauthorized => StatusCode == 401;

    public static ApiResult<T> Ok(T? data, int statusCode) =>
        new() { Success = true, Data = data, StatusCode = statusCode };

    public static ApiResult<T> Fail(int statusCode, List<string> errors) =>
        new() { Success = false, StatusCode = statusCode, Errors = errors };
}