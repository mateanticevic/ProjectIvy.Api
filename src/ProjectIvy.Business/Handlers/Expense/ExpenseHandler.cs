using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Databases.Main.Queries;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Data.Sql;
using ProjectIvy.Data.Sql.Main.Scripts;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.ExpenseType;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.Business.Handlers.Expense
{
    public class ExpenseHandler : Handler<ExpenseHandler>, IExpenseHandler
    {
        public ExpenseHandler(IHandlerContext<ExpenseHandler> context) : base(context)
        {
        }

        public void AddFile(string expenseValueId, string fileValueId, ExpenseFileBinding binding)
        {
            using (var context = GetMainContext())
            {
                int fileId = context.Files.GetId(fileValueId).Value;
                int expenseId = context.Expenses.WhereUser(UserId).GetId(expenseValueId).Value;
                int expenseFileTypeId = context.ExpenseFileTypes.GetId(binding.TypeId).Value;

                var entity = new Model.Database.Main.Finance.ExpenseFile()
                {
                    ExpenseFileTypeId = expenseFileTypeId,
                    ExpenseId = expenseId,
                    FileId = fileId,
                    Name = binding.Name
                };

                context.ExpenseFiles.Add(entity);
                context.SaveChanges();
            }
        }

        public int Count(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var result = context.Expenses.WhereUser(UserId)
                                    .Where(binding, context);

                return result.Count();
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByDay(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Expenses.WhereUser(UserId)
                                       .Where(binding, context)
                                       .GroupBy(x => x.Date)
                                       .OrderByDescending(x => x.Key)
                                       .Select(x => new KeyValuePair<DateTime, int>(x.Key, x.Count()))
                                       .ToList()
                                       .FillMissingDates(x => x.Key, x => new KeyValuePair<DateTime, int>(x, 0), binding.From, to)
                                       .Select(x => new KeyValuePair<string, int>(x.Key.ToString("yyyy-MM-dd"), x.Value));
            }
        }

        public IEnumerable<KeyValuePair<int, int>> CountByDayOfWeek(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                DateTime FirstSunday = new DateTime(2000, 1, 2);
                var to = binding.To ?? DateTime.Now;

                return context.Expenses
                    .WhereUser(UserId)
                    .Where(binding, context)
                    .GroupBy(x => ((int)EF.Functions.DateDiffDay((DateTime?)FirstSunday, (DateTime?)x.Date) - 1) % 7)
                    .OrderBy(x => x.Key)
                    .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                    .ToList();
            }
        }

        public IEnumerable<KeyValuePair<int, int>> CountByMonth(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Expenses.WhereUser(UserId)
                                       .Where(binding, context)
                                       .GroupBy(x => x.Date.Month)
                                       .OrderBy(x => x.Key)
                                       .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                                       .ToList();
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByMonthOfYear(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Expenses.WhereUser(UserId)
                                       .Where(binding, context)
                                       .GroupBy(x => new { x.Date.Year, x.Date.Month })
                                       .Select(x => new GroupedByMonth<int>(x.Count(), x.Key.Year, x.Key.Month))
                                       .ToList()
                                       .FillMissingMonths(datetime => new GroupedByMonth<int>(0, datetime.Year, datetime.Month), binding.From, to)
                                       .Select(x => new KeyValuePair<string, int>($"{x.Year}-{x.Month}", x.Data));
            }
        }

        public IEnumerable<KeyValuePair<int, int>> CountByYear(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Expenses.WhereUser(UserId)
                                       .Where(binding, context)
                                       .GroupBy(x => x.Date.Year)
                                       .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                                       .ToList()
                                       .FillMissingYears(year => new KeyValuePair<int, int>(0, year), binding.From?.Year, to.Year);
            }
        }

        public PagedView<KeyValuePair<ExpenseType, int>> CountByType(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(UserId)
                                       .Where(binding, context)
                                       .Include(x => x.ExpenseType)
                                       .GroupBy(x => new
                                       {
                                           x.ExpenseType.Name,
                                           x.ExpenseType.ValueId
                                       })
                                       .OrderByDescending(x => x.Count())
                                       .Select(x => new KeyValuePair<ExpenseType, int>(new ExpenseType()
                                       {
                                           Id = x.Key.ValueId,
                                           Name = x.Key.Name,
                                       }, x.Count()))
                                       .ToPagedView(binding);
            }
        }

        public PagedView<KeyValuePair<Model.View.Vendor.Vendor, int>> CountByVendor(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(UserId)
                                       .Where(binding, context)
                                       .Include(x => x.Vendor)
                                       .GroupBy(x => new
                                       {
                                           x.Vendor.ValueId,
                                           x.Vendor.Name
                                       })
                                       .OrderByDescending(x => x.Count())
                                       .Select(x => new KeyValuePair<Model.View.Vendor.Vendor, int>(new() { Id = x.Key.ValueId, Name = x.Key.Name }, x.Count()))
                                       .ToPagedView(binding);
            }
        }

        public int CountTypes(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(UserId)
                                       .Include(x => x.Vendor)
                                       .Where(binding, context)
                                       .GroupBy(x => x.ExpenseTypeId)
                                       .Count();
            }
        }

        public int CountVendors(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(UserId)
                                       .Include(x => x.Vendor)
                                       .Where(binding, context)
                                       .Where(x => x.VendorId.HasValue)
                                       .GroupBy(x => x.VendorId)
                                       .Count();
            }
        }

        public string Create(ExpenseBinding binding)
        {
            if (!string.IsNullOrWhiteSpace(binding.VendorName))
                binding.VendorId = CreateVendor(binding.VendorName);

            using (var db = GetMainContext())
            {
                var entity = binding.ToEntity(db);
                entity.UserId = UserId;
                entity.ValueId = db.Expenses.NextValueId(UserId).ToString();

                db.Expenses.Add(entity);
                ResolveTransaction(db, entity);

                db.SaveChanges();

                return entity.ValueId;
            }
        }

        public bool Delete(string valueId)
        {
            using (var db = GetMainContext())
            {
                var entity = db.Expenses.WhereUser(UserId)
                                        .SingleOrDefault(x => x.ValueId == valueId);

                db.Expenses.Remove(entity);
                db.SaveChanges();

                return true;
            }
        }

        public View.Expense Get(string expenseId)
        {
            using (var context = GetMainContext())
            {
                var expense = context.Expenses.Include(x => x.ExpenseType)
                                              .Include(x => x.Currency)
                                              .Include(x => x.Poi)
                                              .Include(x => x.Vendor)
                                              .WhereUser(UserId)
                                              .SingleOrDefault(x => x.ValueId == expenseId);

                if (expense == null)
                    throw new ResourceNotFoundException();

                return new View.Expense(expense);
            }
        }

        public PagedView<View.Expense> Get(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(UserId)
                                       .IncludeAll()
                                       .Where(binding, context)
                                       .OrderBy(binding)
                                       .ThenByDescending(x => x.Created)
                                       .Select(x => new View.Expense(x))
                                       .ToPagedView(binding);
            }
        }

        public IEnumerable<View.ExpenseFile> GetFiles(string expenseId)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.IncludeAll()
                                       .WhereUser(UserId)
                                       .SingleOrDefault(x => x.ValueId == expenseId)
                                       .ExpenseFiles
                                       .Select(x => new View.ExpenseFile(x))
                                       .ToList();
            }
        }

        public async Task<IEnumerable<string>> GetTopDescriptions(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return await context.Expenses
                                    .WhereUser(UserId)
                                    .Where(binding, context)
                                    .Where(x => !string.IsNullOrEmpty(x.Comment))
                                    .GroupBy(x => x.Comment)
                                    .Select(x => new { x.Key, Count = x.Count() })
                                    .OrderByDescending(x => x.Count)
                                    .Take(5)
                                    .Select(x => x.Key)
                                    .ToListAsync();
            }
        }

        public async Task<decimal> SumAmount(ExpenseSumGetBinding binding)
            => await SumAmount(await SumBindingToQuery(binding));

        public async Task<IEnumerable<KeyValuePair<DateTime, decimal>>> SumAmountByDay(ExpenseSumGetBinding binding)
            => await SumAmountByDay(await SumBindingToQuery(binding));

        public async Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByDayOfWeek(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var tasks = Enumerable.Range(0, 7)
                                      .Select(x => (DayOfWeek)x)
                                      .Where(x => binding.Day == null || binding.Day.Contains(x))
                                      .ToList()
                                      .Select(x => new KeyValuePair<DayOfWeek, Task<decimal>>(
                                          x,
                                          SumAmount(binding.OverrideDay(x)))
                                      )
                                      .ToList();

                await Task.WhenAll(tasks.Select(x => x.Value));
                return tasks.Select(x => new KeyValuePair<int, decimal>(x.Key == DayOfWeek.Sunday ? 6 : (int)x.Key - 1, x.Value.Result))
                            .OrderBy(x => x.Key);
            }
        }

        public async Task<IEnumerable<KeyValuePair<int, decimal>>> SumAmountByMonth(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var tasks = Enumerable.Range(1, 12)
                                      .Select(x => new KeyValuePair<int, Task<decimal>>(
                                          x,
                                          SumAmount(binding.OverrideMonth(x))));

                await Task.WhenAll(tasks.Select(x => x.Value));
                return tasks.Select(x => new KeyValuePair<int, decimal>(x.Key, x.Value.Result));
            }
        }

        public IEnumerable<KeyValuePair<string, decimal>> SumAmountByMonthOfYear(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var from = binding.From ?? context.Expenses.WhereUser(UserId).OrderBy(x => x.Date).FirstOrDefault().Date;
                var to = binding.To ?? DateTime.Now;

                var periods = from.RangeMonthsClosed(to)
                                  .Select(x => new FilteredBinding(x.from, x.to))
                                  .ToList();

                var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<decimal>>(x, SumAmount(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To))));

                return tasks.Select(x => new KeyValuePair<string, decimal>($"{x.Key.From.Value.Year}-{x.Key.From.Value.Month}-1", x.Value.Result));
            }
        }

        public async Task<IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, decimal>>>>> SumByMonthOfYearByType(ExpenseSumGetBinding binding)
        {
            using var context = GetMainContext();
            var from = binding.From ?? context.Expenses.WhereUser(UserId).OrderBy(x => x.Date).FirstOrDefault().Date;
            var to = binding.To ?? DateTime.Now;

            var periods = from.RangeMonthsClosed(to)
                              .Select(x => new FilteredBinding(x.from, x.to))
                              .ToList();

            var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<IEnumerable<KeyValuePair<string, decimal>>>>(x, SumByType(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To))));
            await Task.WhenAll(tasks.Select(x => x.Value));

            return tasks.Select(x => new KeyValuePair<string, IEnumerable<KeyValuePair<string, decimal>>>($"{x.Key.From.Value.Year}-{x.Key.From.Value.Month}-1", x.Value.Result));
        }

        public IEnumerable<KeyValuePair<int, decimal>> SumAmountByYear(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int startYear = context.Expenses.WhereUser(UserId)
                                                .Where(binding, context)
                                                .OrderBy(x => x.Date)
                                                .FirstOrDefault().Date.Year;
                int endYear = binding.To?.Year ?? DateTime.Now.Year;

                var years = Enumerable.Range(startYear, endYear - startYear + 1);

                var periods = years.Select(x => new FilteredBinding(new DateTime(x, 1, 1), new DateTime(x, 12, 31)));

                var tasks = periods.Select(x => new KeyValuePair<int, Task<decimal>>(x.From.Value.Year, SumAmount(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To))));

                return tasks.Select(x => new KeyValuePair<int, decimal>(x.Key, x.Value.Result));
            }
        }

        public async Task<IEnumerable<KeyValuePair<short, IEnumerable<KeyValuePair<string, decimal>>>>> SumByYearByType(ExpenseSumGetBinding binding)
        {
            using var context = GetMainContext();
            int startYear = context.Expenses.WhereUser(UserId)
                                .Where(binding, context)
                                .OrderBy(x => x.Date)
                                .FirstOrDefault().Date.Year;
            int endYear = binding.To?.Year ?? DateTime.Now.Year;

            var years = Enumerable.Range(startYear, endYear - startYear + 1);

            var periods = years.Select(x => new FilteredBinding(new DateTime(x, 1, 1), new DateTime(x, 12, 31)));

            var tasks = periods.Select(x => new KeyValuePair<short, Task<IEnumerable<KeyValuePair<string, decimal>>>>((short)x.From.Value.Year, SumByType(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To))));
            await Task.WhenAll(tasks.Select(x => x.Value));

            return tasks.Select(x => new KeyValuePair<short, IEnumerable<KeyValuePair<string, decimal>>>(x.Key, x.Value.Result));
        }

        public async Task<IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>> SumByCurrency(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return await context.Expenses.WhereUser(UserId)
                                             .Where(binding, context)
                                             .Include(x => x.Currency)
                                             .GroupBy(x => new { x.Currency.ValueId, x.Currency.Name })
                                             .Select(x => new KeyValuePair<Model.View.Currency.Currency, decimal>(
                                                 new()
                                                 {
                                                     Id = x.Key.ValueId,
                                                     Name = x.Key.Name
                                                 }, x.Sum(y => y.Amount)
                                                 ))
                                             .ToListAsync();
            }
        }

        public async Task<IEnumerable<KeyValuePair<string, decimal>>> SumByType(ExpenseSumGetBinding binding)
        {
            using var sql = GetSqlConnection();
            var results = await sql.QueryAsync<(int TypeId, string TypeValueId, decimal Amount)>(SqlLoader.Load(SqlScripts.GetExpenseSumByType), await SumBindingToQuery(binding));

            using var context = GetMainContext();

            if (binding.ByBaseType)
            {
                var typeIds = results.Select(x => x.TypeId).ToList();
                var types = context.ExpenseTypes.GetAll()
                                                .Where(x => typeIds.Contains(x.Id))
                                                .Select(x => (x.Id, x.ToParentType().ValueId))
                                                .Distinct()
                                                .ToList();

                return results.Join(types, x => x.TypeId, x => x.Id, (a, b) => (b.ValueId, a.Amount))
                              .GroupBy(x => x.ValueId)
                              .Select(x => new KeyValuePair<string, decimal>(x.Key, Math.Round(x.Sum(y => y.Amount), 2)));
            }
            else
                return results.Select(x => new KeyValuePair<string, decimal>(x.TypeValueId, Math.Round(x.Amount, 2))).ToList();
        }

        public bool Update(ExpenseBinding binding)
        {
            if (!string.IsNullOrWhiteSpace(binding.VendorName))
                binding.VendorId = CreateVendor(binding.VendorName);

            using (var context = GetMainContext())
            {
                var entity = context.Expenses.WhereUser(UserId)
                                             .Include(x => x.Transaction)
                                             .SingleOrDefault(x => x.ValueId == binding.Id);

                entity = binding.ToEntity(context, entity);

                context.Expenses.Update(entity);
                ResolveTransaction(context, entity);

                context.SaveChanges();

                return true;
            }
        }

        private string CreateVendor(string name)
        {
            using (var context = GetMainContext())
            {
                var entity = new Model.Database.Main.Finance.Vendor()
                {
                    Name = name,
                    ValueId = name.Replace(" ", "-").ToLowerInvariant()
                };
                context.Vendors.Add(entity);
                context.SaveChanges();
                return entity.ValueId;
            }
        }

        private void ResolveTransaction(MainContext context, Model.Database.Main.Finance.Expense expense)
        {
            if (expense.Transaction is not null)
                context.Transactions.Remove(expense.Transaction);

            if (expense.PaymentTypeId == (int)Model.Constants.Database.PaymentType.Cash)
            {
                int? accountId = context.Accounts.SingleOrDefault(x => !x.BankId.HasValue && x.CurrencyId == expense.CurrencyId)?.Id;

                if (accountId.HasValue)
                {
                    var transaction = new Model.Database.Main.Finance.Transaction()
                    {
                        AccountId = accountId.Value,
                        Amount = -expense.Amount,
                        Created = expense.Date
                    };
                    expense.Transaction = transaction;
                    context.Transactions.Add(transaction);
                }
            }
        }

        private async Task<decimal> SumAmount(GetExpenseSumQuery query)
        {
            if (query is null)
                return 0;

            using (var sql = GetSqlConnection())
            {
                return Math.Round(await sql.ExecuteScalarAsync<decimal>(SqlLoader.Load(SqlScripts.GetExpenseSumInDefaultCurrency), query), 2);
            }
        }

        private async Task<IEnumerable<KeyValuePair<DateTime, decimal>>> SumAmountByDay(GetExpenseSumQuery query)
        {
            using (var sql = GetSqlConnection())
            {
                return (await sql.QueryAsync<(DateTime, decimal)>(SqlLoader.Load(SqlScripts.GetExpenseSumByDay), query))
                    .Select(x => new KeyValuePair<DateTime, decimal>(x.Item1, Math.Round(x.Item2, 2)))
                    .ToList();
            }
        }

        private async Task<GetExpenseSumQuery> SumBindingToQuery(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int targetCurrencyId = context.GetCurrencyId(binding.TargetCurrencyId, UserId);

                var expenseIds = await context.Expenses.WhereUser(UserId)
                                                       .Where(binding, context)
                                                       .Select(x => x.Id)
                                                       .ToListAsync();

                if (!expenseIds.Any())
                    return null;

                return new GetExpenseSumQuery()
                {
                    ExpenseIds = expenseIds,
                    TargetCurrencyId = targetCurrencyId,
                    UserId = UserId
                };
            }
        }
    }
}
