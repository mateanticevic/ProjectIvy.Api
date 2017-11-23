using System;

namespace ProjectIvy.Extensions.BuiltInTypes
{
    public static class DateTimeExtensions
    {
        public static long ToUnix(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var span = dateTime - epoch;

            return (long)span.TotalSeconds;
        }
    }
}
