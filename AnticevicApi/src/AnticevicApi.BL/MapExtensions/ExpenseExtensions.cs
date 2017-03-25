using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Expense;
using AnticevicApi.Model.Database.Main.Finance;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace AnticevicApi.BL.MapExtensions
{
    public static class ExpenseExtensions
    {
        public static Expense ToEntity(this ExpenseBinding binding, MainContext db, Expense entity = null)
        {
            if (entity == null)
            {
                entity = new Expense();
            }

            entity.Ammount = binding.Amount;
            entity.Comment = binding.Comment;
            entity.CurrencyId = db.Currencies.SingleOrDefault(x => x.Code == binding.CurrencyValueId).Id;
            entity.Date = binding.Date;
            entity.ExpenseTypeId = db.ExpenseTypes.GetId(binding.ExpenseTypeValueid).Value;
            entity.Modified = DateTime.Now;
            entity.VendorId = db.Vendors.GetId(binding.VendorValueId);

            return entity;
        }

        public static string LastValueId(this DbSet<Expense> set, int userId)
        {
            return set.WhereUser(userId)
                      .OrderByDescending(x => x.ValueId.Count())
                      .ThenByDescending(x => x.ValueId)
                      .FirstOrDefault()
                      .ValueId;
        }

        public static int NextValueId(this DbSet<Expense> set, int userId)
        {
            return Convert.ToInt32(set.LastValueId(userId)) + 1;
        }
    }
}
