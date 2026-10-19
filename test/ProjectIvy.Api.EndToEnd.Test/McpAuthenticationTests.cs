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
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Business.Handlers.Tracking;
using Database = ProjectIvy.Model.Database.Main;
using View = ProjectIvy.Model.View.User;
using Xunit;

namespace ProjectIvy.Api.EndToEnd.Test;

public sealed class McpAuthenticationTests
{
    private const string Resource = "https://localhost/mcp";
    private const string Issuer = "https://issuer.example.test/realms/ivy";
    private static readonly SymmetricSecurityKey Key = new(new byte[64].Select((_, i) => (byte)(i + 1)).ToArray());

    private sealed class Factory(IUserHandler? userHandler = null, IExpenseHandler? expenseHandler = null, ITrackingHandler? trackingHandler = null, ITrackingExportHandler? trackingExportHandler = null) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("EndToEnd");
            builder.ConfigureTestServices(services =>
            {
                if (trackingExportHandler is not null)
                {
                    services.RemoveAll<ITrackingExportHandler>();
                    services.AddSingleton(trackingExportHandler);
                    if (trackingHandler is null)
                    {
                        services.RemoveAll<ITrackingHandler>();
                        services.AddSingleton(new Mock<ITrackingHandler>(MockBehavior.Strict).Object);
                    }
                }

                if (trackingHandler is not null)
                {
                    services.RemoveAll<ITrackingHandler>();
                    services.AddSingleton(trackingHandler);
                }

                if (expenseHandler is not null)
                {
                    services.RemoveAll<IExpenseHandler>();
                    services.AddSingleton(expenseHandler);
                    services.RemoveAll<IExpenseTypeHandler>();
                    services.AddSingleton(Mock.Of<IExpenseTypeHandler>());
                }

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
    [InlineData("tracking:view_last", false, false, false)]
    [InlineData("tracking:user", false, false, false)]
    [InlineData("tracking:user tracking:view_last", false, false, false)]
    [InlineData("basic:user expense:user beer:user tracking:view_last", true, true, true)]
    public async Task ToolDiscoveryRespectsGrantedScopes(string scope, bool canUseExpenses, bool canUseBeer, bool canUseBasicTools)
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
        if (canUseExpenses) expected.AddRange(["expense_add", "expense_get", "expense_types_get", "expense_sum"]);
        if (canUseBeer) expected.Add("beer_sum");
        if (canUseBasicTools) expected.AddRange(["car_log_add", "get_current_user"]);
        if (scope.Split(' ').Contains("tracking:view_last")) expected.Add("tracking_last_get");
        if (scope.Split(' ').Contains("tracking:user")) expected.Add("tracking_get");
        Assert.Equal(expected.Order(), names);
    }

    [Fact]
    public async Task CurrentUserDiscoveryReportsProfileSchemaAndReadOnlyHint()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = CreateMcpClient(factory, "basic:user");
        using var json = await SendMcp(client, new { jsonrpc = "2.0", id = 1, method = "tools/list" });
        var tool = Assert.Single(json.RootElement.GetProperty("result").GetProperty("tools").EnumerateArray(),
            tool => tool.GetProperty("name").GetString() == "get_current_user");
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

    [Fact]
    public async Task ExpenseSumDiscoveryExposesOptionalGrouping()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = CreateMcpClient(factory, "expense:user");
        using var json = await SendMcp(client, new { jsonrpc = "2.0", id = 1, method = "tools/list" });
        var tool = Assert.Single(json.RootElement.GetProperty("result").GetProperty("tools").EnumerateArray(),
            tool => tool.GetProperty("name").GetString() == "expense_sum");
        var input = tool.GetProperty("inputSchema");
        var grouping = input.GetProperty("properties").GetProperty("groupBy");
        Assert.Equal("string", grouping.GetProperty("type").GetString());
        Assert.True(!input.TryGetProperty("required", out var required) ||
            !required.EnumerateArray().Any(value => value.GetString() == "groupBy"));
        Assert.Contains("monthOfYearByType", grouping.GetProperty("description").GetString());
        Assert.Contains("calendar months across years", grouping.GetProperty("description").GetString());
        Assert.Contains("0 Monday through 6 Sunday", grouping.GetProperty("description").GetString());
        Assert.True(tool.GetProperty("annotations").GetProperty("readOnlyHint").GetBoolean());
        var output = tool.GetProperty("outputSchema");
        Assert.Equal("object", output.GetProperty("type").GetString());
        var properties = output.GetProperty("properties");
        Assert.Equal(new[] { "byCurrency", "byDay", "byDayOfWeek", "byMonth", "byMonthOfYear", "byMonthOfYearByType", "byType", "byYear", "byYearByType", "total" },
            properties.EnumerateObject().Select(property => property.Name).Order());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("day")]
    [InlineData("dayOfWeek")]
    [InlineData("month")]
    [InlineData("monthOfYear")]
    [InlineData("year")]
    [InlineData("currency")]
    [InlineData("type")]
    [InlineData("monthOfYearByType")]
    [InlineData("yearByType")]
    public async Task ExpenseSumSerializesTypedTotalGroupedAndNestedResults(string? groupBy)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<IExpenseHandler>(MockBehavior.Strict);
        var expected = ExpenseSumToolTests.ConfigureSum(handler, groupBy ?? "none");
        await using var factory = new Factory(userHandler: Mock.Of<IUserHandler>(), expenseHandler: handler.Object);
        using var client = CreateMcpClient(factory, "expense:user");
        var arguments = new Dictionary<string, object?> { ["from"] = "2026-01-01", ["to"] = "2026-10-15", ["typeId"] = "food" };
        if (groupBy is not null) arguments["groupBy"] = groupBy;
        using var json = await SendMcp(client, new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "expense_sum", arguments }
        });
        var result = json.RootElement.GetProperty("result");
        Assert.True(!result.TryGetProperty("isError", out var isError) || !isError.GetBoolean(), result.GetRawText());
        var content = Assert.Single(result.GetProperty("content").EnumerateArray());
        Assert.Equal("text", content.GetProperty("type").GetString());
        using var actual = JsonDocument.Parse(content.GetProperty("text").GetString()!);
        Assert.True(JsonElement.DeepEquals(JsonSerializer.SerializeToElement(expected, McpJsonUtilities.DefaultOptions), actual.RootElement),
            actual.RootElement.GetRawText());
        Assert.Single(actual.RootElement.EnumerateObject());
        Assert.True(JsonElement.DeepEquals(actual.RootElement, result.GetProperty("structuredContent")), result.GetRawText());
        handler.VerifyAll();
        handler.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LastTrackingDiscoveryReportsOptionalCutoffAndLocationSchema()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = CreateMcpClient(factory, "tracking:view_last");
        using var json = await SendMcp(client, new { jsonrpc = "2.0", id = 1, method = "tools/list" });
        var tool = Assert.Single(json.RootElement.GetProperty("result").GetProperty("tools").EnumerateArray());
        Assert.Equal("tracking_last_get", tool.GetProperty("name").GetString());
        Assert.True(tool.GetProperty("annotations").GetProperty("readOnlyHint").GetBoolean());
        var input = tool.GetProperty("inputSchema");
        Assert.Contains("UTC", input.GetProperty("properties").GetProperty("at").GetProperty("description").GetString());
        Assert.True(!input.TryGetProperty("required", out var required) || !required.EnumerateArray().Any());
        Assert.True(tool.GetProperty("outputSchema").GetProperty("properties").TryGetProperty("tracking", out _));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task LastTrackingForwardsOptionalUtcCutoffAndSerializesLocation(bool includeAt)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var at = new DateTime(2026, 10, 15, 12, 0, 0, DateTimeKind.Utc);
        var handler = new Mock<ITrackingHandler>(MockBehavior.Strict);
        var expected = new ProjectIvy.Model.View.Tracking.TrackingLocation
        {
            Tracking = new(new Database.Tracking.Tracking
            {
                Timestamp = at.AddMinutes(-1), Latitude = 45, Longitude = 16
            })
        };
        handler.Setup(x => x.GetLastLocation(includeAt ? at : null)).ReturnsAsync(expected);
        await using var factory = new Factory(trackingHandler: handler.Object);
        using var client = CreateMcpClient(factory, "tracking:view_last");
        var arguments = new Dictionary<string, object?>();
        if (includeAt) arguments["at"] = "2026-10-15T12:00:00Z";
        using var json = await SendMcp(client, new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "tracking_last_get", arguments }
        });
        var result = json.RootElement.GetProperty("result");
        Assert.True(!result.TryGetProperty("isError", out var isError) || !isError.GetBoolean(), result.GetRawText());
        Assert.True(JsonElement.DeepEquals(JsonSerializer.SerializeToElement(expected, McpJsonUtilities.DefaultOptions),
            result.GetProperty("structuredContent")), result.GetRawText());
        handler.Verify(x => x.GetLastLocation(includeAt ? at : null), Times.Once);
        handler.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TokenWithoutTrackingScopeCannotReadLastLocation()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<ITrackingHandler>(MockBehavior.Strict);
        await using var factory = new Factory(trackingHandler: handler.Object);
        using var client = CreateMcpClient(factory, "basic:user");
        using var json = await SendMcp(client, new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "tracking_last_get", arguments = new { } }
        });
        Assert.Contains("Access forbidden: This tool requires authorization.", json.RootElement.GetRawText());
        handler.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TrackingMapDiscoveryRequiresDateAndUsesUserDefaultTimezone()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = CreateMcpClient(factory, "tracking:user");
        using var json = await SendMcp(client, new { jsonrpc = "2.0", id = 1, method = "tools/list" });
        var tool = Assert.Single(json.RootElement.GetProperty("result").GetProperty("tools").EnumerateArray());
        Assert.Equal("tracking_get", tool.GetProperty("name").GetString());
        Assert.True(tool.GetProperty("annotations").GetProperty("readOnlyHint").GetBoolean());
        var input = tool.GetProperty("inputSchema");
        Assert.Equal(new[] { "date" }, input.GetProperty("required").EnumerateArray().Select(x => x.GetString()));
        Assert.Contains("DefaultTimezone", input.GetProperty("properties").GetProperty("timezone").GetProperty("description").GetString());
        Assert.Equal("date", input.GetProperty("properties").GetProperty("date").GetProperty("format").GetString());
        Assert.False(tool.TryGetProperty("outputSchema", out _));
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("America/New_York", false)]
    [InlineData(null, true)]
    public async Task TrackingMapReturnsNativeImageAndDownloadLinkOrEmptyMessage(string? timezone, bool empty)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var date = new DateOnly(2026, 10, 19);
        var handler = new Mock<ITrackingExportHandler>(MockBehavior.Strict);
        handler.Setup(x => x.GetMap(date, timezone!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(empty ? null! : TrackingExportTests.TileFixture.Tile);
        await using var factory = new Factory(trackingExportHandler: handler.Object);
        using var client = CreateMcpClient(factory, "tracking:user");
        var arguments = new Dictionary<string, object?> { ["date"] = "2026-10-19" };
        if (timezone is not null) arguments["timezone"] = timezone;
        using var json = await SendMcp(client, new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "tracking_get", arguments }
        });
        var result = json.RootElement.GetProperty("result");
        Assert.True(!result.TryGetProperty("isError", out var error) || !error.GetBoolean(), result.GetRawText());
        Assert.False(result.TryGetProperty("structuredContent", out _));
        var blocks = result.GetProperty("content").EnumerateArray().ToArray();
        Assert.Equal(empty ? 1 : 2, blocks.Length);
        var content = blocks[0];
        Assert.Equal(empty ? "text" : "image", content.GetProperty("type").GetString());
        if (empty) Assert.Contains("No usable tracking points", content.GetProperty("text").GetString());
        else
        {
            Assert.Equal("image/png", content.GetProperty("mimeType").GetString());
            Assert.Equal(TrackingExportTests.TileFixture.Tile, Convert.FromBase64String(content.GetProperty("data").GetString()!));
            Assert.Equal("text", blocks[1].GetProperty("type").GetString());
            var text = blocks[1].GetProperty("text").GetString()!;
            Assert.Contains("requires an authenticated Ivy session", text);
            var url = new Uri(text.Split("](")[1].Split(')')[0]);
            Assert.Equal("https", url.Scheme);
            Assert.Equal("localhost", url.Host);
            Assert.Equal("/Tracking/Map", url.AbsolutePath);
            var expectedQuery = "?date=2026-10-19" + (timezone is null ? "" : "&timezone=America%2FNew_York");
            Assert.Equal(expectedQuery, url.Query);
            using var anonymous = factory.CreateClient();
            using var download = await anonymous.GetAsync(url);
            Assert.Equal(HttpStatusCode.Unauthorized, download.StatusCode);
        }
        handler.VerifyAll();
    }

    [Theory]
    [InlineData("tracking:view_last")]
    [InlineData("basic:user")]
    public async Task TrackingMapRequiresFullTrackingScope(string scope)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<ITrackingExportHandler>(MockBehavior.Strict);
        await using var factory = new Factory(trackingExportHandler: handler.Object);
        using var client = CreateMcpClient(factory, scope);
        using var json = await SendMcp(client, new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "tracking_get", arguments = new { date = "2026-10-19" } }
        });
        Assert.Contains("Access forbidden: This tool requires authorization.", json.RootElement.GetRawText());
        handler.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("timeout")]
    [InlineData("upstream")]
    public async Task TrackingMapFailuresAreToolErrors(string failure)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<ITrackingExportHandler>(MockBehavior.Strict);
        Exception exception = failure switch
        {
            "invalid" => new ArgumentException("Invalid timezone."),
            "timeout" => new TimeoutException(),
            _ => new HttpRequestException("Private tile URL must not appear in the response")
        };
        handler.Setup(x => x.GetMap(It.IsAny<DateOnly>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
        await using var factory = new Factory(trackingExportHandler: handler.Object);
        using var client = CreateMcpClient(factory, "tracking:user");
        using var json = await SendMcp(client, new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "tracking_get", arguments = new { date = "2026-10-19" } }
        });
        Assert.True(json.RootElement.GetProperty("result").GetProperty("isError").GetBoolean());
        Assert.DoesNotContain("Private tile URL", json.RootElement.GetRawText());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("19-10-2026")]
    public async Task TrackingMapRejectsMissingOrMalformedDate(string? date)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<ITrackingExportHandler>(MockBehavior.Strict);
        await using var factory = new Factory(trackingExportHandler: handler.Object);
        using var client = CreateMcpClient(factory, "tracking:user");
        var arguments = new Dictionary<string, object?>();
        if (date is not null) arguments["date"] = date;
        using var json = await SendMcp(client, new
        {
            jsonrpc = "2.0", id = 1, method = "tools/call",
            @params = new { name = "tracking_get", arguments }
        });
        Assert.True(json.RootElement.TryGetProperty("error", out _) ||
            json.RootElement.GetProperty("result").GetProperty("isError").GetBoolean());
        handler.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TrackingMapRestDocumentsTimezoneAsOptional()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/swagger/v1/swagger.json");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var map = document.RootElement.GetProperty("paths").EnumerateObject()
            .Single(path => path.Name.Equals("/tracking/map", StringComparison.OrdinalIgnoreCase)).Value;
        var parameters = map.GetProperty("get").GetProperty("parameters").EnumerateArray().ToArray();
        var timezone = parameters.Single(parameter => parameter.GetProperty("name").GetString()!
            .Equals("timezone", StringComparison.OrdinalIgnoreCase));
        Assert.True(!timezone.TryGetProperty("required", out var required) || !required.GetBoolean());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("America/New_York")]
    public async Task TrackingMapRestReturnsSamePngAndForwardsQueryFilters(string? timezone)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<ITrackingExportHandler>(MockBehavior.Strict);
        handler.Setup(x => x.GetMap(new DateOnly(2026, 10, 19), timezone!, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TrackingExportTests.TileFixture.Tile);
        await using var factory = new Factory(trackingExportHandler: handler.Object);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(scope: "tracking:user"));
        var query = "?date=2026-10-19" + (timezone is null ? "" : "&timezone=" + Uri.EscapeDataString(timezone));
        using var response = await client.GetAsync("/tracking/map" + query);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(TrackingExportTests.TileFixture.Tile, await response.Content.ReadAsByteArrayAsync());
        handler.VerifyAll();
    }

    [Theory]
    [InlineData("")]
    [InlineData("?date=invalid")]
    public async Task TrackingMapRestRejectsInvalidQueryBeforeCallingHandler(string query)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<ITrackingExportHandler>(MockBehavior.Strict);
        await using var factory = new Factory(trackingExportHandler: handler.Object);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(scope: "tracking:user"));
        using var response = await client.GetAsync("/tracking/map" + query);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        handler.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("tracking:view_last", HttpStatusCode.Forbidden)]
    public async Task TrackingMapRestRequiresFullTrackingScope(string? scope, HttpStatusCode status)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<ITrackingExportHandler>(MockBehavior.Strict);
        await using var factory = new Factory(trackingExportHandler: handler.Object);
        using var client = factory.CreateClient();
        if (scope is not null) client.DefaultRequestHeaders.Authorization = new("Bearer", Token(scope: scope));
        using var response = await client.GetAsync("/tracking/map?date=2026-10-19");
        Assert.Equal(status, response.StatusCode);
        handler.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData("empty", HttpStatusCode.NotFound)]
    [InlineData("invalid", HttpStatusCode.BadRequest)]
    [InlineData("timeout", HttpStatusCode.GatewayTimeout)]
    [InlineData("rendering", HttpStatusCode.InternalServerError)]
    public async Task TrackingMapRestMapsEmptyAndFailureResultsToHttpStatuses(string outcome, HttpStatusCode status)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        var handler = new Mock<ITrackingExportHandler>(MockBehavior.Strict);
        var setup = handler.Setup(x => x.GetMap(new DateOnly(2026, 10, 19), null!, It.IsAny<CancellationToken>()));
        if (outcome == "empty") setup.ReturnsAsync((byte[])null!);
        else setup.ThrowsAsync(outcome switch
        {
            "invalid" => new ArgumentException("Timezone must be a valid IANA timezone identifier."),
            "timeout" => new TimeoutException(),
            _ => new InvalidOperationException("Unable to render the tracking map.")
        });
        await using var factory = new Factory(trackingExportHandler: handler.Object);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new("Bearer", Token(scope: "tracking:user"));
        using var response = await client.GetAsync("/tracking/map?date=2026-10-19");
        Assert.Equal(status, response.StatusCode);
        handler.VerifyAll();
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
            @params = new { name = "expense_add", arguments = new { amount = 1, typeId = "test" } }
        });
        Assert.True(response.StatusCode == HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Access forbidden: This tool requires authorization.", body);
        Assert.DoesNotContain("SqlException", body);
    }
}
