using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using ProjectIvy.Api.EndToEnd.Test.Infrastructure;
using Xunit;

namespace ProjectIvy.Api.EndToEnd.Test;

public sealed class McpAuthenticationTests
{
    private const string Resource = "https://localhost/mcp";
    private const string Issuer = "https://issuer.example.test/realms/ivy";
    private static readonly SymmetricSecurityKey Key = new(new byte[64].Select((_, i) => (byte)(i + 1)).ToArray());

    private sealed class Factory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("EndToEnd");
            builder.ConfigureTestServices(services =>
            {
                foreach (var scheme in new[] { "Bearer", "McpBearer" })
                    services.PostConfigure<JwtBearerOptions>(scheme, options =>
            {
                options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(new()
                {
                    Issuer = Issuer,
                    SigningKeys = { Key }
                });
                options.TokenValidationParameters.ValidIssuer = Issuer;
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;
                    });
            });
        }
    }

    private static string Token(string? audience = Resource, string scope = "expense:user", bool expired = false, bool email = true, string issuer = Issuer)
    {
        var claims = new List<Claim> { new("scope", scope) };
        if (email) claims.Add(new("email", "unmapped@example.test"));
        var jwt = new JwtSecurityToken(issuer, audience, claims,
            DateTime.UtcNow.AddHours(-2), expired ? DateTime.UtcNow.AddHours(-1) : DateTime.UtcNow.AddHours(1),
            new SigningCredentials(Key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    [Theory]
    [InlineData("/expense", "https://existing-rest-client.example.test", true, true)]
    [InlineData("/mcp", "https://mcp-client.example.test", true, false)]
    [InlineData("/mcp", "https://untrusted.example.test", false, false)]
    public async Task CorsPreflightKeepsRestCompatibilityAndMcpOriginRestrictions(string path, string origin, bool allowed, bool credentials)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, path);
        request.Headers.Add("Origin", origin);
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "authorization,content-type");
        using var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(allowed, response.Headers.Contains("Access-Control-Allow-Origin"));
        if (allowed) Assert.Equal(origin, response.Headers.GetValues("Access-Control-Allow-Origin").Single());
        Assert.Equal(credentials, response.Headers.Contains("Access-Control-Allow-Credentials"));
    }

    [Fact]
    public async Task RestUnauthorizedResponseIncludesCorsHeaders()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("Origin", "https://existing-rest-client.example.test");
        using var response = await client.GetAsync("/expense");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("https://existing-rest-client.example.test", response.Headers.GetValues("Access-Control-Allow-Origin").Single());
        Assert.Equal("true", response.Headers.GetValues("Access-Control-Allow-Credentials").Single());
    }

    [Theory]
    [InlineData("http://localhost")]
    [InlineData("http://internal-api:8080")]
    [InlineData("https://localhost")]
    public async Task DiscoveryAndAnonymousChallengeAreAvailableWithoutDatabaseAccess(string baseAddress)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri(baseAddress) });
        using var metadata = await client.GetAsync("/.well-known/oauth-protected-resource/mcp");
        Assert.Equal(HttpStatusCode.OK, metadata.StatusCode);
        var json = JsonDocument.Parse(await metadata.Content.ReadAsStringAsync()).RootElement;
        Assert.Equal(Resource, json.GetProperty("resource").GetString());
        Assert.Equal(new[] { "expense:user", "beer:user" },
            json.GetProperty("scopes_supported").EnumerateArray().Select(x => x.GetString()));
        using var response = await client.PostAsJsonAsync("/mcp", new { });
        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, await response.Content.ReadAsStringAsync());
        Assert.Contains("resource_metadata=\"https://localhost/.well-known/oauth-protected-resource/mcp\"", response.Headers.WwwAuthenticate.ToString());
    }

    [Theory]
    [InlineData("account")]
    [InlineData(null)]
    public async Task RestAcceptsExistingTokenAudienceWhileMcpRejectsIt(string? audience)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(audience: audience, scope: ""));
        using var rest = await client.GetAsync("/expense");
        // An authenticated user without the expense scope is forbidden before database access.
        Assert.True(rest.StatusCode == HttpStatusCode.Forbidden, await rest.Content.ReadAsStringAsync());
        using var mcp = await client.PostAsJsonAsync("/mcp", new { });
        Assert.Equal(HttpStatusCode.Unauthorized, mcp.StatusCode);
    }

    [Theory]
    [InlineData("expired")]
    [InlineData("audience")]
    [InlineData("issuer")]
    [InlineData("email")]
    [InlineData("signature")]
    public async Task InvalidTokensAreRejected(string failure)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        var token = Token(audience: failure == "audience" ? "api" : Resource,
            expired: failure == "expired", email: failure != "email", issuer: failure == "issuer" ? "https://wrong.example.test" : Issuer);
        if (failure == "signature") token = token[..(token.LastIndexOf('.') + 1)] + "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);
        using var response = await client.PostAsJsonAsync("/mcp", new { });
        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized, await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("expense:user", true, true)]
    [InlineData("beer:user", false, false)]
    public async Task ToolDiscoveryRespectsGrantedScopes(string scope, bool canRead, bool canCreate)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(scope: scope));
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.DefaultRequestHeaders.Accept.ParseAdd("text/event-stream");
        client.DefaultRequestHeaders.Add("MCP-Protocol-Version", "2025-11-25");
        using var response = await client.PostAsJsonAsync("/mcp", new { jsonrpc = "2.0", id = 1, method = "tools/list" });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(canRead, body.Contains("get_expenses"));
        Assert.Equal(canCreate, body.Contains("add_expense"));
        Assert.DoesNotContain("error", body);
    }

    [Fact]
    public async Task TokenWithoutExpenseScopeCannotCreateExpenses()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(scope: "beer:user"));
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.DefaultRequestHeaders.Accept.ParseAdd("text/event-stream");
        client.DefaultRequestHeaders.Add("MCP-Protocol-Version", "2025-11-25");
        using var response = await client.PostAsJsonAsync("/mcp", new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "add_expense", arguments = new { amount = 1, typeId = "test" } }
        });
        Assert.True(response.StatusCode == HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Access forbidden: This tool requires authorization.", body);
        Assert.DoesNotContain("SqlException", body);
    }
}
