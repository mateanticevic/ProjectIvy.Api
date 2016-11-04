using AnticevicApi.Model.Database.Main.User;
using AnticevicApi.Model.Database.Main;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.DL.Extensions
{
    public static class UserEntityExtensions
    {
        public static IEnumerable<T> WhereUser<T>(this ICollection<T> collection, int userId) where T : UserEntity
        {
            return collection.Where(x => x.UserId == userId);
        }

        public static IEnumerable<T> WhereUser<T>(this ICollection<T> collection, User user) where T : UserEntity
        {
            return collection.Where(x => x.User == user);
        }

        public static IQueryable<T> WhereUser<T>(this IQueryable<T> collection, int userId) where T : UserEntity
        {
            return collection.Where(x => x.UserId == userId);
        }

        public static IQueryable<T> WhereUser<T>(this IQueryable<T> collection, User user) where T : UserEntity
        {
            return collection.Where(x => x.User == user);
        }
    }
}
