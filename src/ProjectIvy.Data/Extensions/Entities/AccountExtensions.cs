using System.Linq;
using ProjectIvy.Model.Binding.Account;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Data.Extensions.Entities;

public static class AccountExtensions
{
    public static IQueryable<Account> Where(this IQueryable<Account> query, AccountGetBinding binding)
        => query.WhereIf(binding.IsActive, x => x.Active == binding.IsActive)
                .WhereIf(binding.BankIds, x => x.Bank != null && binding.BankIds.Contains(x.Bank.ValueId))
                .WhereIf(!string.IsNullOrEmpty(binding.Search), x => x.Name.ToLower().Contains(binding.Search.ToLower()));
}
