using AnticevicApi.Model.Database.Main.User;
using AnticevicApi.Model.Database.Main;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AnticevicApi.DL.Extensions
{
    public static class DbSetExtensions
    {
        public static User GetById(this DbSet<User> set, int id)
        {
            return set.SingleOrDefault(x => x.Id == id);
        }

        public static int? GetId<T>(this DbSet<T> set, string valueId) where T : class, IHasValueId
        {
            return set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }
    }
}
