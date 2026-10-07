using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using ProjectIvy.Api.EndToEnd.Test.Infrastructure;
using ProjectIvy.Business.Handlers.Expense;
using ProjectIvy.Model.Binding.Expense;
using Xunit;

namespace ProjectIvy.Api.EndToEnd.Test;

public sealed class ExpenseSplitContractTests
{
    private sealed class Factory : WebApplicationFactory<Program>
    {
        public Mock<IExpenseHandler> Handler { get; } = new(MockBehavior.Strict);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("EndToEnd");
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IExpenseHandler>();
                services.AddSingleton(Handler.Object);
                services.AddAuthentication()
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.SchemeName, _ => { });
                services.PostConfigure<AuthenticationOptions>(options =>
                {
                    options.DefaultScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
                    options.DefaultForbidScheme = TestAuthenticationHandler.SchemeName;
                });
            });
        }
    }

    private static HttpClient Client(Factory factory, string? scope = "expense:user", bool authenticated = true)
    {
        var client = factory.CreateClient();
        if (authenticated) client.DefaultRequestHeaders.Add("X-E2E-Email", "split@example.test");
        if (scope is not null) client.DefaultRequestHeaders.Add("X-E2E-Scope", scope);
        return client;
    }

    [Fact]
    public async Task PostBindsOriginalAndAdditionalPartsAndReturnsValueIds()
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        ExpenseSplitBinding? received = null;
        factory.Handler.Setup(x => x.Split("42", It.IsAny<ExpenseSplitBinding>()))
            .Callback<string, ExpenseSplitBinding>((_, body) => received = body)
            .ReturnsAsync(new[] { "42", "43", "44" });
        using var client = Client(factory);
        using var response = await client.PostAsJsonAsync("/expense/42/split", new
        {
            amount = 5m, expenseTypeId = "food", comment = "Original",
            expenses = new[]
            {
                new { amount = 3m, expenseTypeId = (string?)"drinks", comment = (string?)"New" },
                new { amount = 2m, expenseTypeId = (string?)null, comment = (string?)null }
            }
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new[] { "42", "43", "44" }, await response.Content.ReadFromJsonAsync<string[]>());
        Assert.NotNull(received);
        Assert.Equal(5m, received.Amount);
        Assert.Equal("food", received.ExpenseTypeId);
        Assert.Equal("Original", received.Comment);
        Assert.Equal(2, received.Expenses.Count);
        Assert.Equal(3m, received.Expenses[0].Amount);
        Assert.Equal("drinks", received.Expenses[0].ExpenseTypeId);
        Assert.Equal("New", received.Expenses[0].Comment);
        Assert.Null(received.Expenses[1].ExpenseTypeId);
        Assert.Null(received.Expenses[1].Comment);
        factory.Handler.VerifyAll();
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"amount\":5,\"expenses\":[]}")]
    [InlineData("{\"amount\":5,\"expenses\":[{}]}")]
    [InlineData("{\"expenses\":[{\"amount\":5}]}")]
    [InlineData("{\"amount\":\"invalid\",\"expenses\":[{\"amount\":5}]}")]
    public async Task InvalidBodiesAreRejectedBeforeHandlerExecution(string json)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = Client(factory);
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        using var response = await client.PostAsync("/expense/42/split", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        factory.Handler.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(false, null, HttpStatusCode.Unauthorized)]
    [InlineData(true, "beer:user", HttpStatusCode.Forbidden)]
    public async Task SplitRequiresAuthenticationAndExpenseScope(bool authenticated, string? scope, HttpStatusCode expected)
    {
        using var environment = new TestEnvironment("Server=127.0.0.1,1;Database=unused;User Id=sa;Password=Unused!123456");
        await using var factory = new Factory();
        using var client = Client(factory, scope, authenticated);
        using var response = await client.PostAsJsonAsync("/expense/42/split", new
        {
            amount = 5m, expenses = new[] { new { amount = 5m } }
        });
        Assert.Equal(expected, response.StatusCode);
        factory.Handler.VerifyNoOtherCalls();
    }
}
