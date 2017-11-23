using ProjectIvy.Model.Database.Main.User;
using ProjectIvy.Model.Database.Main;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ProjectIvy.DL.Extensions
{
    public static class DbSetExtensions
    {
        public static User GetById(this DbSet<User> set, int id) => set.SingleOrDefault(x => x.Id == id);

        public static int? GetId<T>(this DbSet<T> set, string valueId) where T : class, IHasValueId
        {
            return string.IsNullOrWhiteSpace(valueId) ? null : set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
        }
    }
}
