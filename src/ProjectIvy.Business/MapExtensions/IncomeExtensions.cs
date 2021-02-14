using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Income;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Business.MapExtensions
{
    public static class IncomeExtensions
    {
        public static Income ToEntity(this IncomeBinding binding, MainContext context, Income entity = null)
        {
            entity = entity ?? new Income();

            entity.Amount = binding.Amount;
            entity.CurrencyId = context.Currencies.GetId(binding.CurrencyId).Value;
            entity.Date = binding.Date;
            entity.Description = binding.Description;
            entity.IncomeSourceId = context.IncomeSources.GetId(binding.SourceId).Value;
            entity.IncomeTypeId = context.IncomeTypes.GetId(binding.TypeId).Value;

            return entity;
        }
    }
}
