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
    public async Task SplitUpdatesOriginalAndCopiesAdditionalExpensesAtomically()
    {
        var id = await CreateAsync(Binding());
        int newTypeId;
        using (var db = _session.OpenDatabase())
        {
            var type = new ProjectIvy.Model.Database.Main.Finance.ExpenseType
            {
                ValueId = "e2e-drink", Name = "Test drink"
            };
            db.ExpenseTypes.Add(type);
            var expense = await db.Expenses.SingleAsync();
            expense.ExcludeFromMonthlySums = true;
            expense.ExternalId = "split-test";
            expense.InstallmentRef = "installment";
            expense.ParentAmount = 24.68m;
            expense.ParentCurrencyId = _session.CurrencyId;
            expense.ParentCurrencyExchangeRate = 2m;
            await db.SaveChangesAsync();
            newTypeId = type.Id;
        }
        Assert.Equal(1, (await GetAsync()).GetProperty("count").GetInt32());

        using var response = await _client.PostAsJsonAsync($"/expense/{id}/split", new ExpenseSplitBinding
        {
            Amount = 5m, ExpenseTypeId = "e2e-drink", Comment = "Original part",
            Expenses = new()
            {
                new() { Amount = 4m, Comment = "" },
                new() { Amount = 3.34m, ExpenseTypeId = "e2e-drink", Comment = "New part" }
            }
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new[] { id, "2", "3" }, await response.Content.ReadFromJsonAsync<string[]>());

        using var verify = _session.OpenDatabase();
        var expenses = await verify.Expenses.OrderBy(x => x.ValueId).ToListAsync();
        Assert.Equal(new[] { 5m, 4m, 3.34m }, expenses.Select(x => x.Amount));
        Assert.Equal(new[] { "Original part", "", "New part" }, expenses.Select(x => x.Comment));
        Assert.Equal(new[] { newTypeId, _session.ExpenseTypeId, newTypeId }, expenses.Select(x => x.ExpenseTypeId));
        Assert.Equal(24.68m, expenses.Sum(x => x.ParentAmount));
        Assert.All(expenses, x =>
        {
            Assert.Equal(_session.User.Id, x.UserId);
            Assert.Equal(_session.CurrencyId, x.CurrencyId);
            Assert.Equal(Binding().Date, x.Date);
            Assert.Equal(Binding().Date, x.DatePaid);
            Assert.True(x.NeedsReview);
            Assert.True(x.ExcludeFromMonthlySums);
            Assert.Equal("split-test", x.ExternalId);
            Assert.Equal("installment", x.InstallmentRef);
            Assert.Equal(2m, x.ParentCurrencyExchangeRate);
        });
        Assert.Equal(3, (await GetAsync()).GetProperty("count").GetInt32());
    }

    [Theory]
    [InlineData(7.34, "missing-type")]
    [InlineData(7.33, "e2e-food")]
    [InlineData(7.341, "e2e-food")]
    public async Task InvalidSplitLeavesOriginalUnchanged(double amount, string typeId)
    {
        var id = await CreateAsync(Binding());
        using var response = await _client.PostAsJsonAsync($"/expense/{id}/split", new ExpenseSplitBinding
        {
            Amount = 5m, Comment = "Must not persist",
            Expenses = new() { new() { Amount = (decimal)amount, ExpenseTypeId = typeId } }
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var db = _session.OpenDatabase();
        var original = Assert.Single(await db.Expenses.ToListAsync());
        Assert.Equal(Binding().Amount, original.Amount);
        Assert.Equal(Binding().Comment, original.Comment);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"amount\":5,\"expenses\":[]}")]
    [InlineData("{\"amount\":5,\"expenses\":[{}]}")]
    [InlineData("{\"amount\":5,\"expenses\":[null]}")]
    public async Task SplitRejectsMissingAmountsAndParts(string json)
    {
        var id = await CreateAsync(Binding());
        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync($"/expense/{id}/split", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var db = _session.OpenDatabase();
        Assert.Equal(Binding().Amount, (await db.Expenses.SingleAsync()).Amount);
    }

    [Fact]
    public async Task SplitInheritsOmittedTypeAndComment()
    {
        var id = await CreateAsync(Binding());
        using var response = await _client.PostAsJsonAsync($"/expense/{id}/split", new ExpenseSplitBinding
        {
            Amount = 5m, Expenses = new() { new() { Amount = 7.34m } }
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var db = _session.OpenDatabase();
        var expenses = await db.Expenses.ToListAsync();
        Assert.Equal(2, expenses.Count);
        Assert.All(expenses, x =>
        {
            Assert.Equal(Binding().Comment, x.Comment);
            Assert.Equal(_session.ExpenseTypeId, x.ExpenseTypeId);
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SplitReturnsNotFoundForMissingOrOtherUsersExpense(bool otherUser)
    {
        if (otherUser)
            await _session.SeedExpenseAsync(_session.OtherUser.Id, "1", Binding().Date, 12.34m);
        using var response = await _client.PostAsJsonAsync("/expense/1/split", new ExpenseSplitBinding
        {
            Amount = 5m, Expenses = new() { new() { Amount = 7.34m } }
        });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        using var db = _session.OpenDatabase();
        Assert.False(await db.Expenses.AnyAsync(x => x.UserId == _session.User.Id));
        if (otherUser)
            Assert.Equal(12.34m, (await db.Expenses.SingleAsync()).Amount);
    }

    [Fact]
    public async Task SplitRequiresExpenseScope()
    {
        var id = await CreateAsync(Binding());
        using var client = _session.Factory!.CreateUserClient(_session.User.Email, scope: "beer:user");
        using var response = await client.PostAsJsonAsync($"/expense/{id}/split", new ExpenseSplitBinding
        {
            Amount = 5m, Expenses = new() { new() { Amount = 7.34m } }
        });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        using var db = _session.OpenDatabase();
        Assert.Equal(Binding().Amount, (await db.Expenses.SingleAsync()).Amount);
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
