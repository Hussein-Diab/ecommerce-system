using System.Net;
using System.Text.Json;

namespace ECommerce.Client.Web.ApiClients;

public static class ApiResponseReader
{

    public static async Task<ApiResult<T>> SendAsync<T>(Func<Task<HttpResponseMessage>> send)
    {
        try
        {
            using var response = await send();
            return await ReadAsync<T>(response);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            return ApiResult<T>.Fail(503, new() { "The service is currently unavailable. Please try again later." });
        }
    }

    private static async Task<ApiResult<T>> ReadAsync<T>(HttpResponseMessage response)
    {
        var status = (int)response.StatusCode;

        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == HttpStatusCode.NoContent)       
                return ApiResult<T>.Ok(default, status);

            var data = await response.Content.ReadFromJsonAsync<T>();
            return ApiResult<T>.Ok(data, status);
        }

        return ApiResult<T>.Fail(status, await ReadErrorsAsync(response));
    }

    private static async Task<List<string>> ReadErrorsAsync(HttpResponseMessage response)
    {
        var errors = new List<string>();
        try
        {
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var root = doc.RootElement;

            // Shape 1: { "errors": { "Email": ["..."], "Password": ["..."] } }
            if (root.TryGetProperty("errors", out var errorsObj) && errorsObj.ValueKind == JsonValueKind.Object)
            {
                foreach (var field in errorsObj.EnumerateObject())
                    foreach (var message in field.Value.EnumerateArray())
                        errors.Add(message.GetString() ?? string.Empty);
            }
            // Shape 2: { "message": "..." }
            else if (root.TryGetProperty("message", out var msg))
            {
                errors.Add(msg.GetString() ?? string.Empty);
            }
        }
        catch (JsonException)
        {
            // empty or non-JSON body: fall through to the generic message
        }

        if (errors.Count == 0)
            errors.Add($"Something went wrong (error {(int)response.StatusCode}).");

        return errors;
    }
}