using System;
namespace ProjectIvy.Common.Extensions;

public static class LongExtensions
{
    public static DateTime FromUnixTimestamp(this long timestamp)
    {
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return epoch.AddSeconds(timestamp);
    }
}
