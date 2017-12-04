using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.DL.Extensions
{
    public static class GroupedByYearExtensions
    {
        public static IEnumerable<GroupedByYear<T>> OrderBy<T>(this IEnumerable<GroupedByYear<T>> items, IOrderable<GroupedSort> orderable)
        {
            switch (orderable.OrderBy)
            {
                case GroupedSort.Data:
                    return orderable.OrderAscending ? items.OrderBy(x => x.Data) : items.OrderByDescending(x => x.Data);
                default:
                    return orderable.OrderAscending ? items.OrderBy(x => x.Year) : items.OrderByDescending(x => x.Year);
            }
        }
    }
}
