using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using View = AnticevicApi.Model.View.Expense;

namespace AnticevicApi.BL.Handlers.Expense
{
    public class ExpenseHandler : Handler, IExpenseHandler
    {
        public string Create(ExpenseBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var entity = binding.ToEntity(db);
                entity.UserId = UserId;

                db.Expenses.Add(entity);
                db.SaveChanges();

                return entity.ValueId;
            }
        }

        public bool Delete(string valueId)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var entity = db.Expenses.WhereUser(UserId)
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

            using (var db = new MainContext(ConnectionString))
            {
                int? expenseTypeId = db.ExpenseTypes.GetId(expenseTypeValueId);
                int? vendorId = db.Vendors.GetId(vendorValueId);

                var result = db.Expenses.Include(x => x.ExpenseType)
                                        .Include(x => x.Currency)
                                        .Include(x => x.Vendor)
                                        .WhereUser(UserId);

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
            using (var db = new MainContext(ConnectionString))
            {
                return db.Expenses.WhereUser(UserId)
                                  .Where(x => x.Date.Date == date)
                                  .ToList()
                                  .Select(x => new View.Expense(x));
            }
        }

        public int GetCount(DateTime from, DateTime to)
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.Expenses.WhereUser(UserId)
                                  .Where(x => x.Date.Date >= from && x.Date.Date <= to)
                                  .Count();
            }
        }

        public IEnumerable<KeyValuePair<DateTime, decimal>> GetGroupedSum(string typeValueId, TimeGroupingTypes timeGroupingTypes)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var q = db.Expenses.WhereUser(UserId)
                                   .Where(x => x.ExpenseType.ValueId == typeValueId);

                if(timeGroupingTypes == TimeGroupingTypes.Year)
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

        public decimal GetSum(FilteredBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.Expenses.WhereUser(UserId)
                                  .Where(x => x.Date.Date >= binding.From && x.Date.Date <= binding.To)
                                  .Sum(x => x.Ammount);
            }
        }

        public IEnumerable<View.Expense> GetByVendor(string valueId, DateTime? from = null, DateTime? to = null)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var c = db.Vendors.SingleOrDefault(x => x.ValueId == valueId);

                var expenses = db.Vendors.Include(x => x.Expenses)
                                 .ThenInclude(x => x.Currency)
                                 .SingleOrDefault(x => x.ValueId == valueId)
                                 .Expenses
                                 .WhereUser(UserId);

                expenses = from.HasValue ? expenses.Where(x => x.Date >= from) : expenses;
                expenses = to.HasValue ? expenses.Where(x => x.Date <= to) : expenses;

                return expenses.ToList().Select(x => new View.Expense(x));
            }
        }

        public IEnumerable<View.Expense> GetByType(string valueId, DateTime? from = null, DateTime? to = null)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var c = db.Vendors.SingleOrDefault(x => x.ValueId == valueId);

                var expenses = db.ExpenseTypes.Include(x => x.Expenses)
                                 .ThenInclude(x => x.Currency)
                                 .SingleOrDefault(x => x.ValueId == valueId)
                                 .Expenses
                                 .WhereUser(UserId);

                expenses = from.HasValue ? expenses.Where(x => x.Date >= from) : expenses;
                expenses = to.HasValue ? expenses.Where(x => x.Date <= to) : expenses;

                return expenses.ToList().Select(x => new View.Expense(x));
            }
        }

        #endregion

        public bool Update(ExpenseBinding binding)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var entity = db.Expenses.WhereUser(UserId).SingleOrDefault(x => x.ValueId == binding.ValueId);

                entity = binding.ToEntity(db, entity);
                db.SaveChanges();

                return true;
            }
        }
    }
}
