using System.Threading.Tasks;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.User;
using ProjectIvy.Model.Database.Main.User;

namespace ProjectIvy.Business.MapExtensions;

public static class UserExtensions
{
    public static async Task<User> ToEntity(this UserUpdateBinding binding, MainContext context, User entity)
    {
        if (binding.DefaultCarId is not null)
            entity.DefaultCarId = binding.DefaultCarId == string.Empty ? null : (await context.Cars.GetIdAsync(binding.DefaultCarId)).Value;

        if (binding.DefaultCurrencyId is not null)
            entity.DefaultCurrencyId = (await context.Currencies.GetIdAsync(binding.DefaultCurrencyId)).Value;

        return entity;
    }
}
