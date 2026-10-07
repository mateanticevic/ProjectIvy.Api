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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol;
using Moq;
using ProjectIvy.Api.EndToEnd.Test.Infrastructure;
using ProjectIvy.Business.Handlers.User;
using Database = ProjectIvy.Model.Database.Main;
using View = ProjectIvy.Model.View.User;
using Xunit;

namespace ProjectIvy.Api.EndToEnd.Test;

public sealed class McpAuthenticationTests
{
    private const string Resource = "https://localhost/mcp";
    private const string Issuer = "https://issuer.example.test/realms/ivy";
    private static readonly SymmetricSecurityKey Key = new(new byte[64].Select((_, i) => (byte)(i + 1)).ToArray());

    private sealed class Factory(IUserHandler? userHandler = null) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("EndToEnd");
            builder.ConfigureTestServices(services =>
            {
                if (userHandler is not null)
                {
                    services.RemoveAll<IUserHandler>();
                    services.AddSingleton(userHandler);
                }

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
    [InlineData("openid email profile", false, false, false)]
    [InlineData("expense:user", true, false, false)]
    [InlineData("beer:user", false, true, false)]
    [InlineData("expense:user beer:user", true, true, false)]
    [InlineData("basic:user", false, false, true)]
    [InlineData("basic:user expense:user beer:user", true, true, true)]
    public async Task ToolDiscoveryRespectsGrantedScopes(string scope, bool canUseExpenses, bool canUseBeer, bool canUseCurrentUser)
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
        var payload = body.Split('\n').Single(line => line.StartsWith("data: "))[6..];
        using var json = JsonDocument.Parse(payload);
        Assert.False(json.RootElement.TryGetProperty("error", out _), body);
        var names = json.RootElement.GetProperty("result").GetProperty("tools")
            .EnumerateArray().Select(tool => tool.GetProperty("name").GetString()).Order().ToArray();
        var expected = new List<string>();
        if (canUseExpenses) expected.AddRange(["add_expense", "get_expenses", "get_types", "sum"]);
        if (canUseBeer) expected.Add("sum_beer");
        if (canUseCurrentUser) expected.Add("get_current_user");
        Assert.Equal(expected.Order(), names);
    }

    [Fact]
    public async Task CurrentUserDiscoveryReportsProfileSchemaAndReadOnlyHint()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = CreateMcpClient(factory, "basic:user");
        using var json = await SendMcp(client, new { jsonrpc = "2.0", id = 1, method = "tools/list" });
        var tool = Assert.Single(json.RootElement.GetProperty("result").GetProperty("tools").EnumerateArray());
        Assert.Equal("get_current_user", tool.GetProperty("name").GetString());
        Assert.Contains("authenticated user's profile", tool.GetProperty("description").GetString());
        Assert.True(tool.GetProperty("annotations").GetProperty("readOnlyHint").GetBoolean());

        var input = tool.GetProperty("inputSchema");
        Assert.Equal("object", input.GetProperty("type").GetString());
        Assert.True(!input.TryGetProperty("properties", out var inputs) || !inputs.EnumerateObject().Any());
        Assert.True(!input.TryGetProperty("required", out var required) || required.GetArrayLength() == 0);

        var properties = tool.GetProperty("outputSchema").GetProperty("properties");
        Assert.Equal(new[] { "defaultCar", "defaultCurrency", "email", "firstName", "lastName", "trackingStartDate", "username" },
            properties.EnumerateObject().Select(property => property.Name).Order());
        var currency = properties.GetProperty("defaultCurrency").GetProperty("properties");
        Assert.Equal(new[] { "code", "id", "name", "symbol" }, currency.EnumerateObject().Select(property => property.Name).Order());
        var car = properties.GetProperty("defaultCar").GetProperty("properties");
        Assert.Equal(new[] { "id", "model", "productionYear", "serviceDue", "services" }, car.EnumerateObject().Select(property => property.Name).Order());
        var model = car.GetProperty("model").GetProperty("properties");
        Assert.Equal(new[] { "engineDisplacement", "id", "manufacturer", "modelYear", "name", "power" }, model.EnumerateObject().Select(property => property.Name).Order());
        Assert.Equal("date-time", properties.GetProperty("trackingStartDate").GetProperty("format").GetString());
        Assert.Contains("null", properties.GetProperty("trackingStartDate").GetProperty("type").EnumerateArray().Select(type => type.GetString()));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task CurrentUserToolReturnsSameProfileAsRestWithoutDatabaseAccess(bool hasTrackingStartDate)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var profile = new View.User(new Database.User.User
        {
            Username = "test-user", FirstName = "Test", LastName = "User", Email = "user@example.test",
            TrackingStartDate = hasTrackingStartDate ? new DateTime(2026, 10, 1) : null,
            DefaultCurrency = new Database.Common.Currency { Code = "EUR", Name = "Euro", Symbol = "€" },
            DefaultCar = new Database.Transport.Car
            {
                ValueId = "test-car", ProductionYear = 2020,
                CarModel = new Database.Transport.CarModel { ValueId = "test-model", Name = "Test Model", ModelYear = 2020 }
            }
        });
        var handler = new Mock<IUserHandler>(MockBehavior.Strict);
        handler.Setup(value => value.Get((int?)null)).ReturnsAsync(profile);
        await using var factory = new Factory(handler.Object);
        using var client = CreateMcpClient(factory, "basic:user");
        using var json = await SendMcp(client, new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "get_current_user", arguments = new { } }
        });
        var result = json.RootElement.GetProperty("result");
        Assert.True(!result.TryGetProperty("isError", out var isError) || !isError.GetBoolean());
        var structuredContent = result.GetProperty("structuredContent");
        var expected = JsonSerializer.SerializeToElement(profile, McpJsonUtilities.DefaultOptions);
        Assert.True(JsonElement.DeepEquals(expected, structuredContent), structuredContent.GetRawText());

        using var rest = await client.GetAsync("/user");
        Assert.Equal(HttpStatusCode.OK, rest.StatusCode);
        using var restJson = JsonDocument.Parse(await rest.Content.ReadAsStringAsync());
        // REST uses a custom date converter; both transports must preserve the same profile values.
        var restOptions = factory.Services.GetRequiredService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>().Value.JsonSerializerOptions;
        var expectedRest = JsonSerializer.SerializeToElement(profile, restOptions);
        Assert.True(JsonElement.DeepEquals(expectedRest, restJson.RootElement), restJson.RootElement.GetRawText());
        handler.Verify(value => value.Get((int?)null), Times.Exactly(2));
        handler.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("openid email profile")]
    [InlineData("expense:user beer:user")]
    public async Task TokenWithoutBasicUserScopeCannotGetCurrentUser(string scope)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<IUserHandler>(MockBehavior.Strict);
        await using var factory = new Factory(handler.Object);
        using var client = CreateMcpClient(factory, scope);
        using var json = await SendMcp(client, new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "get_current_user", arguments = new { } }
        });
        Assert.Equal("Access forbidden: This tool requires authorization.", json.RootElement.GetProperty("error").GetProperty("message").GetString());
        handler.VerifyNoOtherCalls();
    }

    private static HttpClient CreateMcpClient(Factory factory, string scope)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(scope: scope));
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.DefaultRequestHeaders.Accept.ParseAdd("text/event-stream");
        client.DefaultRequestHeaders.Add("MCP-Protocol-Version", "2025-11-25");
        return client;
    }

    private static async Task<JsonDocument> SendMcp(HttpClient client, object request)
    {
        using var response = await client.PostAsJsonAsync("/mcp", request);
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.OK, body);
        var payload = body.Split('\n').Single(line => line.StartsWith("data: "))[6..];
        return JsonDocument.Parse(payload);
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
