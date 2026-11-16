using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace ECommerce.Client.Web.Handlers;

// A DelegatingHandler is "middleware for HttpClient": it runs on every outgoing request
public class BearerTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _accessor;

    public BearerTokenHandler(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _accessor.HttpContext;

        if (httpContext is not null)
        {
            // The JWT stored inside the login cookie by AccountController.SignInWithTokenAsync
            var token = await httpContext.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);   // pass the request on
    }
}