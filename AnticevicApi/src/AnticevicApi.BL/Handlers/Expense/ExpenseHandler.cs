using AnticevicApi.BL.MapExtensions;
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

        public PaginatedView<View.Expense> Get(DateTime? from, DateTime? to, string expenseTypeValueId, string vendorValueId, int? page = 0, int? pageSize = 10)
        {
            page = page.HasValue ? page : 0;
            pageSize = pageSize.HasValue ? pageSize : 10;

            var view = new PaginatedView<View.Expense>();

            using (var db = GetMainContext())
            {
                int? expenseTypeId = db.ExpenseTypes.GetId(expenseTypeValueId);
                int? vendorId = db.Vendors.GetId(vendorValueId);

                var result = db.Expenses.Include(x => x.ExpenseType)
                                        .Include(x => x.Currency)
                                        .Include(x => x.Vendor)
                                        .WhereUser(User.Id);

                result = from.HasValue ? result.Where(x => x.Date >= from) : result;
                result = to.HasValue ? result.Where(x => x.Date <= to) : result;

                result = expenseTypeId.HasValue ? result.Where(x => x.ExpenseTypeId == expenseTypeId) : result;
                result = vendorId.HasValue ? result.Where(x => x.VendorId == vendorId) : result;

                view.Pages = (int)((result.Count() + pageSize - 1) / pageSize);

                result = result.OrderByDescending(x => x.Date)
                               .ThenByDescending(x => x.Id)
                               .Page(page, pageSize);

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

        public IEnumerable<KeyValuePair<DateTime, decimal>> GetGroupedSum(string typeValueId, TimeGroupingTypes timeGroupingTypes)
        {
            using (var db = GetMainContext())
            {
                var q = db.Expenses.WhereUser(User.Id)
                                   .Where(x => x.ExpenseType.ValueId == typeValueId);

                if (timeGroupingTypes == TimeGroupingTypes.Year)
                {
                    return q.GroupBy(x => x.Date.Year)
                            .Select(x => new KeyValuePair<DateTime, decimal>(new DateTime(x.Key, 1, 1), x.Sum(y => y.Ammount)))
                            .ToList();
                }
                else
                {
                    return q.GroupBy(x => new { x.Date.Year, x.Date.Month })
                            .Select(x => new KeyValuePair<DateTime, decimal>(new DateTime(x.Key.Year, x.Key.Month, 1), x.Sum(y => y.Ammount)))
                            .ToList();
                }
            }
        }

        public decimal GetSum(FilteredBinding binding, string currencyCode)
        {
            using (var db = GetMainContext())
            {
                int targetCurrencyId = string.IsNullOrEmpty(currencyCode) ? db.Users.SingleOrDefault(x => x.Id == User.Id).DefaultCurrencyId
                                                                          : db.Currencies.SingleOrDefault(x => x.Code == currencyCode).Id;

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
