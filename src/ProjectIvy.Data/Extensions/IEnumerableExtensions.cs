using ProjectIvy.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.Data.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> FillMissingDates<T>(this IEnumerable<T> items, Func<T, DateTime> dateSelector, Func<DateTime, T> emptyFactory, DateTime? from, DateTime to)
        {
            var hasDates = items.Select(dateSelector)
                                .ToList();

            var allDates = (from ?? hasDates.Min()).Range(to);

            var missingDates = allDates.Except(hasDates);

            return missingDates.Select(emptyFactory)
                               .Union(items)
                               .OrderByDescending(dateSelector);
        }
    }
}
