using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Database.Main.Finance;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace AnticevicApi.BL.MapExtensions
{
    public static class ExpenseExtensions
    {
        public static Expense ToEntity(this ExpenseBinding binding, MainContext context, Expense entity = null)
        {
            if (entity == null)
            {
                entity = new Expense();
            }

            entity.Ammount = binding.Amount;
            entity.CardId = context.Cards.GetId(binding.CardId);
            entity.Comment = binding.Comment;
            entity.CurrencyId = context.Currencies.SingleOrDefault(x => x.Code == binding.CurrencyId).Id;
            entity.Date = binding.Date;
            entity.ExpenseTypeId = context.ExpenseTypes.GetId(binding.ExpenseTypeId).Value;
            entity.Modified = DateTime.Now;
            entity.PaymentTypeId = context.PaymentTypes.GetId(binding.PaymentTypeId);
            entity.PoiId = context.Pois.GetId(binding.PoiId);
            entity.VendorId = context.Vendors.GetId(binding.VendorId);

            return entity;
        }

        public static string LastValueId(this DbSet<Expense> set, int userId)
        {
            return set.WhereUser(userId)
                      .OrderByDescending(x => x.ValueId.Count())
                      .ThenByDescending(x => x.ValueId)
                      .FirstOrDefault()?
                      .ValueId;
        }

        public static int NextValueId(this DbSet<Expense> set, int userId)
        {
            string lastValueId = set.LastValueId(userId);

            lastValueId = string.IsNullOrEmpty(lastValueId) ? 0.ToString() : lastValueId;

            return Convert.ToInt32(lastValueId) + 1;
        }
    }
}
