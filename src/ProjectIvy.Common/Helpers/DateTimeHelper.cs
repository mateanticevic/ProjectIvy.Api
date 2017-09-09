using System;

namespace ProjectIvy.Common.Helpers
{
    public class DateTimeHelper
    {
        public static DateTime FromUnix(double uts)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dt.AddSeconds(uts).ToLocalTime();
        }

        public static long ToUnix(DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan span = dateTime - epoch;

            return (long)span.TotalSeconds;
        }
    }
}
