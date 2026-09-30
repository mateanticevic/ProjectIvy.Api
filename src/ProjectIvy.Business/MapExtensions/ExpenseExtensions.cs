using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Expense;
using ProjectIvy.Model.Database.Main;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Business.MapExtensions;

public static class ExpenseExtensions
{
    public static string LastValueId<T>(this DbSet<T> set, int userId) where T : UserEntity, IHasValueId
    {
        return set.WhereUser(userId)
                  .OrderByDescending(x => EF.Functions.DataLength(x.ValueId))
                  .ThenByDescending(x => x.ValueId)
                  .FirstOrDefault()
                  ?.ValueId;
    }

    public static int NextValueId<T>(this DbSet<T> set, int userId) where T : UserEntity, IHasValueId
    {
        string lastValueId = set.LastValueId(userId);

        lastValueId = string.IsNullOrEmpty(lastValueId) ? 0.ToString() : lastValueId;

        return Convert.ToInt32(lastValueId) + 1;
    }

    public static async Task<string> LastValueIdAsync<T>(this DbSet<T> set, int userId) where T : UserEntity, IHasValueId
    {
        return (await set.WhereUser(userId)
                         .OrderByDescending(x => EF.Functions.DataLength(x.ValueId))
                         .ThenByDescending(x => x.ValueId)
                         .FirstOrDefaultAsync())
                         ?.ValueId;
    }

    public static async Task<int> NextValueIdAsync<T>(this DbSet<T> set, int userId) where T : UserEntity, IHasValueId
    {
        string lastValueId = await set.LastValueIdAsync(userId);

        lastValueId = string.IsNullOrEmpty(lastValueId) ? 0.ToString() : lastValueId;

        return Convert.ToInt32(lastValueId) + 1;
    }

    public static async Task<Expense> ToEntity(this ExpenseBinding binding, MainContext context, Expense entity = null)
    {
        if (entity == null)
            entity = new Expense();

        entity.Amount = binding.Amount;
        entity.CardId = await context.Cards.GetIdAsync(binding.CardId);
        entity.Comment = binding.Comment;
        entity.CurrencyId = (await context.Currencies.SingleOrDefaultAsync(x => x.Code == binding.CurrencyId)).Id;
        entity.Date = binding.Date;
        entity.DatePaid = binding.DatePaid ?? entity.Date;
        entity.ExpenseTypeId = (await context.ExpenseTypes.GetIdAsync(binding.ExpenseTypeId)).Value;
        entity.ExternalId = string.IsNullOrEmpty(binding.ExternalId) ? entity.ExternalId : binding.ExternalId;
        entity.Modified = DateTime.Now;
        entity.NeedsReview = binding.NeedsReview;
        entity.ParentAmount = binding.ParentAmount;
        entity.ParentCurrencyExchangeRate = binding.ParentAmount.HasValue ? binding.ParentAmount.Value / binding.Amount : null;
        entity.ParentCurrencyId = string.IsNullOrEmpty(binding.ParentCurrencyId) ? null : (await context.Currencies.SingleOrDefaultAsync(x => x.Code == binding.ParentCurrencyId))?.Id;
        entity.PaymentTypeId = await context.PaymentTypes.GetIdAsync(binding.PaymentTypeId);
        entity.PoiId = await context.Pois.GetIdAsync(binding.PoiId);
        entity.ValueId = binding.Id;
        entity.VendorId = await context.Vendors.GetIdAsync(binding.VendorId);
        entity.InstallmentRef = binding.InstallmentRef;

        return entity;
    }
}
