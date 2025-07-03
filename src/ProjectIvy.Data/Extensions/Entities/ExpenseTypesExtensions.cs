using System.Collections.Generic;
using System.Linq;
using ProjectIvy.Model.Database.Main.Finance;

namespace ProjectIvy.Data.Extensions.Entities;

public static class ExpenseTypesExtensions
{
    public static ExpenseType ToParentType(this ExpenseType type)
    {
        while (type.ParentTypeId.HasValue)
        {
            type = type.ParentType;
        }

        return type;
    }

    public static IEnumerable<int> ToChildTypeIds(this IEnumerable<ExpenseType> types, IEnumerable<int> parentTypeIds)
    {
        var firstLevelIds = types.Where(x => x.ParentTypeId.HasValue && parentTypeIds.Contains(x.ParentTypeId.Value))
                                 .Select(x => x.Id);
        var recursiveIds = firstLevelIds.Any() ? types.ToChildTypeIds(firstLevelIds) : new List<int>();

        return firstLevelIds.Concat(recursiveIds);
    }

    public static IEnumerable<int> ToParentTypeIds(this ExpenseType type)
    {
        while (type.ParentTypeId.HasValue)
        {
            yield return type.ParentTypeId.Value;
            type = type.ParentType;
        }
    }

    public static IEnumerable<ExpenseType> GetAll(this IQueryable<ExpenseType> types)
    {
        var all = types.ToList();

        all.ForEach(x => { if (x.ParentTypeId.HasValue) x.ParentType = types.SingleOrDefault(y => y.Id == x.ParentTypeId.Value); });
        return all;
    }
}
