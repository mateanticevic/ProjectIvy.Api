using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.Extensions.Entities;
using AnticevicApi.DL.Extensions;
using AnticevicApi.DL.Sql;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using View = AnticevicApi.Model.View.Expense;
using System.Threading.Tasks;
using System.Collections;

namespace AnticevicApi.BL.Handlers.Expense
{
    public class ExpenseHandler : Handler<ExpenseHandler>, IExpenseHandler
    {
        public ExpenseHandler(IHandlerContext<ExpenseHandler> context) : base(context)
        {
        }

        public string Create(ExpenseBinding binding)
        {
            using (var db = GetMainContext())
            {
                var entity = binding.ToEntity(db);
                entity.UserId = User.Id;

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

        #region Get

        public PaginatedView<View.Expense> Get(ExpenseGetBinding binding)
        {
            var view = new PaginatedView<View.Expense>();

            using (var db = GetMainContext())
            {
                int? currencyId = db.Currencies.GetId(binding.CurrencyId);
                int? expenseTypeId = db.ExpenseTypes.GetId(binding.TypeId);
                int? vendorId = db.Vendors.GetId(binding.VendorId);

                var result = db.Expenses.Include(x => x.ExpenseType)
                                        .Include(x => x.Currency)
                                        .Include(x => x.Vendor)
                                        .WhereUser(User.Id);

                result = binding.From.HasValue ? result.Where(x => x.Date >= binding.From) : result;
                result = binding.To.HasValue ? result.Where(x => x.Date <= binding.To) : result;

                result = expenseTypeId.HasValue ? result.Where(x => x.ExpenseTypeId == expenseTypeId) : result;
                result = vendorId.HasValue ? result.Where(x => x.VendorId == vendorId) : result;
                result = currencyId.HasValue ? result.Where(x => x.CurrencyId == currencyId) : result;

                view.Count = result.Count();

                result = result.OrderByDescending(x => x.Date)
                               .ThenByDescending(x => x.Id)
                               .Page(binding.Page, binding.PageSize);

                view.Items = result.ToList().Select(x => new View.Expense(x));

                return view;
            }
        }

        public IEnumerable<View.Expense> GetByDate(DateTime date)
        {
            using (var db = GetMainContext())
            {
                return db.Expenses.WhereUser(User.Id)
                                  .Where(x => x.Date.Date == date)
                                  .ToList()
                                  .Select(x => new View.Expense(x));
            }
        }

        public int GetCount(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                var expenses = db.Expenses.WhereUser(User.Id);

                expenses = binding.From.HasValue ? expenses.Where(x => x.Date >= binding.From) : expenses;
                expenses = binding.To.HasValue ? expenses.Where(x => x.Date <= binding.To) : expenses;

                return expenses.Count();
            }
        }

        public async Task<IEnumerable<GroupedByMonth<decimal>>> GetGroupedByMonthSum(string currencyValueId)
        {
            using (var context = GetMainContext())
            {
                var startDate = context.Expenses.WhereUser(User.Id).OrderBy(x => x.Date).FirstOrDefault().Date;
                var endDate = DateTime.Now;

                var periods = new List<FilteredBinding>();

                foreach (var year in Enumerable.Range(startDate.Year, endDate.Year - startDate.Year + 1))
                {
                    int startMonth = startDate.Year == year ? startDate.Month : 1;
                    int endMonth = endDate.Year == year ? endDate.Month : 12;

                    foreach (var month in Enumerable.Range(startMonth, endMonth - startMonth + 1))
                    {
                        var startOfMonth = new DateTime(year, month, 1);
                        var endOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                        periods.Add(new FilteredBinding(startOfMonth, endOfMonth));
                    }
                }

                var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<decimal>>(x, GetSum(x, currencyValueId)));

                await System.Threading.Tasks.Task.WhenAll(tasks.Select(x => x.Value));

                return tasks.Select(x => new GroupedByMonth<decimal>(x.Value.Result, x.Key.From.Value.Year, x.Key.From.Value.Month));
            }
        }

        public async Task<IEnumerable<GroupedByYear<decimal>>> GetGroupedByYearSum(string currencyValueId)
        {
            using (var context = GetMainContext())
            {
                int startYear = context.Expenses.WhereUser(User.Id).OrderBy(x => x.Date).FirstOrDefault().Date.Year;
                int endYear = DateTime.Now.Year;

                var years = Enumerable.Range(startYear, endYear - startYear + 1);

                var periods = years.Select(x => new FilteredBinding(new DateTime(x, 1, 1), new DateTime(x, 12, 31)));

                var tasks = periods.Select(x => new KeyValuePair<int, Task<decimal>>(x.From.Value.Year, GetSum(x, currencyValueId)));

                await System.Threading.Tasks.Task.WhenAll(tasks.Select(x => x.Value));

                return tasks.Select(x => new GroupedByYear<decimal>(x.Value.Result, x.Key));
            }
        }

        public async Task<decimal> GetSum(FilteredBinding binding, string currencyCode)
        {
            using (var db = GetMainContext())
            {
                int targetCurrencyId = db.GetCurrencyId(currencyCode, User.Id);

                using (var sql = GetSqlConnection())
                {
                    var parameters = new
                    {
                        TargetCurrencyId = targetCurrencyId,
                        From = binding.From,
                        To = binding.To,
                        UserId = User.Id
                    };
                    return Math.Round(sql.ExecuteScalar<decimal>(SqlLoader.Load(MainSnippets.GetExpenseSumInDefaultCurrency), parameters), 2);
                }
            }
        }

        public IEnumerable<View.Expense> GetByVendor(string valueId, DateTime? from = null, DateTime? to = null)
        {
            using (var db = GetMainContext())
            {
                var c = db.Vendors.SingleOrDefault(x => x.ValueId == valueId);

                var expenses = db.Vendors.Include(x => x.Expenses)
                                 .ThenInclude(x => x.Currency)
                                 .SingleOrDefault(x => x.ValueId == valueId)
                                 .Expenses
                                 .WhereUser(User.Id);

                expenses = from.HasValue ? expenses.Where(x => x.Date >= from) : expenses;
                expenses = to.HasValue ? expenses.Where(x => x.Date <= to) : expenses;

                return expenses.ToList().Select(x => new View.Expense(x));
            }
        }

        public IEnumerable<View.Expense> GetByType(string valueId, DateTime? from = null, DateTime? to = null)
        {
            using (var db = GetMainContext())
            {
                var c = db.Vendors.SingleOrDefault(x => x.ValueId == valueId);

                var expenses = db.ExpenseTypes.Include(x => x.Expenses)
                                 .ThenInclude(x => x.Currency)
                                 .SingleOrDefault(x => x.ValueId == valueId)
                                 .Expenses
                                 .WhereUser(User.Id);

                expenses = from.HasValue ? expenses.Where(x => x.Date >= from) : expenses;
                expenses = to.HasValue ? expenses.Where(x => x.Date <= to) : expenses;

                return expenses.ToList().Select(x => new View.Expense(x));
            }
        }

        #endregion

        public bool Update(ExpenseBinding binding)
        {
            using (var db = GetMainContext())
            {
                var entity = db.Expenses.WhereUser(User.Id).SingleOrDefault(x => x.ValueId == binding.ValueId);

                entity = binding.ToEntity(db, entity);
                db.SaveChanges();

                return true;
            }
        }
    }
}
