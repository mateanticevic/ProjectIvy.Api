using ProjectIvy.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProjectIvy.Model.View
{
    public static class Extensions
    {
        public static IEnumerable<GroupedByMonth<T>> FillMissingMonths<T>(this IEnumerable<GroupedByMonth<T>> items, Func<DateTime, GroupedByMonth<T>> factory, DateTime fromMonth, DateTime toMonth)
        {
            var existingMonths = items.Select(x => new DateTime(x.Year, x.Month, 1));
            var allMonths = fromMonth.RangeMonths(toMonth);

            var missingMonths = allMonths.Except(existingMonths);

            return missingMonths.Select(factory)
                                .Union(items)
                                .OrderByDescending(x => x.Year)
                                .ThenByDescending(x => x.Month);
        }

        public static IEnumerable<GroupedByYear<T>> FillMissingYears<T>(this IEnumerable<GroupedByYear<T>> items, Func<int, GroupedByYear<T>> factory, int fromYear, int toYear)
        {
            var existingYears = items.Select(x => x.Year);
            var allYears = Enumerable.Range(fromYear, toYear - fromYear + 1);

            return allYears.Except(existingYears)
                           .Select(factory)
                           .Union(items)
                           .OrderByDescending(x => x.Year);
        }
    }
}
