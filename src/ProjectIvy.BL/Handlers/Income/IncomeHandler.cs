using Dapper;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.DL.Extensions.Entities;
using ProjectIvy.DL.Extensions;
using ProjectIvy.DL.Sql;
using ProjectIvy.Model.Binding.Common;
using ProjectIvy.Model.Binding.Income;
using ProjectIvy.Model.View;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Common.Extensions;
using View = ProjectIvy.Model.View.Income;

namespace ProjectIvy.BL.Handlers.Income
{
    public class IncomeHandler : Handler<IncomeHandler>, IIncomeHandler
    {
        public IncomeHandler(IHandlerContext<IncomeHandler> context) : base(context)
        {
        }

        public PagedView<View.Income> Get(IncomeGetBinding binding)
        {
            using (var db = GetMainContext())
            {
                var query = db.Incomes.WhereUser(User.Id)
                                      .Include(x => x.Currency)
                                      .Include(x => x.IncomeSource)
                                      .Include(x => x.IncomeType)
                                      .OrderBy(binding)
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

        public async Task<decimal> GetSum(IncomeGetSumBinding binding)
        {
            using (var db = GetMainContext())
            {
                int targetCurrencyId = db.GetCurrencyId(binding.TargetCurrencyId, User.Id);

                using (var sql = GetSqlConnection())
                {
                    var parameters = new
                    {
                        TargetCurrencyId = targetCurrencyId,
                        From = binding.From,
                        To = binding.To,
                        UserId = User.Id
                    };

                    return Math.Round(await sql.ExecuteScalarAsync<decimal>(SqlLoader.Load(MainSnippets.GetIncomeSum), parameters), 2);
                }
            }
        }

        public IEnumerable<GroupedByMonth<decimal>> GetSumByMonth(IncomeGetSumBinding binding)
        {
            using (var context = GetMainContext())
            {
                var from = binding.From ?? context.Incomes.WhereUser(User.Id).OrderBy(x => x.Timestamp).FirstOrDefault().Timestamp;
                var to = binding.To ?? DateTime.Now;

                var periods = from.RangeMonthsClosed(to)
                                  .Select(x => new FilteredBinding(x.from, x.to))
                                  .ToList();

                var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<decimal>>(x, GetSum(binding.OverrideFromTo<IncomeGetSumBinding>(x.From, x.To))));

                return tasks.Select(x => new GroupedByMonth<decimal>(x.Value.Result, x.Key.From.Value.Year, x.Key.From.Value.Month));
            }
        }
    }
}
