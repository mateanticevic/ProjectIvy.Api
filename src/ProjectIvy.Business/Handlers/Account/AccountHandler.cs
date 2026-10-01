using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Account;
using ProjectIvy.Model.Binding.Transaction;
using ProjectIvy.Model.Database.Main.Finance;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Account;

namespace ProjectIvy.Business.Handlers.Account;

public class AccountHandler : Handler<AccountHandler>, IAccountHandler
{
    public AccountHandler(IHandlerContext<AccountHandler> context) : base(context)
    {
    }

    public async Task<string> Create(AccountBinding binding)
    {
        using var context = GetMainContext();

        var entity = await binding.ToEntity(context);
        entity.UserId = UserId;
        entity.ValueId = (await context.Accounts.NextValueIdAsync(UserId)).ToString();

        await context.Accounts.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity.ValueId;
    }

    public async Task CreateTransaction(string accountValueId, TransactionBinding binding)
    {
        using var context = GetMainContext();

        int accountId = (await context.Accounts.WhereUser(UserId)
                                        .GetIdAsync(accountValueId)).Value;
        var lastTransaction = await context.Transactions.Where(x => x.AccountId == accountId)
                                                        .OrderByDescending(x => x.Created)
                                                        .FirstOrDefaultAsync();

        var entity = new Transaction()
        {
            AccountId = accountId,
            Amount = binding.Amount,
            Balance = lastTransaction is not null ? (lastTransaction.Balance is null ? null : lastTransaction.Balance + binding.Amount) : 0,
            Created = binding.Created
        };

        await context.Transactions.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<PagedView<View.Account>> Get(AccountGetBinding b)
    {
        using var context = GetMainContext();
        const int baseCurrencyId = 3;
        int defaultCurrencyId = (await context.Users.SingleOrDefaultAsync(x => x.Id == UserId))!.DefaultCurrencyId;

        var accountsQuery = context.Accounts.WhereUser(UserId)
                                     .Include(x => x.Bank)
                                     .Include(x => x.Currency)
                                     .Where(b)
                                     .Select(x => new
                                     {
                                         Account = x,
                                         Balance = x.Transactions.Sum(t => t.Amount),
                                         RateToBase = x.CurrencyId == baseCurrencyId ? (decimal?)1 : context.CurrencyRates
                                             .Where(r => r.FromCurrencyId == baseCurrencyId
                                                      && r.ToCurrencyId == x.CurrencyId)
                                             .OrderByDescending(r => r.Timestamp)
                                             .Select(r => (decimal?)r.Rate)
                                             .FirstOrDefault(),
                                         RateFromBase = defaultCurrencyId == baseCurrencyId ? (decimal?)1 : context.CurrencyRates
                                             .Where(r => r.FromCurrencyId == baseCurrencyId
                                                      && r.ToCurrencyId == defaultCurrencyId)
                                             .OrderByDescending(r => r.Timestamp)
                                             .Select(r => (decimal?)r.Rate)
                                             .FirstOrDefault()
                                     });

        var totalCount = await accountsQuery.CountAsync();
        var accounts = await accountsQuery
                                     .Skip(b.Page * b.PageSize)
                                     .Take(b.PageSize)
                                     .ToListAsync();

        var items = accounts.Select(x =>
        {
            var balance = x.Balance;
            decimal balanceInDefaultCurrency = x.RateToBase.HasValue && x.RateFromBase.HasValue
                    ? balance / x.RateToBase.Value * x.RateFromBase.Value
                    : 0;

            return new View.Account(x.Account)
            {
                Balance = balance,
                BalanceInDefaultCurrency = Math.Round(balanceInDefaultCurrency, 3)
            };
        }).ToList();

        return new PagedView<View.Account>
        {
            Items = items,
            Count = totalCount
        };
    }

    public async Task<decimal> GetNetWorth()
    {
        using var context = GetMainContext();
        int targetCurrencyId = await context.Users.Where(x => x.Id == UserId)
                                            .Select(x => x.DefaultCurrencyId)
                                            .SingleAsync();

        var accountBalances = await context.Accounts.WhereUser(UserId)
                                .Include(x => x.Transactions)
                                .Select(x => new
                                {
                                    x.CurrencyId,
                                    Balance = x.Transactions.Sum(x => x.Amount)
                                })
                                .ToListAsync();

        decimal total = 0;
        foreach (var x in accountBalances)
        {
            if (x.CurrencyId == targetCurrencyId)
            {
                total += x.Balance;
                continue;
            }

            decimal? rate = await context.CurrencyRates.Where(y => y.FromCurrencyId == x.CurrencyId
                                                        && y.ToCurrencyId == targetCurrencyId)
                                                .OrderByDescending(r => r.Timestamp)
                                                .Select(r => (decimal?)r.Rate)
                                                .FirstOrDefaultAsync();

            if (rate is null)
                continue;

            total += x.Balance * rate.Value;
        }

        return total;
    }

    public async Task<View.AccountOverview> GetOverview(string accountValueId)
    {
        using var context = GetMainContext();
        int accountId = (await context.Accounts.WhereUser(UserId)
                                        .GetIdAsync(accountValueId)).Value;
        decimal sumIn = await context.Transactions.Where(x => x.AccountId == accountId && x.Amount > 0)
                                                  .SumAsync(x => x.Amount);

        decimal sumOut = await context.Transactions.Where(x => x.AccountId == accountId && x.Amount < 0)
                                                   .SumAsync(x => x.Amount);

        return new View.AccountOverview()
        {
            SumIn = sumIn,
            SumOut = sumOut
        };
    }

    public async Task<PagedView<View.Transaction>> GetTransactions(string accountValueId, FilteredPagedBinding b)
    {
        using var context = GetMainContext();
        int accountId = (await context.Accounts.WhereUser(UserId)
                                        .GetIdAsync(accountValueId)).Value;

        return await context.Transactions.Where(x => x.AccountId == accountId)
                                         .OrderByDescending(x => x.Created)
                                         .Select(x => new View.Transaction(x))
                                         .ToPagedViewAsync(b);
    }

    private IEnumerable<string> ParseCsvLine(string line, char separator = ',')
    {
        var sb = new StringBuilder();
        bool insideQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
                insideQuotes = !insideQuotes;
            else if (c == separator && !insideQuotes)
            {
                if (sb.Length == 0)
                    yield return string.Empty;
                else
                {
                    string field = sb.ToString();
                    sb.Clear();
                    yield return field;
                }

                if (line.Length == i + 1)
                    yield return string.Empty;
            }
            else
                sb.Append(c);
        }

        yield return sb.ToString();
    }

    public async Task ProcessHacTransactions(string accountKey, string csv)
    {
        using var context = GetMainContext();
        int accountId = (await context.Accounts.WhereUser(UserId)
                                        .GetIdAsync(accountKey)).Value;

        var transactions = new List<Transaction>();
        foreach (string item in csv.Split("\r\n").Skip(1).Reverse().Skip(1))
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;

            string[] parts = ParseCsvLine(item, ';').ToArray();

            decimal amountIn = Convert.ToDecimal(parts[9].Replace(",", "."));
            decimal amountOut = Convert.ToDecimal(parts[10].Replace(",", "."));
            decimal balance = Convert.ToDecimal(parts[11].Replace(",", ".").Replace(" ", string.Empty));

            decimal amount = amountIn > 0 ? amountIn : amountOut * -1;

            string dateTimeFormat = "dd.MM.yyyy HH:mm:ss";

            var transaction = new Transaction()
            {
                AccountId = accountId,
                Amount = amount,
                Balance = balance,
                Created = amount < 0 ? DateTime.ParseExact(parts[4], dateTimeFormat, CultureInfo.InvariantCulture) : DateTime.ParseExact(parts[3], dateTimeFormat, CultureInfo.InvariantCulture)
            };

            if (amount < 0)
            {
                transaction.Completed = DateTime.ParseExact(parts[3], dateTimeFormat, CultureInfo.InvariantCulture);
                transaction.Description = $"{parts[1]} [{parts[7]}]";
            }

            transactions.Add(transaction);
        }

        foreach (var transaction in transactions)
        {
            if (await context.Transactions.AnyAsync(x => x.Created == transaction.Created
                                            && x.Amount == transaction.Amount
                                            && x.AccountId == accountId))
                continue;
            await context.Transactions.AddAsync(transaction);
        }

        await context.SaveChangesAsync();
    }

    public async Task ProcessOtpBankTransactions(string accountKey, string csv)
    {
        using var context = GetMainContext();
        int accountId = (await context.Accounts.WhereUser(UserId)
                                        .GetIdAsync(accountKey)).Value;

        var transactions = new List<Transaction>();
        foreach (string item in csv.Split("\r\n").Skip(1))
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;

            string[] parts = ParseCsvLine(item, ',').ToArray();

            string descritpion = parts[2];
            var dateTimeRe = new Regex("[0-9]{2}/[0-9]{2}/[0-9]{4} [0-9]{2}:[0-9]{2}");
            var match = dateTimeRe.Match(descritpion);

            decimal amount = Convert.ToDecimal(parts[3].Replace(",", string.Empty));
            if (amount == 0)
                continue;

            var transaction = new Transaction()
            {
                AccountId = accountId,
                Amount = amount,
                Description = descritpion,
                Created = match.Success ? DateTime.ParseExact(match.Value, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) : DateTime.ParseExact(parts[1], "dd.MM.yyyy", CultureInfo.InvariantCulture)
            };

            if (decimal.TryParse(parts[4].Replace(",", string.Empty), out decimal balance))
                transaction.Balance = balance;

            if (match.Success)
                transaction.Completed = DateTime.ParseExact(parts[1], "dd.MM.yyyy", CultureInfo.InvariantCulture);

            transactions.Add(transaction);
        }

        foreach (var transaction in transactions)
        {
            if (await context.Transactions.AnyAsync(x => x.Created == transaction.Created
                                            && x.Description == transaction.Description
                                            && x.AccountId == accountId))
                continue;
            await context.Transactions.AddAsync(transaction);
        }

        await context.SaveChangesAsync();
    }

    public async Task ProcessZabaBankTransactions(string accountKey, string csv)
    {
        using var context = GetMainContext();
        int accountId = (await context.Accounts.WhereUser(UserId)
                                        .GetIdAsync(accountKey)).Value;

        var transactions = new List<Transaction>();
        // Columns: Datum, Referencija, Opis, Uplata, Isplata, Saldo, Valuta.
        foreach (string item in csv.Split('\n').Skip(1))
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;

            string[] parts = ParseCsvLine(item.TrimEnd('\r'), ',').ToArray();
            decimal amount = decimal.Parse(parts[3], NumberStyles.Number, CultureInfo.InvariantCulture)
                           - decimal.Parse(parts[4], NumberStyles.Number, CultureInfo.InvariantCulture);
            if (amount == 0)
                continue;

            var transaction = new Transaction()
            {
                AccountId = accountId,
                Amount = amount,
                Description = parts[2],
                Created = DateTime.ParseExact(parts[0], "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Balance = decimal.Parse(parts[5], NumberStyles.Number, CultureInfo.InvariantCulture)
            };

            if (transactions.Any(x => x.Created == transaction.Created
                                  && x.Description == transaction.Description
                                  && x.Amount == transaction.Amount))
                continue;

            if (await context.Transactions.AnyAsync(x => x.AccountId == accountId
                                                     && x.Created == transaction.Created
                                                     && x.Description == transaction.Description
                                                     && x.Amount == transaction.Amount))
                continue;

            transactions.Add(transaction);
        }

        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();
    }

    public async Task ProcessRevolutTransactions(string accountKey, string csv)
    {
        using var context = GetMainContext();
        int accountId = (await context.Accounts.WhereUser(UserId)
                                        .GetIdAsync(accountKey)).Value;

        var transactions = new List<Transaction>();
        var existingTimestamps = await context.Transactions.Where(x => x.AccountId == accountId)
                                                           .Select(x => x.Created)
                                                           .ToListAsync();

        foreach (string item in csv.Split("\n").Skip(1))
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;

            string[] parts = ParseCsvLine(item, ';').ToArray();
            var transaction = new Transaction()
            {
                AccountId = accountId,
                Amount = Convert.ToDecimal(parts[5]) - Convert.ToDecimal(parts[6]),
                Description = parts[4],
                Created = DateTime.Parse(parts[2]),
                Type = parts[0]
            };

            if (existingTimestamps.Contains(transaction.Created))
                continue;

            if (decimal.TryParse(parts[9].Replace("\r", string.Empty), out decimal balance))
                transaction.Balance = balance;

            if (DateTime.TryParse(parts[3], out DateTime completed))
                transaction.Completed = completed;

            transactions.Add(transaction);
        }

        await context.Transactions.AddRangeAsync(transactions);
        await context.SaveChangesAsync();
    }

    public async Task Update(string accountValueId, AccountBinding binding)
    {
        using var context = GetMainContext();

        var entity = await context.Accounts.WhereUser(UserId)
                                           .SingleOrDefaultAsync(x => x.ValueId == accountValueId) ?? throw new ResourceNotFoundException();
        await binding.ToEntity(context, entity);
        await context.SaveChangesAsync();
    }
}
