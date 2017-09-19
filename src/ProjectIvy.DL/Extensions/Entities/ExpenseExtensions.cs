using ProjectIvy.DL.DbContexts;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Database.Main.Finance;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
{
    public static class ExpenseExtensions
    {
        public static IQueryable<Expense> Where(this IQueryable<Expense> query, ExpenseGetBinding binding, MainContext context)
        {
            int? currencyId = context.Currencies.GetId(binding.CurrencyId);
            int? expenseTypeId = context.ExpenseTypes.GetId(binding.TypeId);
            int? vendorId = context.Vendors.GetId(binding.VendorId);

            return query.WhereIf(binding.From.HasValue, x => x.Date >= binding.From)
                        .WhereIf(binding.To.HasValue, x => x.Date <= binding.To)
                        .WhereIf(expenseTypeId.HasValue, x => x.ExpenseTypeId == expenseTypeId)
                        .WhereIf(vendorId.HasValue, x => x.VendorId == vendorId)
                        .WhereIf(currencyId.HasValue, x => x.CurrencyId == currencyId)
                        .WhereIf(!string.IsNullOrWhiteSpace(binding.Description), x => x.Comment.Contains(binding.Description))
                        .WhereIf(binding.AmountFrom.HasValue, x => x.Ammount >= binding.AmountFrom)
                        .WhereIf(binding.AmountTo.HasValue, x => x.Ammount <= binding.AmountTo);
        }
    }
}
