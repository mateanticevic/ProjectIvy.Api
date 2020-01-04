using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Database.Main.Finance;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class ExpenseExtensions
    {
        public static IQueryable<Expense> IncludeAll(this IQueryable<Expense> query)
        {
            return query.Include(x => x.ExpenseType)
                        .Include(x => x.Currency)
                        .Include(x => x.ParentCurrency)
                        .Include(x => x.Poi)
                        .Include(x => x.Vendor)
                        .Include(x => x.PaymentType)
                        .Include(x => x.Card)
                        .Include(x => x.ExpenseFiles)
                        .ThenInclude(x => x.ExpenseFileType)
                        .Include(x => x.ExpenseFiles)
                        .ThenInclude(x => x.File)
                        .ThenInclude(x => x.FileType);
        }

        public static IQueryable<Expense> Where(this IQueryable<Expense> query, ExpenseGetBinding b, MainContext context)
        {
            var cardIds = context.Cards.GetIds(b.CardId);
            var currencyIds = context.Currencies.GetIds(b.CurrencyId);
            var expenseTypeIds = context.ExpenseTypes.GetIds(b.TypeId);
            var paymentTypeIds = context.PaymentTypes.GetIds(b.PaymentTypeId);
            var vendorIds = context.Vendors.GetIds(b.VendorId);

            IEnumerable<ExpenseType> expenseTypes = null;
            if (expenseTypeIds != null && expenseTypeIds.Any())
                expenseTypes = context.ExpenseTypes.GetAll();

            return query.WhereIf(b.From.HasValue, x => x.Date >= b.From)
                        .WhereIf(b.To.HasValue, x => x.Date <= b.To)
                        .WhereIf(cardIds, x => x.CardId.HasValue && cardIds.Contains(x.CardId.Value))
                        .WhereIf(paymentTypeIds, x => x.PaymentTypeId.HasValue && paymentTypeIds.Contains(x.PaymentTypeId.Value))
                        .WhereIf(expenseTypeIds, x => expenseTypeIds.Contains(x.ExpenseTypeId) || expenseTypes.SingleOrDefault(y => y.Id == x.ExpenseTypeId).IsChildType(expenseTypeIds))
                        .WhereIf(vendorIds, x => x.VendorId.HasValue && vendorIds.Contains(x.VendorId.Value))
                        .WhereIf(currencyIds, x => currencyIds.Contains(x.CurrencyId))
                        .WhereIf(b.Day != null, x => b.Day.Contains(x.Date.DayOfWeek))
                        .WhereIf(b.HasLinkedFiles.HasValue, x => !(b.HasLinkedFiles.Value ^ x.ExpenseFiles.Any()))
                        .WhereIf(b.HasPoi.HasValue, x => !(b.HasPoi.Value ^ x.PoiId.HasValue))
                        .WhereIf(b.NeedsReview.HasValue, x => !(b.NeedsReview.Value ^ x.NeedsReview))
                        .WhereIf(!string.IsNullOrWhiteSpace(b.Description), x => x.Comment.Contains(b.Description))
                        .WhereIf(b.AmountFrom.HasValue, x => x.Amount >= b.AmountFrom)
                        .WhereIf(b.ExcludeId != null, x => !b.ExcludeId.Contains(x.ValueId))
                        .WhereIf(b.AmountTo.HasValue, x => x.Amount <= b.AmountTo);
        }

        public static IOrderedQueryable<Expense> OrderBy(this IQueryable<Expense> query, ExpenseGetBinding binding)
        {
            switch (binding.OrderBy)
            {
                case ExpenseSort.Date:
                    return query.OrderBy(binding.OrderAscending, x => x.Date);
                case ExpenseSort.Created:
                    return query.OrderBy(binding.OrderAscending, x => x.Created);
                case ExpenseSort.Modified:
                    return query.OrderBy(binding.OrderAscending, x => x.Modified);
                case ExpenseSort.Amount:
                    return query.OrderBy(binding.OrderAscending, x => x.Amount);
                default:
                    return query.OrderBy(binding.OrderAscending, x => x.Date);
            }
        }
    }
}
