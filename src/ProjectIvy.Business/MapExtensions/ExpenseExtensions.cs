using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Business.MapExtensions;

public static class ExpenseExtensions
{
    public static Expense ToEntity(this ExpenseBinding binding, MainContext context, Expense entity = null)
    {
        if (entity == null)
            entity = new Expense();

        entity.Amount = binding.Amount;
        entity.CardId = context.Cards.GetId(binding.CardId);
        entity.Comment = binding.Comment;
        entity.CurrencyId = context.Currencies.SingleOrDefault(x => x.Code == binding.CurrencyId).Id;
        entity.Date = binding.Date;
        entity.DatePaid = binding.DatePaid ?? entity.Date;
        entity.ExpenseTypeId = context.ExpenseTypes.GetId(binding.ExpenseTypeId).Value;
        entity.Modified = DateTime.Now;
        entity.NeedsReview = binding.NeedsReview;
        entity.ParentAmount = binding.ParentAmount;
        entity.ParentCurrencyExchangeRate = binding.ParentAmount.HasValue ? binding.ParentAmount.Value / binding.Amount : null;
        entity.ParentCurrencyId = string.IsNullOrEmpty(binding.ParentCurrencyId) ? null : context.Currencies.SingleOrDefault(x => x.Code == binding.ParentCurrencyId)?.Id;
        entity.PaymentTypeId = context.PaymentTypes.GetId(binding.PaymentTypeId);
        entity.PoiId = context.Pois.GetId(binding.PoiId);
        entity.ValueId = binding.Id;
        entity.VendorId = context.Vendors.GetId(binding.VendorId);
        entity.InstallmentRef = binding.InstallmentRef;

        return entity;
    }

    public static string LastValueId(this DbSet<Expense> set, int userId)
    {
        return set.WhereUser(userId)
                  .OrderByDescending(x => EF.Functions.DataLength(x.ValueId))
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
