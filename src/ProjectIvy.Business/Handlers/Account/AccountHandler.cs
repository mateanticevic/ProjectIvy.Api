using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Account;
using ProjectIvy.Model.Binding.Transaction;
using ProjectIvy.Model.Database.Main.Finance;
using View = ProjectIvy.Model.View.Account;

namespace ProjectIvy.Business.Handlers.Account;
public class AccountHandler : Handler<AccountHandler>, IAccountHandler
{
    public AccountHandler(IHandlerContext<AccountHandler> context) : base(context)
    {
    }

    public async Task CreateTransaction(string accountValueId, TransactionBinding binding)
    {
        using var context = GetMainContext();

        int accountId = context.Accounts.GetId(accountValueId).Value;
        var lastTransaction = await context.Transactions.Where(x => x.AccountId == accountId)
                                                        .OrderByDescending(x => x.Created)
                                                        .FirstOrDefaultAsync();

        var entity = new Transaction()
        {
            AccountId = accountId,
            Amount = binding.Amount,
            Balance = lastTransaction is null ? 0 : lastTransaction.Balance + binding.Amount,
            Created  = binding.Created
        };

        await context.Transactions.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<View.Account>> Get(AccountGetBinding b)
    {
        using (var context = GetMainContext())
        {
            return await context.Accounts.Include(x => x.Bank)
                                         .Include(x => x.Currency)
                                         .WhereIf(b.IsActive, x => x.Active == b.IsActive)
                                         .Select(x =>
                                            new View.Account(x)
                                            {
                                                Balance = x.Transactions.Sum(x => x.Amount)
                                            })
                                         .ToListAsync();
        }
    }

    public async Task<View.AccountOverview> GetOverview(string accountValueId)
    {
        using (var context = GetMainContext())
        {
            int accountId = context.Accounts.GetId(accountValueId).Value;
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
    }

    public async Task ProcessHacTransactions(string accountKey, string csv)
    {
        using (var context = GetMainContext())
        {
            int accountId = context.Accounts.GetId(accountKey).Value;

            var transactions = new List<Transaction>();
            foreach (string item in csv.Split("\r\n").Skip(1).Reverse().Skip(1))
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                string[] parts = ParseCsvLine(item, ';').ToArray();

                decimal amountIn = Convert.ToDecimal(parts[7].Replace(",", "."));
                decimal amountOut = Convert.ToDecimal(parts[8].Replace(",", "."));
                decimal balance = Convert.ToDecimal(parts[9].Replace(",", ".").Replace(" ", string.Empty));

                decimal amount = amountIn > 0 ? amountIn : amountOut * -1;

                string dateTimeFormat = "dd.MM.yyyy HH:mm:ss";

                var transaction = new Transaction()
                {
                    AccountId = accountId,
                    Amount = amount,
                    Balance = balance,
                    Created = amount < 0 ? DateTime.ParseExact(parts[3], dateTimeFormat, CultureInfo.InvariantCulture) : DateTime.ParseExact(parts[2], dateTimeFormat, CultureInfo.InvariantCulture)
                };

                if (amount < 0)
                {
                    transaction.Completed = DateTime.ParseExact(parts[2], dateTimeFormat, CultureInfo.InvariantCulture);
                    transaction.Description = $"{parts[0]} [{parts[6]}]";
                }

                transactions.Add(transaction);
            }
            
            foreach (var transaction in transactions)
            {
                if (context.Transactions.Any(x => x.Created == transaction.Created
                                                && x.Amount == transaction.Amount
                                                && x.AccountId == accountId))
                    continue;
                await context.Transactions.AddAsync(transaction);
            }

            await context.SaveChangesAsync();
        }
    }

    public async Task ProcessRevolutTransactions(string accountKey, string csv)
    {
        using (var context = GetMainContext())
        {
            int accountId = context.Accounts.GetId(accountKey).Value;

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
    }

    public async Task ProcessOtpBankTransactions(string accountKey, string csv)
    {
        using (var context = GetMainContext())
        {
            int accountId = context.Accounts.GetId(accountKey).Value;

            var transactions = new List<Transaction>();
            foreach (string item in csv.Split("\r\n").Skip(1))
            {
                if (string.IsNullOrWhiteSpace(item))
                    continue;

                string[] parts = ParseCsvLine(item, ';').ToArray();

                string descritpion = parts[2];
                var dateTimeRe = new Regex("[0-9]{2}/[0-9]{2}/[0-9]{4} [0-9]{2}:[0-9]{2}");
                var match = dateTimeRe.Match(descritpion);

                decimal amount = Convert.ToDecimal(parts[3].Replace(".", string.Empty).Replace(",", "."));
                if (amount == 0)
                    continue;

                var transaction = new Transaction()
                {
                    AccountId = accountId,
                    Amount = amount,
                    Description = descritpion,
                    Created = match.Success ? DateTime.ParseExact(match.Value, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture) : DateTime.ParseExact(parts[1], "dd.MM.yyyy", CultureInfo.InvariantCulture)
                };

                if (decimal.TryParse(parts[4].Replace(".", string.Empty).Replace(",", "."), out decimal balance))
                    transaction.Balance = balance;

                if (match.Success)
                    transaction.Completed = DateTime.ParseExact(parts[1], "dd.MM.yyyy", CultureInfo.InvariantCulture);

                transactions.Add(transaction);
            }

            foreach (var transaction in transactions)
            {
                if (context.Transactions.Any(x => x.Created == transaction.Created
                                                && x.Description == transaction.Description
                                                && x.AccountId == accountId))
                    continue;
                await context.Transactions.AddAsync(transaction);
            }

            await context.SaveChangesAsync();
        }
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
}