using Dapper;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Databases.Main.Queries;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Data.Sql;
using ProjectIvy.Data.Sql.Main.Scripts;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Income;
using ProjectIvy.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.Income;

namespace ProjectIvy.Business.Handlers.Income
{
    public class IncomeHandler : Handler<IncomeHandler>, IIncomeHandler
    {
        public IncomeHandler(IHandlerContext<IncomeHandler> context) : base(context)
        {
        }

        public async Task Add(IncomeBinding binding)
        {
            using (var context = GetMainContext())
            {
                var entity = binding.ToEntity(context);
                entity.UserId = UserId.Value;

                await context.Incomes.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public PagedView<View.Income> Get(IncomeGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Incomes.WhereUser(UserId.Value)
                                      .Include(x => x.Currency)
                                      .Include(x => x.IncomeSource)
                                      .Include(x => x.IncomeType)
                                      .Where(binding, context)
                                      .OrderBy(binding)
                                      .Select(x => new View.Income(x))
                                      .ToPagedView(binding);
            }
        }

        public int GetCount(FilteredBinding binding)
        {
            using (var db = GetMainContext())
            {
                var query = db.Incomes.WhereUser(UserId.Value)
                                      .Where(x => x.Date >= binding.From && x.Date <= binding.To);

                return query.Count();
            }
        }

        public async Task<IEnumerable<View.IncomeSource>> GetSources()
        {
            using (var context = GetMainContext())
            {
                return await context.IncomeSources.WhereUser(UserId.Value)
                                                  .OrderBy(x => x.Name)
                                                  .Select(x => new View.IncomeSource(x))
                                                  .ToListAsync();
            }
        }

        public async Task<decimal> GetSum(IncomeGetSumBinding binding)
        {
            using (var context = GetMainContext())
            {
                int targetCurrencyId = context.GetCurrencyId(binding.TargetCurrencyId, UserId.Value);

                var incomeIds = context.Incomes.WhereUser(UserId.Value)
                                               .Where(binding, context)
                                               .Select(x => x.Id)
                                               .ToList();

                if (incomeIds.Count == 0)
                    return 0;

                using (var sql = GetSqlConnection())
                {
                    var parameters = new GetIncomeSumQuery()
                    {
                        IncomeIds = incomeIds,
                        TargetCurrencyId = targetCurrencyId,
                        UserId = UserId.Value
                    };

                    return Math.Round(await sql.ExecuteScalarAsync<decimal>(SqlLoader.Load(Constants.GetIncomeSum), parameters), 2);
                }
            }
        }

        public IEnumerable<GroupedByMonth<decimal>> GetSumByMonth(IncomeGetSumBinding binding)
        {
            using (var context = GetMainContext())
            {
                var from = binding.From ?? context.Incomes.WhereUser(UserId.Value).OrderBy(x => x.Date).FirstOrDefault().Date;
                var to = binding.To ?? DateTime.Now;

                var periods = from.RangeMonthsClosed(to)
                                  .Select(x => new FilteredBinding(x.from, x.to))
                                  .ToList();

                var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<decimal>>(x, GetSum(binding.OverrideFromTo<IncomeGetSumBinding>(x.From, x.To))));

                return tasks.Select(x => new GroupedByMonth<decimal>(x.Value.Result, x.Key.From.Value.Year, x.Key.From.Value.Month));
            }
        }

        public IEnumerable<KeyValuePair<int, decimal>> GetSumByYear(IncomeGetSumBinding binding)
        {
            using (var context = GetMainContext())
            {
                int startYear = binding.From?.Year ?? context.Incomes.WhereUser(UserId.Value).OrderBy(x => x.Date).FirstOrDefault().Date.Year;
                int endYear = binding.To?.Year ?? DateTime.Now.Year;

                var years = Enumerable.Range(startYear, endYear - startYear + 1);

                var periods = years.Select(x => new FilteredBinding(new DateTime(x, 1, 1), new DateTime(x, 12, 31)));

                var tasks = periods.Select(x => new KeyValuePair<int, Task<decimal>>(x.From.Value.Year, GetSum(binding.OverrideFromTo<IncomeGetSumBinding>(x.From, x.To))));

                return tasks.Select(x => new KeyValuePair<int, decimal>(x.Key, x.Value.Result));
            }
        }

        public async Task<IEnumerable<View.IncomeType>> GetTypes()
        {
            using (var context = GetMainContext())
            {
                return await context.IncomeTypes.OrderBy(x => x.Name)
                                                .Select(x => new View.IncomeType(x))
                                                .ToListAsync();
            }
        }
    }
}
