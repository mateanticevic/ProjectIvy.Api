using System;
using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Data.Services.LastFm;

public static class Extensions
{
    public const string Key = "{key}";
    public const string Method = "{method}";
    public const string Username = "{username}";

    public static string SetFrom(this string url, DateTime? from)
    {
        return from.HasValue ? $"{url}&from={from.Value.ToUnix()}" : url;
    }

    public static string SetTo(this string url, DateTime? to)
    {
        return to.HasValue ? $"{url}&to={to.Value.ToUnix()}" : url;
    }

    public static string SetPage(this string url, int? page)
    {
        return page.HasValue && page.Value != 0 ? $"{url}&page={page}" : url;
    }

    public static string SetPageSize(this string url, int? pageSize)
    {
        return pageSize.HasValue ? $"{url}&limit={pageSize}" : url;
    }

    public static string SetPeriod(this string url, string period) => $"{url}&period={period}";

    public static string SetKey(this string url, string key)
    {
        return url.Replace(Key, key);
    }

    public static string SetUsername(this string url, string username)
    {
        return url.Replace(Username, username);
    }

    public static string SetMethod(this string url, string method)
    {
        return url.Replace(Method, method);
    }
}
