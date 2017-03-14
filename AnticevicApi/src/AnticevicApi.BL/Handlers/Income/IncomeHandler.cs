using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using View = AnticevicApi.Model.View.Income;
using AnticevicApi.DL.Sql;
using Dapper;
using AnticevicApi.DL.Extensions.Entities;

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
                var incomes = db.Incomes.WhereUser(User.Id)
                                        .Include(x => x.Currency)
                                        .Include(x => x.IncomeSource)
                                        .Include(x => x.IncomeType)
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
                var query = db.Incomes.WhereUser(User.Id)
                                      .Where(x => x.Timestamp >= from && x.Timestamp <= to);

                return query.Count();
            }
        }

        public decimal GetSum(FilteredBinding binding, string currencyCode)
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
                    return Math.Round(sql.ExecuteScalar<decimal>(SqlLoader.Load(MainSnippets.GetIncomeSum), parameters), 2);
                }
            }
        }
    }
}
