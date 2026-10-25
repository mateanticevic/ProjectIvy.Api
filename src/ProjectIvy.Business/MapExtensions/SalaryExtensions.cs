using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Salary;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Business.MapExtensions;

public static class SalaryExtensions
{
    public static string GetNextValueId(DateTime date, IEnumerable<string> valueIds)
    {
        string prefix = date.ToString("yyyy-MM-dd-", CultureInfo.InvariantCulture);
        int number = valueIds.Where(x => x != null && x.StartsWith(prefix, StringComparison.Ordinal))
                            .Select(x => int.TryParse(x.AsSpan(prefix.Length), NumberStyles.None, CultureInfo.InvariantCulture, out int value) ? value : 0)
                            .DefaultIfEmpty(0)
                            .Max() + 1;

        return prefix + number.ToString(CultureInfo.InvariantCulture);
    }

    public static async Task<Salary> ToEntity(this SalaryBinding binding, MainContext context, Salary entity = null)
    {
        int companyId = await context.Companies.GetIdAsync(binding.CompanyId)
                        ?? throw new InvalidRequestException("Company not found.");
        int currencyId = await context.Currencies.GetIdAsync(binding.CurrencyId)
                         ?? throw new InvalidRequestException("Currency not found.");

        entity ??= new Salary();
        entity.CompanyId = companyId;
        entity.CurrencyId = currencyId;
        entity.Date = binding.Date;
        entity.Deductions = binding.Deductions;
        entity.Gross = binding.Gross;
        entity.GrossTotal = binding.GrossTotal;
        entity.Medical = binding.Medical;
        entity.Net = binding.Net;
        entity.NetTotal = binding.NetTotal;
        entity.Note = binding.Note;
        entity.Tax = binding.Tax;
        entity.TravelExpenses = binding.TravelExpenses;

        return entity;
    }
}
