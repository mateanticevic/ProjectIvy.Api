using System.Collections.Generic;
using System;

namespace ProjectIvy.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static IEnumerable<DateTime> Range(this DateTime from, DateTime to)
        {
            yield return from;

            var current = from.Date;

            while (current < to.Date)
            {
                current = current.AddDays(1);
                yield return current;
            }

            yield return to.Date;
        }

        public static IEnumerable<DateTime> RangeMonths(this DateTime from, DateTime to)
        {
            yield return from;

            var current = from;

            while (to.Year != current.Year || to.Month - current.Month > 1)
            {
                current = current.AddMonths(1);
                yield return current;
            }

            if (current.Month != 1)
                yield return current.AddMonths(1);
        }

        public static IEnumerable<(DateTime from, DateTime to)> RangeMonthsClosed(this DateTime from, DateTime to)
        {
            if (from.Year == to.Year && from.Month == to.Month)
                yield return (from, to);
            else
            {
                yield return (from, new DateTime(from.Year, from.Month, 1).AddMonths(1).AddDays(-1));

                var current = new DateTime(from.Year, from.Month, 1);

                while ( current.Year != to.Year || to.Month - current.Month > 1)
                {
                    current = current.AddMonths(1);

                    if (current.AddMonths(1).AddDays(-1) < to)
                        yield return (current, current.AddMonths(1).AddDays(-1));
                }

                yield return (new DateTime(to.Year, to.Month, 1), to);
            }
        }

        public static long ToUnix(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var span = dateTime - epoch;

            return (long)span.TotalSeconds;
        }
    }
}
