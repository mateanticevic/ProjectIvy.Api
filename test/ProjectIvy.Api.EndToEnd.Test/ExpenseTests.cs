using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Api.EndToEnd.Test.Infrastructure;
using ProjectIvy.Model.Binding.Expense;
using Xunit;

namespace ProjectIvy.Api.EndToEnd.Test;

[Collection(SqlServerCollection.Name)]
public sealed class ExpenseTests(SqlServerFixture fixture) : IAsyncLifetime
{
    private readonly ExpenseTestSession _session = new(fixture);
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        // Prove even an inherited production-looking value is replaced before host creation.
        var previous = Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN");
        _ambientConnection = previous;
        Environment.SetEnvironmentVariable("CONNECTION_STRING_MAIN", "Server=production.invalid;Database=MustNeverConnect;Integrated Security=true");
        try
        {
            await _session.InitializeAsync();
            Assert.Equal(_session.ConnectionString, Environment.GetEnvironmentVariable("CONNECTION_STRING_MAIN"));
            _client = _session.Factory!.CreateUserClient(_session.User.Email);
        }
        catch
        {
            try { await _session.DisposeAsync(); }
            finally { Environment.SetEnvironmentVariable("CONNECTION_STRING_MAIN", previous); }
            throw;
        }
    }

    private string? _ambientConnection;

    public async Task DisposeAsync()
    {
        try
        {
            _client?.Dispose();
            await _session.DisposeAsync();
        }
        finally
        {
            Environment.SetEnvironmentVariable("CONNECTION_STRING_MAIN", _ambientConnection);
        }
    }

    private static ExpenseBinding Binding() => new()
    {
        Amount = 12.34m, CurrencyId = "EUR", ExpenseTypeId = "e2e-food",
        Date = new DateTime(2026, 2, 12), Comment = "Lunch", NeedsReview = true
    };

    private async Task<string> CreateAsync(ExpenseBinding binding)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/expense")
        {
            Content = JsonContent.Create(binding)
        };
        request.Headers.Accept.ParseAdd("application/json");
        using var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var id = await response.Content.ReadFromJsonAsync<string>();
        Assert.False(string.IsNullOrWhiteSpace(id));
        return id!;
    }

    private async Task<JsonElement> GetAsync(string query = "")
    {
        using var response = await _client.GetAsync($"/expense{query}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }

    [Fact]
    public async Task PostPersistsExpenseAndGetReturnsIt()
    {
        var binding = Binding();
        var id = await CreateAsync(binding);
        Assert.Equal("1", id);

        using var db = _session.OpenDatabase();
        var entity = await db.Expenses.SingleAsync();
        Assert.Equal(id, entity.ValueId);
        Assert.Equal(_session.User.Id, entity.UserId);
        Assert.Equal(binding.Amount, entity.Amount);
        Assert.Equal(binding.Date, entity.Date);
        Assert.Equal(binding.Date, entity.DatePaid);
        Assert.Equal(binding.Comment, entity.Comment);
        Assert.Equal(_session.CurrencyId, entity.CurrencyId);
        Assert.Equal(_session.ExpenseTypeId, entity.ExpenseTypeId);
        Assert.True(entity.NeedsReview);

        var result = await GetAsync();
        Assert.Equal(1, result.GetProperty("count").GetInt32());
        var expense = Assert.Single(result.GetProperty("items").EnumerateArray());
        Assert.Equal(id, expense.GetProperty("id").GetString());
        Assert.Equal(binding.Amount, expense.GetProperty("amount").GetDecimal());
        Assert.Equal(binding.Comment, expense.GetProperty("comment").GetString());
        Assert.Equal(binding.Date, DateTime.Parse(expense.GetProperty("date").GetString()!));
        Assert.Equal(binding.Date, DateTime.Parse(expense.GetProperty("datePaid").GetString()!));
        Assert.Equal("EUR", expense.GetProperty("currency").GetProperty("code").GetString());
        Assert.Equal("e2e-food", expense.GetProperty("expenseType").GetProperty("id").GetString());
    }

    [Fact]
    public async Task GetEmptyCollection()
    {
        var result = await GetAsync();
        Assert.Equal(0, result.GetProperty("count").GetInt32());
        Assert.Empty(result.GetProperty("items").EnumerateArray());
    }

    [Fact]
    public async Task GetOnlyReturnsCurrentUsersExpenses()
    {
        await _session.SeedExpenseAsync(_session.OtherUser.Id, "1", new DateTime(2026, 2, 12), 99);
        var id = await CreateAsync(Binding());
        var result = await GetAsync();
        Assert.Equal(1, result.GetProperty("count").GetInt32());
        var expense = Assert.Single(result.GetProperty("items").EnumerateArray());
        Assert.Equal(id, expense.GetProperty("id").GetString());
        Assert.Equal(12.34m, expense.GetProperty("amount").GetDecimal());
    }

    [Fact]
    public async Task PostInvalidatesCachedCollection()
    {
        Assert.Equal(0, (await GetAsync()).GetProperty("count").GetInt32());
        var id = await CreateAsync(Binding());
        var result = await GetAsync();
        Assert.Equal(1, result.GetProperty("count").GetInt32());
        Assert.Equal(id, Assert.Single(result.GetProperty("items").EnumerateArray()).GetProperty("id").GetString());
    }

    [Fact]
    public async Task GetBindsDateFilterAndPagination()
    {
        await _session.SeedExpenseAsync(_session.User.Id, "1", new DateTime(2026, 1, 31), 1);
        await _session.SeedExpenseAsync(_session.User.Id, "2", new DateTime(2026, 2, 1), 2);
        await _session.SeedExpenseAsync(_session.User.Id, "3", new DateTime(2026, 2, 28), 3);
        await _session.SeedExpenseAsync(_session.User.Id, "4", new DateTime(2026, 3, 1), 4);
        const string query = "?From=2026-02-01&To=2026-02-28&PageSize=1&OrderBy=date&OrderAscending=true";
        var first = await GetAsync(query + "&Page=1");
        var second = await GetAsync(query + "&Page=2");
        Assert.Equal(2, first.GetProperty("count").GetInt32());
        Assert.Equal(2, second.GetProperty("count").GetInt32());
        Assert.Equal("2", Assert.Single(first.GetProperty("items").EnumerateArray()).GetProperty("id").GetString());
        Assert.Equal("3", Assert.Single(second.GetProperty("items").EnumerateArray()).GetProperty("id").GetString());
    }

    [Theory]
    [InlineData("GET", false, HttpStatusCode.Unauthorized)]
    [InlineData("POST", false, HttpStatusCode.Unauthorized)]
    [InlineData("GET", true, HttpStatusCode.Forbidden)]
    [InlineData("POST", true, HttpStatusCode.Forbidden)]
    public async Task EndpointsRequireAuthenticationAndScope(string method, bool authenticated, HttpStatusCode expected)
    {
        using var client = _session.Factory!.CreateUserClient(authenticated ? _session.User.Email : null, scope: null);
        using var request = new HttpRequestMessage(new HttpMethod(method), "/expense");
        if (method == "POST") request.Content = JsonContent.Create(Binding());
        using var response = await client.SendAsync(request);
        Assert.Equal(expected, response.StatusCode);
        using var db = _session.OpenDatabase();
        Assert.False(await db.Expenses.AnyAsync());
    }
}
