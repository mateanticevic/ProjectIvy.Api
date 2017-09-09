using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Common;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Model.View;
using ProjectIvy.DL.Extensions.Entities;
using ProjectIvy.DL.Sql;
using Dapper;
using System.Linq;
using System;
using View = ProjectIvy.Model.View.Income;

namespace ProjectIvy.BL.Handlers.Income
{
    public class IncomeHandler : Handler<IncomeHandler>, IIncomeHandler
    {
        public IncomeHandler(IHandlerContext<IncomeHandler> context) : base(context)
        {
        }

        public PagedView<View.Income> Get(FilteredPagedBinding binding)
        {
            using (var db = GetMainContext())
            {
                var query = db.Incomes.WhereUser(User.Id)
                                      .Include(x => x.Currency)
                                      .Include(x => x.IncomeSource)
                                      .Include(x => x.IncomeType)
                                      .OrderByDescending(x => x.Timestamp)
                                      .WhereTimestampInclusive(binding);

                var items = query.Page(binding)
                                 .ToList()
                                 .Select(x => new View.Income(x));

                var count = query.Count();

                return new PagedView<View.Income>(items, count);
            }
        }

        public int GetCount(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                var query = db.Incomes.WhereUser(User.Id)
                                      .Where(x => x.Timestamp >= binding.From && x.Timestamp <= binding.To);

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
