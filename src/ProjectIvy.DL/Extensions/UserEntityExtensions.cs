using ProjectIvy.Model.Database.Main.User;
using ProjectIvy.Model.Database.Main;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProjectIvy.DL.Extensions
{
    public static class UserEntityExtensions
    {
        public static IQueryable<T> WhereUser<T>(this DbSet<T> collection, int userId) where T : UserEntity
        {
            return collection.Where(x => x.UserId == userId);
        }

        public static IQueryable<T> WhereUser<T>(this DbSet<T> collection, User user) where T : UserEntity
        {
            return collection.Where(x => x.UserId == user.Id);
        }

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
