using System.Linq;
using ProjectIvy.Model.Binding.Salary;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Data.Extensions.Entities;

public static class SalaryExtensions
{
    public static IQueryable<Salary> Where(this IQueryable<Salary> query, SalaryGetBinding binding)
    {
        return query.WhereIf(binding.From.HasValue, x => x.Date >= binding.From)
                    .WhereIf(binding.To.HasValue, x => x.Date <= binding.To)
                    .WhereIf(!string.IsNullOrWhiteSpace(binding.CurrencyId), x => x.Currency.ValueId == binding.CurrencyId);
    }
}
