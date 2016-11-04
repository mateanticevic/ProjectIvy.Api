using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Common;
using AnticevicApi.Model.View.Income;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AnticevicApi.BL.Handlers
{
    public class IncomeHandler : Handler
    {
        public IncomeHandler(int userId)
        {
            UserId = userId;
        }

        public IEnumerable<Income> Get(FilteredPagedBinding binding)
        {
            using (var db = new MainContext())
            {
                var incomes = db.Incomes.WhereUser(UserId)
                                        .Include(x => x.Currency)
                                        .Include(x => x.IncomeSource)
                                        .OrderByDescending(x => x.Timestamp)
                                        .WhereTimestampInclusive(binding)
                                        .Page(binding);

                return incomes.ToList().Select(x => new Income(x));
            }
        }

        public int GetCount(DateTime from, DateTime to)
        {
            using (var db = new MainContext())
            {
                var query = db.Incomes.WhereUser(UserId)
                                      .Where(x => x.Timestamp >= from && x.Timestamp <= to);

                return query.Count();
            }
        }

        public IEnumerable<AmountInCurrency> GetSum(DateTime from, DateTime to)
        {
            using (var db = new MainContext())
            {
                var query = db.Incomes.WhereUser(UserId)
                                      .Where(x => x.Timestamp >= from && x.Timestamp <= to)
                                      .Include(x => x.Currency)
                                      .ToList()
                                      .GroupBy(x => x.Currency);

                return query.Select(x => new AmountInCurrency(x.Sum(y => y.Ammount), x.Key));
            }
        }
    }
}
