namespace ECommerce.Client.Api.Services;

public record TokenResult(string Token, DateTime ExpiresAtUtc);