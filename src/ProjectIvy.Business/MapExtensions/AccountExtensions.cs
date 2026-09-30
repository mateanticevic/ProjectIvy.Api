using System.Threading.Tasks;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Account;
using Entity = ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Business.MapExtensions;

public static class AccountExtensions
{
    public static async Task<Entity.Account> ToEntity(this AccountBinding b, MainContext context, Entity.Account a = null)
    {
        a = a.DefaultIfNull();
        a.Name = b.Name;
        a.Iban = b.Iban;
        a.BankId = string.IsNullOrWhiteSpace(b.BankId) ? null : await context.Banks.GetIdAsync(b.BankId);
        a.CurrencyId = (await context.Currencies.GetIdAsync(b.CurrencyId)).Value;
        a.Active = b.Active;

        return a;
    }
}
