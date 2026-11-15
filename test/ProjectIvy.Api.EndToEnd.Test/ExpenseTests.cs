using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Api.EndToEnd.Test.Infrastructure;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.Database.Main.Common;
using ProjectIvy.Model.Database.Main.Finance;
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
    public async Task MonthlySumsPreservePartialPeriodsFiltersAndEmptyMonths()
    {
        await _session.SeedExpenseAsync(_session.User.Id, "before", new(2025, 12, 9), 100m);
        await _session.SeedExpenseAsync(_session.User.Id, "december", new(2025, 12, 10), 12.34m);
        await _session.SeedExpenseAsync(_session.User.Id, "december-end", new(2025, 12, 31), 2.01m);
        await _session.SeedExpenseAsync(_session.User.Id, "excluded", new(2026, 1, 5), 200m);
        await _session.SeedExpenseAsync(_session.User.Id, "february", new(2026, 2, 10), 5.67m);
        await _session.SeedExpenseAsync(_session.User.Id, "after", new(2026, 2, 11), 300m);
        await _session.SeedExpenseAsync(_session.OtherUser.Id, "other", new(2025, 12, 15), 400m);
        using (var db = _session.OpenDatabase())
        {
            var excluded = await db.Expenses.SingleAsync(x => x.ValueId == "excluded");
            excluded.ExcludeFromMonthlySums = true;
            await db.SaveChangesAsync();
        }

        const string url = "/expense/sum/byMonthOfYear?From=2025-12-10&To=2026-02-10";
        var expected = new[]
        {
            KeyValuePair.Create("2025-12-1", 14.35m),
            KeyValuePair.Create("2026-1-1", 0m),
            KeyValuePair.Create("2026-2-1", 5.67m)
        };
        Assert.Equal(expected, await _client.GetFromJsonAsync<KeyValuePair<string, decimal>[]>(url));
        Assert.Equal(expected.Select(x => KeyValuePair.Create(x.Key, x.Key == "2026-2-1" ? x.Value : 0m)),
            await _client.GetFromJsonAsync<KeyValuePair<string, decimal>[]>(url + "&Month=2"));
        Assert.Equal(expected.Select(x => KeyValuePair.Create(x.Key, 0m)),
            await _client.GetFromJsonAsync<KeyValuePair<string, decimal>[]>(url + "&Description=no-match"));
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
    public async Task TransactionIdsCreateReplacePreserveAndClearLinks()
    {
        string firstId;
        string secondId;
        string otherUserId;
        using (var db = _session.OpenDatabase())
        {
            var first = new Transaction { Amount = -6m, Created = Binding().Date };
            var second = new Transaction { Amount = -6.34m, Created = Binding().Date };
            var other = new Transaction { Amount = -12.34m, Created = Binding().Date };
            db.Accounts.AddRange(
                new Account
                {
                    UserId = _session.User.Id, ValueId = "1", Name = "Test account",
                    CurrencyId = _session.CurrencyId, Active = true, Transactions = [first, second]
                },
                new Account
                {
                    UserId = _session.OtherUser.Id, ValueId = "2", Name = "Other account",
                    CurrencyId = _session.CurrencyId, Active = true, Transactions = [other]
                });
            await db.SaveChangesAsync();
            firstId = first.ValueId;
            secondId = second.ValueId;
            otherUserId = other.ValueId;
        }

        var binding = Binding();
        binding.TransactionIds = [firstId, secondId, firstId.ToUpperInvariant()];
        var id = await CreateAsync(binding);

        async Task<(decimal Amount, string?[] TransactionIds)> GetExpense()
        {
            var expense = await GetAsync($"/{id}");
            return (expense.GetProperty("amount").GetDecimal(), expense.GetProperty("transactions").EnumerateArray()
                .Select(x => x.GetProperty("id").GetString()).OrderBy(x => x).ToArray());
        }

        Assert.Equal(new[] { firstId, secondId }.OrderBy(x => x), (await GetExpense()).TransactionIds);

        binding.TransactionIds = [secondId];
        using (var response = await _client.PutAsJsonAsync($"/expense/{id}", binding))
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new[] { secondId }, (await GetExpense()).TransactionIds);

        binding.Amount = 20m;
        binding.TransactionIds = [firstId, otherUserId];
        using (var response = await _client.PutAsJsonAsync($"/expense/{id}", binding))
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var afterRejectedUpdate = await GetExpense();
        Assert.Equal(12.34m, afterRejectedUpdate.Amount);
        Assert.Equal(new[] { secondId }, afterRejectedUpdate.TransactionIds);

        binding.TransactionIds = [Guid.NewGuid().ToString()];
        using (var response = await _client.PostAsJsonAsync("/expense", binding))
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(1, (await GetAsync()).GetProperty("count").GetInt32());

        binding.TransactionIds = null;
        using (var response = await _client.PutAsJsonAsync($"/expense/{id}", binding))
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var afterPreservedUpdate = await GetExpense();
        Assert.Equal(20m, afterPreservedUpdate.Amount);
        Assert.Equal(new[] { secondId }, afterPreservedUpdate.TransactionIds);

        binding.TransactionIds = [];
        using (var response = await _client.PutAsJsonAsync($"/expense/{id}", binding))
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Empty((await GetExpense()).TransactionIds);

        using var dbAfter = _session.OpenDatabase();
        Assert.Equal(3, await dbAfter.Transactions.CountAsync());
    }

    [Theory]
    [InlineData(TransactionSource.Hac, "Header\r\n;Lunch;;13.02.2026 11:30:00;12.02.2026 11:30:00;;;Toll;;0;12;100\r\n")]
    [InlineData(TransactionSource.OtpBank, "Header\r\n,13.02.2026,Lunch 12/02/2026 11:30,-12,100\r\n")]
    [InlineData(TransactionSource.Revolut, "Header\nCARD_PAYMENT,Current,2026-02-12 11:30:00,2026-02-13 11:30:00,Lunch,-12,0,EUR,COMPLETED,100\n")]
    [InlineData(TransactionSource.ZabaBank, "Datum,Referencija,Opis,Uplata,Isplata,Saldo,Valuta\n12.02.2026,ref,Lunch,0,12,100,EUR\n")]
    public async Task ImportLinksMatchingExpenseAndReuploadAddsNoRows(TransactionSource source, string csv)
    {
        var purchaseDay = new DateTime(2026, 2, 12);
        var account = await SeedImportAccountAsync();
        await _session.SeedExpenseAsync(_session.User.Id, "lunch", purchaseDay, 12m);
        using (var db = _session.OpenDatabase())
        {
            var expense = await db.Expenses.SingleAsync(x => x.ValueId == "lunch");
            expense.DatePaid = purchaseDay.AddDays(2);
            await db.SaveChangesAsync();
        }

        async Task<JsonElement> GetListedTransactionsAsync(string query = "")
        {
            var expenses = (await GetAsync(query)).GetProperty("items").EnumerateArray();
            return Assert.Single(expenses).GetProperty("transactions");
        }

        Assert.Empty((await GetListedTransactionsAsync()).EnumerateArray());
        Assert.Empty((await GetListedTransactionsAsync("?hasTransactions=false")).EnumerateArray());
        Assert.Equal(0, (await GetAsync("?hasTransactions=true")).GetProperty("count").GetInt32());

        await UploadTransactionsAsync(account.ValueId, source, csv);
        var linked = Assert.Single((await GetListedTransactionsAsync()).EnumerateArray());
        Assert.Single((await GetListedTransactionsAsync("?hasTransactions=true")).EnumerateArray());
        Assert.Equal(0, (await GetAsync("?hasTransactions=false")).GetProperty("count").GetInt32());
        Assert.Equal(-12m, linked.GetProperty("amount").GetDecimal());
        Assert.Equal(purchaseDay, linked.GetProperty("created").GetDateTime().Date);

        await UploadTransactionsAsync(account.ValueId, source, csv);
        using var dbAfter = _session.OpenDatabase();
        var transaction = await dbAfter.Transactions.SingleAsync(x => x.AccountId == account.Id);
        Assert.Equal(transaction.ValueId, linked.GetProperty("id").GetString());
        Assert.Equal(transaction.Id, (await dbAfter.ExpenseTransactions.SingleAsync()).TransactionId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DiscountedDebitMatchesRoundedExpenseAmount(bool import)
    {
        var purchaseDay = new DateTime(2026, 2, 12);
        var account = await SeedImportAccountAsync(0.2174m);
        await _session.SeedExpenseAsync(_session.User.Id, "discounted", purchaseDay, 40m);
        await _session.SeedExpenseAsync(_session.User.Id, "full-amount", purchaseDay, 51.11m);

        if (import)
        {
            const string csv = """
                Datum,Referencija,Opis,Uplata,Isplata,Saldo,Valuta
                12.02.2026,1,Toll,0,51.11,100,EUR
                """;
            await UploadTransactionsAsync(account.ValueId, TransactionSource.ZabaBank, csv);
        }
        else
        {
            using (var db = _session.OpenDatabase())
            {
                db.Transactions.Add(new Transaction { AccountId = account.Id, Amount = -51.11m, Created = purchaseDay });
                await db.SaveChangesAsync();
            }

            using var response = await _client.PostAsync($"/account/{account.ValueId}/transaction/link", null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        using var dbAfter = _session.OpenDatabase();
        var link = await dbAfter.ExpenseTransactions
            .Select(x => new { x.Expense.ValueId, x.Transaction.Amount })
            .SingleAsync();
        Assert.Equal("discounted", link.ValueId);
        Assert.Equal(-51.11m, link.Amount);
    }

    [Fact]
    public async Task ImportLeavesIneligibleAndAmbiguousDebitsUnlinked()
    {
        var purchaseDay = new DateTime(2026, 2, 12);
        var account = await SeedImportAccountAsync();
        var userId = _session.User.Id;
        await _session.SeedExpenseAsync(userId, "currency", purchaseDay, 11m);
        await _session.SeedExpenseAsync(userId, "amount", purchaseDay, 22m);
        await _session.SeedExpenseAsync(userId, "day", purchaseDay.AddDays(1), 30m);
        await _session.SeedExpenseAsync(userId, "credit", purchaseDay, 40m);
        await _session.SeedExpenseAsync(_session.OtherUser.Id, "other-user", purchaseDay, 50m);
        await _session.SeedExpenseAsync(userId, "linked", purchaseDay, 60m);
        await _session.SeedExpenseAsync(userId, "duplicate-expense-1", purchaseDay, 70m);
        await _session.SeedExpenseAsync(userId, "duplicate-expense-2", purchaseDay, 70m);
        await _session.SeedExpenseAsync(userId, "duplicate-debit", purchaseDay, 80m);

        var existing = new Transaction { AccountId = account.Id, Amount = -60m, Created = purchaseDay.AddDays(-1) };
        using (var db = _session.OpenDatabase())
        {
            var otherCurrency = new Currency { Code = "USD", ValueId = "usd", Name = "US Dollar", Symbol = "$" };
            db.Currencies.Add(otherCurrency);
            (await db.Expenses.SingleAsync(x => x.ValueId == "currency")).Currency = otherCurrency;
            (await db.Expenses.SingleAsync(x => x.ValueId == "day")).DatePaid = purchaseDay;
            var linkedExpense = await db.Expenses.SingleAsync(x => x.ValueId == "linked");
            db.ExpenseTransactions.Add(new ExpenseTransaction { Expense = linkedExpense, Transaction = existing });
            await db.SaveChangesAsync();
        }

        const string csv = """
            Datum,Referencija,Opis,Uplata,Isplata,Saldo,Valuta
            12.02.2026,1,Currency mismatch,0,11,100,EUR
            12.02.2026,2,Amount mismatch,0,21,100,EUR
            12.02.2026,3,Day mismatch,0,30,100,EUR
            12.02.2026,4,Credit,40,0,100,EUR
            12.02.2026,5,Other user,0,50,100,EUR
            12.02.2026,6,Already linked,0,60,100,EUR
            12.02.2026,7,Two expenses,0,70,100,EUR
            12.02.2026,8,First debit,0,80,100,EUR
            12.02.2026,9,Second debit,0,80,100,EUR
            """;
        await UploadTransactionsAsync(account.ValueId, TransactionSource.ZabaBank, csv);

        using var dbAfter = _session.OpenDatabase();
        Assert.Equal(10, await dbAfter.Transactions.CountAsync(x => x.AccountId == account.Id));
        Assert.Equal(existing.Id, (await dbAfter.ExpenseTransactions.SingleAsync()).TransactionId);
    }

    [Fact]
    public async Task ImportMatchesParentAmountAndRequiresUniquenessAcrossCurrencies()
    {
        var purchaseDay = new DateTime(2026, 2, 12);
        var account = await SeedImportAccountAsync();
        var userId = _session.User.Id;
        await _session.SeedExpenseAsync(userId, "parent-only", purchaseDay, 25m);
        await _session.SeedExpenseAsync(userId, "same-match", purchaseDay, 14m);
        await _session.SeedExpenseAsync(userId, "direct-candidate", purchaseDay, 19m);
        await _session.SeedExpenseAsync(userId, "parent-candidate", purchaseDay, 19m);
        await _session.SeedExpenseAsync(userId, "two-debits", purchaseDay, 21m);

        using (var db = _session.OpenDatabase())
        {
            var otherCurrency = new Currency { Code = "USD", ValueId = "usd", Name = "US Dollar", Symbol = "$" };
            db.Currencies.Add(otherCurrency);
            var expenses = await db.Expenses.ToDictionaryAsync(x => x.ValueId);
            expenses["parent-only"].Currency = otherCurrency;
            expenses["parent-candidate"].Currency = otherCurrency;
            foreach (var (id, amount) in new[] { ("parent-only", 12m), ("same-match", 14m),
                         ("parent-candidate", 19m), ("two-debits", 22m) })
            {
                expenses[id].ParentCurrencyId = account.CurrencyId;
                expenses[id].ParentAmount = amount;
            }
            await db.SaveChangesAsync();
        }

        const string csv = """
            Datum,Referencija,Opis,Uplata,Isplata,Saldo,Valuta
            12.02.2026,1,Parent amount,0,12,100,EUR
            12.02.2026,2,Same direct and parent amount,0,14,100,EUR
            12.02.2026,3,Two expense candidates,0,19,100,EUR
            12.02.2026,4,Direct amount,0,21,100,EUR
            12.02.2026,5,Parent alternative,0,22,100,EUR
            """;
        await UploadTransactionsAsync(account.ValueId, TransactionSource.ZabaBank, csv);

        using var dbAfter = _session.OpenDatabase();
        var links = await dbAfter.ExpenseTransactions
            .Select(x => new { x.Expense.ValueId, x.Transaction.Amount })
            .ToDictionaryAsync(x => x.ValueId, x => x.Amount);
        Assert.Equal(2, links.Count);
        Assert.Equal(-12m, links["parent-only"]);
        Assert.Equal(-14m, links["same-match"]);
        Assert.Equal(5, await dbAfter.Transactions.CountAsync(x => x.AccountId == account.Id));
    }

    [Fact]
    public async Task LinkTransactionsReportsNewLinksAndAllRemainingUnlinkedTransactions()
    {
        var purchaseDay = new DateTime(2026, 2, 12);
        var account = await SeedImportAccountAsync();
        var userId = _session.User.Id;
        await _session.SeedExpenseAsync(userId, "match", purchaseDay, 12m);
        await _session.SeedExpenseAsync(userId, "ambiguous", purchaseDay, 20m);
        await _session.SeedExpenseAsync(userId, "already-linked", purchaseDay.AddDays(-1), 40m);
        await _session.SeedExpenseAsync(userId, "potential-expense-link", purchaseDay, 40m);
        await _session.SeedExpenseAsync(userId, "potential-income-link", purchaseDay, 50m);

        Transaction NewTransaction(decimal amount, int hour = 10) =>
            new() { AccountId = account.Id, Created = purchaseDay.AddHours(hour), Amount = amount };

        var matching = NewTransaction(-12m);
        var expenseLinked = NewTransaction(-40m);
        var incomeLinked = NewTransaction(-50m);
        using (var db = _session.OpenDatabase())
        {
            db.Transactions.AddRange(matching, NewTransaction(-20m), NewTransaction(-20m, 11),
                NewTransaction(-33m), NewTransaction(15m), NewTransaction(0m), expenseLinked, incomeLinked);
            var linkedExpense = await db.Expenses.SingleAsync(x => x.ValueId == "already-linked");
            db.ExpenseTransactions.Add(new ExpenseTransaction { Expense = linkedExpense, Transaction = expenseLinked });
            db.IncomeTransactions.Add(new IncomeTransaction
            {
                Transaction = incomeLinked,
                Income = new Income
                {
                    UserId = userId, ValueId = "linked-income", CurrencyId = _session.CurrencyId, Amount = 50m,
                    Date = purchaseDay, Created = purchaseDay, Modified = purchaseDay,
                    IncomeSource = new IncomeSource { UserId = userId, ValueId = "test-source", Name = "Test source" },
                    IncomeType = new IncomeType { ValueId = "test-type", Name = "Test type" }
                }
            });
            await db.SaveChangesAsync();
        }

        var url = $"/account/{account.ValueId}/transaction/link";
        using (var response = await _client.PostAsync(url, null))
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var counts = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(1, counts.GetProperty("linkedCount").GetInt32());
            Assert.Equal(5, counts.GetProperty("unlinkedCount").GetInt32());
            Assert.Equal(8, counts.GetProperty("totalTransactionCount").GetInt32());
        }

        using (var response = await _client.PostAsync(url, null))
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var counts = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(0, counts.GetProperty("linkedCount").GetInt32());
            Assert.Equal(5, counts.GetProperty("unlinkedCount").GetInt32());
            Assert.Equal(8, counts.GetProperty("totalTransactionCount").GetInt32());
        }

        using var dbAfter = _session.OpenDatabase();
        var links = await dbAfter.ExpenseTransactions
            .Select(x => new { x.Expense.ValueId, x.TransactionId })
            .ToDictionaryAsync(x => x.ValueId, x => x.TransactionId);
        Assert.Equal(2, links.Count);
        Assert.Equal(matching.Id, links["match"]);
        Assert.Equal(expenseLinked.Id, links["already-linked"]);
        Assert.Equal((int?)incomeLinked.Id, (await dbAfter.IncomeTransactions.SingleAsync()).TransactionId);
    }

    [Fact]
    public async Task LinkTransactionsReturnsNotFoundForMissingOrOtherUsersAccount()
    {
        var account = await SeedImportAccountAsync();
        using var otherClient = _session.Factory!.CreateUserClient(_session.OtherUser.Email);

        using var foreign = await otherClient.PostAsync($"/account/{account.ValueId}/transaction/link", null);
        Assert.Equal(HttpStatusCode.NotFound, foreign.StatusCode);

        using var missing = await _client.PostAsync("/account/missing/transaction/link", null);
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    private async Task<Account> SeedImportAccountAsync(decimal discountPercentage = 0m)
    {
        using var db = _session.OpenDatabase();
        var account = new Account
        {
            UserId = _session.User.Id, ValueId = "1", Name = "Import account",
            CurrencyId = _session.CurrencyId, Active = true, DiscountPercentage = discountPercentage
        };
        db.Accounts.Add(account);
        await db.SaveChangesAsync();
        return account;
    }

    private async Task UploadTransactionsAsync(string accountId, TransactionSource source, string csv)
    {
        using var content = new StringContent(csv, System.Text.Encoding.UTF8, "text/csv");
        using var response = await _client.PostAsync($"/account/{accountId}/transaction/import?transactionSource={source}", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
