using System.Linq;
using System.Threading.Tasks;
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
using View = ProjectIvy.Model.View.Income;

namespace ProjectIvy.Business.Handlers.Income;

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
            entity.UserId = UserId;

            await context.Incomes.AddAsync(entity);
            await context.SaveChangesAsync();
        }
    }

    public async Task<PagedView<View.Income>> Get(IncomeGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Incomes.WhereUser(UserId)
                                    .Include(x => x.Currency)
                                    .Include(x => x.IncomeSource)
                                    .Include(x => x.IncomeType)
                                    .Where(binding, context)
                                    .OrderBy(binding)
                                    .Select(x => new View.Income(x))
                                    .ToPagedViewAsync(binding);
    }

    public async Task<int> GetCount(FilteredBinding binding)
    {
        using var db = GetMainContext();
        return await db.Incomes.WhereUser(UserId)
                               .Where(x => x.Date >= binding.From && x.Date <= binding.To)
                               .CountAsync();
    }

    public async Task<IEnumerable<View.IncomeSource>> GetSources()
    {
        using (var context = GetMainContext())
        {
            return await context.IncomeSources.WhereUser(UserId)
                                              .OrderBy(x => x.Name)
                                              .Select(x => new View.IncomeSource(x))
                                              .ToListAsync();
        }
    }

    public async Task<decimal> GetSum(IncomeGetSumBinding binding)
    {
        using (var context = GetMainContext())
        {
            int targetCurrencyId = context.GetCurrencyId(binding.TargetCurrencyId, UserId);

            var incomeIds = context.Incomes.WhereUser(UserId)
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
                    UserId = UserId
                };

                return Math.Round(await sql.ExecuteScalarAsync<decimal>(SqlLoader.Load(SqlScripts.GetIncomeSum), parameters), 2);
            }
        }
    }

    public IEnumerable<KeyValuePair<DateTime, decimal>> GetSumByMonthOfYear(IncomeGetSumBinding binding)
    {
        using (var context = GetMainContext())
        {
            var from = binding.From ?? context.Incomes.WhereUser(UserId).OrderBy(x => x.Date).FirstOrDefault().Date;
            var to = binding.To ?? DateTime.Now;

            var periods = from.RangeMonthsClosed(to)
                              .Select(x => new FilteredBinding(x.from, x.to))
                              .ToList();

            var tasks = periods.Select(x => new KeyValuePair<FilteredBinding, Task<decimal>>(x, GetSum(binding.OverrideFromTo<IncomeGetSumBinding>(x.From, x.To))));

            return tasks.Select(x => new KeyValuePair<DateTime, decimal>(new DateTime(x.Key.From.Value.Year, x.Key.From.Value.Month, 1), x.Value.Result));
        }
    }

    public IEnumerable<KeyValuePair<int, decimal>> GetSumByYear(IncomeGetSumBinding binding)
    {
        using (var context = GetMainContext())
        {
            int startYear = binding.From?.Year ?? context.Incomes.WhereUser(UserId).OrderBy(x => x.Date).FirstOrDefault().Date.Year;
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
