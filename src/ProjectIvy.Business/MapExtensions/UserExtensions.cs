using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.User;
using ProjectIvy.Model.Database.Main.User;

namespace ProjectIvy.Business.MapExtensions
{
    public static class UserExtensions
    {
        public static User ToEntity(this UserUpdateBinding binding, MainContext context, User entity)
        {
            if (binding.DefaultCarId is not null)
                entity.DefaultCarId = binding.DefaultCarId == string.Empty ? null : context.Cars.GetId(binding.DefaultCarId).Value;

            if (binding.DefaultCurrencyId is not null)
                entity.DefaultCurrencyId = context.Currencies.GetId(binding.DefaultCurrencyId).Value;

            return entity;
        }
    }
}

