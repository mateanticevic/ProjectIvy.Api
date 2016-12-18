using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using View = AnticevicApi.Model.View.Income;

namespace AnticevicApi.BL.Handlers.Income
{
    public class IncomeHandler : Handler<IncomeHandler>, IIncomeHandler
    {
        public IncomeHandler(IHandlerContext<IncomeHandler> context) : base(context)
        {

        }

        public IEnumerable<View.Income> Get(FilteredPagedBinding binding)
        {
            using (var db = GetMainContext())
            {
                var incomes = db.Incomes.WhereUser(UserId)
                                        .Include(x => x.Currency)
                                        .Include(x => x.IncomeSource)
                                        .OrderByDescending(x => x.Timestamp)
                                        .WhereTimestampInclusive(binding)
                                        .Page(binding);

                return incomes.ToList().Select(x => new View.Income(x));
            }
        }

        public int GetCount(DateTime from, DateTime to)
        {
            using (var db = GetMainContext())
            {
                var query = db.Incomes.WhereUser(UserId)
                                      .Where(x => x.Timestamp >= from && x.Timestamp <= to);

                return query.Count();
            }
        }

        public IEnumerable<View.AmountInCurrency> GetSum(DateTime from, DateTime to)
        {
            using (var db = GetMainContext())
            {
                var query = db.Incomes.WhereUser(UserId)
                                      .Where(x => x.Timestamp >= from && x.Timestamp <= to)
                                      .Include(x => x.Currency)
                                      .ToList()
                                      .GroupBy(x => x.Currency);

                return query.Select(x => new View.AmountInCurrency(x.Sum(y => y.Ammount), x.Key));
            }
        }
    }
}
