using System.Collections.Generic;
using System;

namespace ProjectIvy.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static IEnumerable<DateTime> Range(this DateTime from, DateTime to)
        {
            yield return from;

            var current = from;

            while (current != to)
            {
                current = current.AddDays(1);
                yield return current;
            }

            yield return to;
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

            yield return current.AddMonths(1);
        }

        public static long ToUnix(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var span = dateTime - epoch;

            return (long)span.TotalSeconds;
        }
    }
}
