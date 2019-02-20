using ProjectIvy.Model.Binding;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.Data.Extensions
{
    public static class GroupedByMonthExtensions
    {
        public static IEnumerable<GroupedByMonth<T>> OrderBy<T>(this IEnumerable<GroupedByMonth<T>> items, IOrderable<GroupedSort> orderable)
        {
            switch (orderable.OrderBy)
            {
                case GroupedSort.Data:
                    return orderable.OrderAscending ? items.OrderBy(x => x.Data) : items.OrderByDescending(x => x.Data);
                default:
                    return orderable.OrderAscending ? items.OrderBy(x => x.Year).ThenBy(x => x.Month) : items.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month);
            }
        }
    }
}
