﻿using ProjectIvy.DL.DbContexts;
using ProjectIvy.Model.Binding.Income;
using ProjectIvy.Model.Database.Main.Finance;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
{
    public static class IncomeExtensions
    {
        public static IOrderedQueryable<Income> OrderBy(this IQueryable<Income> query, IncomeGetBinding binding)
        {
            switch (binding.OrderBy)
            {
                case IncomeSort.Date:
                    return query.OrderBy(binding.OrderAscending, x => x.Timestamp);
                case IncomeSort.Amount:
                    return query.OrderBy(binding.OrderAscending, x => x.Ammount);
                default:
                    return query.OrderBy(binding.OrderAscending, x => x.Timestamp);
            }
        }

        public static IQueryable<Income> Where(this IQueryable<Income> query, IncomeGetBinding binding, MainContext context)
        {
            int? currencyId = context.Currencies.GetId(binding.CurrencyId);
            int? sourceId = context.IncomeSources.GetId(binding.SourceId);
            int? typeId = context.IncomeTypes.GetId(binding.TypeId);

            return query.WhereIf(binding.From.HasValue, x => x.Timestamp >= binding.From)
                        .WhereIf(binding.To.HasValue, x => x.Timestamp <= binding.To)
                        .WhereIf(typeId.HasValue, x => x.IncomeTypeId == typeId)
                        .WhereIf(sourceId.HasValue, x => x.IncomeSourceId == sourceId)
                        .WhereIf(currencyId.HasValue, x => x.CurrencyId == currencyId);
        }
    }
}
