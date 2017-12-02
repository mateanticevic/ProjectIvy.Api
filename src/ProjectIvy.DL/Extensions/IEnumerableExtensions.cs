using ProjectIvy.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProjectIvy.DL.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> FillMissingDates<T>(this IEnumerable<T> items, Func<T, DateTime> dateSelector, Func<DateTime, T> emptyFactory, DateTime from, DateTime to)
        {
            var hasDates = items.Select(dateSelector);

            var allDates = from.Range(to);

            var missingDates = allDates.Except(hasDates);

            return missingDates.Select(emptyFactory)
                               .Union(items)
                               .OrderByDescending(dateSelector);
        }
    }
}
