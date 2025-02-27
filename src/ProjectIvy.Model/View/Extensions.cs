using ProjectIvy.Common.Extensions;
using System.Linq;

namespace ProjectIvy.Model.View;

public static class Extensions
{
    public static IEnumerable<KeyValuePair<int, T>> FillMissingMonths<T>(this IEnumerable<KeyValuePair<int, T>> items)
        => Enumerable.Range(1, 12).Select(x => new KeyValuePair<int, T>(x, items.Any(y => y.Key == x) ? items.Single(y => y.Key == x).Value : default));

    public static IEnumerable<GroupedByMonth<T>> FillMissingMonths<T>(this IEnumerable<GroupedByMonth<T>> items, Func<DateTime, GroupedByMonth<T>> factory, DateTime? fromMonth, DateTime toMonth)
    {
        var existingMonths = items.Select(x => new DateTime(x.Year, x.Month, 1)).ToList();

        var allMonths = (fromMonth ?? existingMonths.Min()).RangeMonths(toMonth);

        var missingMonths = allMonths.Except(existingMonths);

        return missingMonths.Select(factory)
                            .Union(items)
                            .OrderByDescending(x => x.Year)
                            .ThenByDescending(x => x.Month);
    }

    public static IEnumerable<KeyValuePair<TKey, int>> FillMissingYears<TKey>(this IEnumerable<KeyValuePair<TKey, int>> items, Func<int, KeyValuePair<TKey, int>> factory, int? fromYear, int toYear)
    {
        var existingYears = items.Select(x => x.Value).ToList();

        int fromYearValue = fromYear ?? existingYears.Min();

        var allYears = Enumerable.Range(fromYearValue, toYear - fromYearValue + 1);

        return allYears.Except(existingYears)
                       .Select(factory)
                       .Union(items)
                       .OrderByDescending(x => x.Value);
    }
}
