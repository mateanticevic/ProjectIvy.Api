using ProjectIvy.Model.Database.Main.Finance;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.DL.Extensions.Entities
{
    public static class ExpenseTypesExtensions
    {
        public static bool IsChildType(this ExpenseType type, IEnumerable<int> parentIds)
        {
            foreach (int parentId in parentIds)
            {
                if (type.IsChildType(parentId))
                    return true;
            }
            return false;
        }

        public static bool IsChildType(this ExpenseType type, int parentId)
        {
            while (true)
            {
                if (!type.ParentTypeId.HasValue)
                    return false;
                if (type.ParentTypeId == parentId)
                    return true;

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
}
