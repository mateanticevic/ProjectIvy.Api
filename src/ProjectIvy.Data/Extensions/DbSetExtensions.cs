using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Model.Database.Main;
using ProjectIvy.Model.Database.Main.User;

namespace ProjectIvy.Data.Extensions;

public static class DbSetExtensions
{
    public static User GetById(this DbSet<User> set, int id) => set.SingleOrDefault(x => x.Id == id);

    public static int? GetId<T>(this DbSet<T> set, string valueId) where T : class, IHasValueId
    {
        return string.IsNullOrWhiteSpace(valueId) ? null : set.SingleOrDefault(x => x.ValueId == valueId)?.Id;
    }

    public static IEnumerable<int> GetIds<T>(this DbSet<T> set, IEnumerable<string> valueIds) where T : class, IHasValueId
    {
        return valueIds != null && valueIds.Any() ? set.Where(x => valueIds.Contains(x.ValueId)).Select(x => x.Id).ToList() : null;
    }
}
