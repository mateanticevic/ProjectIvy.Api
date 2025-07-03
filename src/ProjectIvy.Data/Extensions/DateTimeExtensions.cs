using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProjectIvy.Data.Extensions;

public static class DateTimeExtensions
{
    public static readonly DateTime FirstSunday = new DateTime(2000, 1, 2);

    public static int ToDayOfWeek(this DateTime date)
        => (int)EF.Functions.DateDiffDay((DateTime?)FirstSunday, (DateTime?)date) % 7;

    public static IQueryable<IGrouping<int, T>> GroupByDayOfWeek<T>(this IQueryable<T> queryable, Func<T, DateTime> selector)
        => queryable.GroupBy(x => (int)EF.Functions.DateDiffDay((DateTime?)FirstSunday, (DateTime?)selector.Invoke(x)) % 7);
}
