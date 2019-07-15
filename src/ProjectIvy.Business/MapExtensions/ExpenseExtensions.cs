using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Database.Main.Finance;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace ProjectIvy.Business.MapExtensions
{
    public static class ExpenseExtensions
    {
        public static Expense ToEntity(this ExpenseBinding binding, MainContext context, Expense entity = null)
        {
            if (entity == null)
            {
                entity = new Expense();
            }

            entity.Amount = binding.Amount;
            entity.CardId = context.Cards.GetId(binding.CardId);
            entity.Comment = binding.Comment;
            entity.CurrencyId = context.Currencies.SingleOrDefault(x => x.Code == binding.CurrencyId).Id;
            entity.Date = binding.Date;
            entity.ExpenseTypeId = context.ExpenseTypes.GetId(binding.ExpenseTypeId).Value;
            entity.Modified = DateTime.Now;
            entity.NeedsReview = false;
            entity.ParentCurrencyId = string.IsNullOrEmpty(binding.ParentCurrencyId) ? null : context.Currencies.SingleOrDefault(x => x.Code == binding.ParentCurrencyId)?.Id;
            entity.PaymentTypeId = context.PaymentTypes.GetId(binding.PaymentTypeId);
            entity.ParentCurrencyExchangeRate = binding.ParentCurrencyExchangeRate;
            entity.PoiId = context.Pois.GetId(binding.PoiId);
            entity.ValueId = binding.Id;
            entity.VendorId = context.Vendors.GetId(binding.VendorId);

            return entity;
        }

        public static string LastValueId(this DbSet<Expense> set, int userId)
        {
            return set.WhereUser(userId)
                      .OrderByDescending(x => x.ValueId.Count())
                      .ThenByDescending(x => x.ValueId)
                      .FirstOrDefault()
                      ?.ValueId;
        }

        public static int NextValueId(this DbSet<Expense> set, int userId)
        {
            string lastValueId = set.LastValueId(userId);

            lastValueId = string.IsNullOrEmpty(lastValueId) ? 0.ToString() : lastValueId;

            return Convert.ToInt32(lastValueId) + 1;
        }
    }
}
