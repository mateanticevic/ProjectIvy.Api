﻿using Microsoft.EntityFrameworkCore;
using ProjectIvy.DL.DbContexts;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Database.Main.Finance;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
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
                        .Include($"{nameof(Expense.ExpenseFiles)}.{nameof(ExpenseFile.File)}.{nameof(Model.Database.Main.Storage.File.FileType)}")
                        .Include($"{nameof(Expense.ExpenseFiles)}.{nameof(ExpenseFile.ExpenseFileType)}");
        }

        public static IQueryable<Expense> Where(this IQueryable<Expense> query, ExpenseGetBinding binding, MainContext context)
        {
            int? cardId = context.Cards.GetId(binding.CardId);
            int? currencyId = context.Currencies.GetId(binding.CurrencyId);
            var expenseTypeIds = context.ExpenseTypes.GetIds(binding.TypeId);
            int? paymentTypeId = context.PaymentTypes.GetId(binding.PaymentTypeId);
            var vendorIds = context.Vendors.GetIds(binding.VendorId);

            IEnumerable<ExpenseType> expenseTypes = null;
            if (expenseTypeIds.Any())
                expenseTypes = context.ExpenseTypes.GetAll();

            return query.WhereIf(binding.From.HasValue, x => x.Date >= binding.From)
                        .WhereIf(binding.To.HasValue, x => x.Date <= binding.To)
                        .WhereIf(cardId.HasValue, x => x.CardId == cardId)
                        .WhereIf(paymentTypeId.HasValue, x => x.PaymentTypeId == paymentTypeId)
                        .WhereIf(expenseTypeIds, x => expenseTypeIds.Contains(x.ExpenseTypeId) || expenseTypes.SingleOrDefault(y => y.Id == x.ExpenseTypeId).IsChildType(expenseTypeIds))
                        .WhereIf(vendorIds, x => x.VendorId.HasValue && vendorIds.Contains(x.VendorId.Value))
                        .WhereIf(currencyId.HasValue, x => x.CurrencyId == currencyId)
                        .WhereIf(binding.Day != null, x => binding.Day.Contains(x.Date.DayOfWeek))
                        .WhereIf(binding.HasLinkedFiles.HasValue, x => !(binding.HasLinkedFiles.Value ^ x.ExpenseFiles.Any()))
                        .WhereIf(binding.HasPoi.HasValue, x => !(binding.HasPoi.Value ^ x.PoiId.HasValue))
                        .WhereIf(!string.IsNullOrWhiteSpace(binding.Description), x => x.Comment.Contains(binding.Description))
                        .WhereIf(binding.AmountFrom.HasValue, x => x.Ammount >= binding.AmountFrom)
                        .WhereIf(binding.AmountTo.HasValue, x => x.Ammount <= binding.AmountTo);
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
                    return query.OrderBy(binding.OrderAscending, x => x.Ammount);
                default:
                    return query.OrderBy(binding.OrderAscending, x => x.Date);
            }
        }
    }
}
