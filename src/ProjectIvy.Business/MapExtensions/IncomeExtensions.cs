using System.Threading.Tasks;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Income;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Business.MapExtensions;

public static class IncomeExtensions
{
    public static async Task<Income> ToEntity(this IncomeBinding binding, MainContext context, Income entity = null)
    {
        entity = entity ?? new Income();

        entity.Amount = binding.Amount;
        entity.CurrencyId = (await context.Currencies.GetIdAsync(binding.CurrencyId)).Value;
        entity.Date = binding.Date;
        entity.Description = binding.Description;
        entity.IncomeSourceId = (await context.IncomeSources.GetIdAsync(binding.SourceId)).Value;
        entity.IncomeTypeId = (await context.IncomeTypes.GetIdAsync(binding.TypeId)).Value;

        return entity;
    }
}
