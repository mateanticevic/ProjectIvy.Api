using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProjectIvy.Api.EndToEnd.Test.Infrastructure;

public sealed class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "E2E";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-E2E-Email", out var email))
            return Task.FromResult(AuthenticateResult.NoResult());

        var claims = new List<Claim> { new(ClaimTypes.Email, email.ToString()) };
        if (Request.Headers.TryGetValue("X-E2E-Scope", out var scope))
            claims.Add(new Claim("scope", scope.ToString()));
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SchemeName));
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName)));
    }
}
