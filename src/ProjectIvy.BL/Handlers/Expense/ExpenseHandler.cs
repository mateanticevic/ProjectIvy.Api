using Dapper;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.BL.MapExtensions;
using ProjectIvy.DL.Databases.Main.Queries;
using ProjectIvy.DL.Extensions.Entities;
using ProjectIvy.DL.Extensions;
using ProjectIvy.DL.Sql;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Vendor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using ProjectIvy.Model.View.ExpenseType;
using ProjectIvy.Model.View.Poi;
using View = ProjectIvy.Model.View.Expense;

namespace ProjectIvy.BL.Handlers.Expense
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
                int expenseId = context.Expenses.WhereUser(User).GetId(expenseValueId).Value;
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
                var result = context.Expenses.WhereUser(User.Id)
                                    .Where(binding, context);

                return result.Count();
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByDay(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Expenses.WhereUser(User.Id)
                                       .Where(binding, context)
                                       .GroupBy(x => x.Date)
                                       .OrderByDescending(x => x.Key)
                                       .Select(x => new KeyValuePair<DateTime, int>(x.Key, x.Count()))
                                       .ToList()
                                       .FillMissingDates(x => x.Key, x => new KeyValuePair<DateTime, int>(x, 0), binding.From, to)
                                       .Select(x => new KeyValuePair<string, int>(x.Key.ToString("yyyy-MM-dd"), x.Value));
            }
        }

        public IEnumerable<KeyValuePair<string, int>> CountByDayOfWeek(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Expenses.WhereUser(User.Id)
                    .Where(binding, context)
                    .GroupBy(x => x.Date.DayOfWeek)
                    .OrderByDescending(x => x.Key)
                    .Select(x => new KeyValuePair<DayOfWeek, int>(x.Key, x.Count()))
                    .ToList()
                    .Select(x => new KeyValuePair<string, int>(x.Key.ToString(), x.Value));
            }
        }

        public IEnumerable<GroupedByMonth<int>> CountByMonth(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Expenses.WhereUser(User.Id)
                                       .Where(binding, context)
                                       .GroupBy(x => new { x.Date.Year, x.Date.Month })
                                       .Select(x => new GroupedByMonth<int>(x.Count(), x.Key.Year, x.Key.Month))
                                       .ToList()
                                       .FillMissingMonths(datetime => new GroupedByMonth<int>(0, datetime.Year, datetime.Month), binding.From, to);
            }
        }

        public IEnumerable<GroupedByYear<int>> CountByYear(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var to = binding.To ?? DateTime.Now;

                return context.Expenses.WhereUser(User.Id)
                                       .Where(binding, context)
                                       .GroupBy(x => x.Date.Year)
                                       .Select(x => new GroupedByYear<int>(x.Count(), x.Key))
                                       .ToList()
                                       .FillMissingYears(year => new GroupedByYear<int>(0, year), binding.From?.Year, to.Year);
            }
        }

        public PagedView<PoiCount> CountByPoi(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(User.Id)
                                       .Where(binding, context)
                                       .Include(x => x.Poi)
                                       .GroupBy(x => x.Poi)
                                       .Select(x => new PoiCount(x.Key, x.Count()))
                                       .OrderByDescending(x => x.Count)
                                       .ToPagedView(binding);
            }
        }

        public PagedView<ExpenseTypeCount> CountByType(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(User.Id)
                                       .Where(binding, context)
                                       .Include(x => x.ExpenseType)
                                       .GroupBy(x => x.ExpenseType)
                                       .Select(x => new ExpenseTypeCount(x.Key, x.Count()))
                                       .OrderByDescending(x => x.Count)
                                       .ToPagedView(binding);
            }
        }

        public PagedView<VendorCount> CountByVendor(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(User.Id)
                                       .Where(binding, context)
                                       .Include(x => x.Vendor)
                                       .GroupBy(x => new {x.VendorId, x.Vendor })
                                       .Select(x => new VendorCount(x.Key.Vendor, x.Count()))
                                       .OrderByDescending(x => x.Count)
                                       .ToPagedView(binding);
            }
        }

        public int CountTypes(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(User)
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
                return context.Expenses.WhereUser(User)
                                       .Include(x => x.Vendor)
                                       .Where(binding, context)
                                       .Where(x => x.VendorId.HasValue)
                                       .GroupBy(x => x.VendorId)
                                       .Count();
            }
        }

        public string Create(ExpenseBinding binding)
        {
            using (var db = GetMainContext())
            {
                var entity = binding.ToEntity(db);
                entity.UserId = User.Id;
                entity.ValueId = db.Expenses.NextValueId(User.Id).ToString();

                db.Expenses.Add(entity);
                db.SaveChanges();

                return entity.ValueId;
            }
        }

        public bool Delete(string valueId)
        {
            using (var db = GetMainContext())
            {
                var entity = db.Expenses.WhereUser(User.Id)
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
                                              .WhereUser(User)
                                              .SingleOrDefault(x => x.ValueId == expenseId);

                return new View.Expense(expense);
            }
        }

        public PagedView<View.Expense> Get(ExpenseGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Expenses.WhereUser(User.Id)
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
                                       .WhereUser(User)
                                       .SingleOrDefault(x => x.ValueId == expenseId)
                                       .ExpenseFiles
                                       .Select(x => new View.ExpenseFile(x))
                                       .ToList();
            }
        }

        public IEnumerable<GroupedByMonth<decimal>> GetSumByMonth(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var from = binding.From ?? context.Expenses.WhereUser(User.Id).OrderBy(x => x.Date).FirstOrDefault().Date;
                var to = binding.To ?? DateTime.Now;

                var periods = from.RangeMonthsClosed(to)
                                  .Select(x => new FilteredBinding(x.from, x.to))
                                  .ToList();

                var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<decimal>>(x, GetSum(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To))));

                return tasks.Select(x => new GroupedByMonth<decimal>(x.Value.Result, x.Key.From.Value.Year, x.Key.From.Value.Month))
                            .OrderBy(binding);
            }
        }

        public IEnumerable<GroupedByYear<decimal>> GetSumByYear(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int startYear = context.Expenses.WhereUser(User.Id).OrderBy(x => x.Date).FirstOrDefault().Date.Year;
                int endYear = DateTime.Now.Year;

                var years = Enumerable.Range(startYear, endYear - startYear + 1);

                var periods = years.Select(x => new FilteredBinding(new DateTime(x, 1, 1), new DateTime(x, 12, 31)));

                var tasks = periods.Select(x => new KeyValuePair<int, Task<decimal>>(x.From.Value.Year, GetSum(binding.OverrideFromTo<ExpenseSumGetBinding>(x.From, x.To))));

                return tasks.Select(x => new GroupedByYear<decimal>(x.Value.Result, x.Key))
                            .OrderBy(binding);
            }
        }

        public async Task<IEnumerable<KeyValuePair<string, decimal>>> GetSumByTypeSum(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var result = context.Expenses.Include(x => x.ExpenseType)
                                             .WhereUser(User.Id);

                result = binding.From.HasValue ? result.Where(x => x.Date >= binding.From) : result;
                result = binding.To.HasValue ? result.Where(x => x.Date <= binding.To) : result;

                var types = result.Select(x => x.ExpenseType.ValueId).Distinct().ToList();

                var tasks = types.Select(x => new KeyValuePair<string, Task<decimal>>(x, GetSum(binding.Override(x))));

                await System.Threading.Tasks.Task.WhenAll(tasks.Select(x => x.Value));

                return tasks.Select(x => new KeyValuePair<string, decimal>(x.Key, x.Value.Result))
                            .OrderByDescending(x => x.Value);
            }
        }

        public async Task<decimal> GetSum(ExpenseSumGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int targetCurrencyId = context.GetCurrencyId(binding.TargetCurrencyId, User.Id);

                var expenseIds = context.Expenses.WhereUser(User)
                                                 .Where(binding, context)
                                                 .Select(x => x.Id)
                                                 .ToList();

                if (!expenseIds.Any())
                    return 0;

                using (var sql = GetSqlConnection())
                {
                    var parameters = new GetExpenseSumQuery()
                    {
                        ExpenseIds = expenseIds,
                        TargetCurrencyId = targetCurrencyId,
                        UserId = User.Id
                    };
                    return Math.Round(await sql.ExecuteScalarAsync<decimal>(SqlLoader.Load(MainSnippets.GetExpenseSumInDefaultCurrency), parameters), 2);
                }
            }
        }

        public bool Update(ExpenseBinding binding)
        {
            using (var context = GetMainContext())
            {
                var entity = context.Expenses.WhereUser(User.Id).SingleOrDefault(x => x.ValueId == binding.Id);

                entity = binding.ToEntity(context, entity);

                context.Expenses.Update(entity);
                context.SaveChanges();

                return true;
            }
        }
    }
}
