using System;

namespace ProjectIvy.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static T2 ConvertTo<T1, T2>(this T1 o, Func<T1, T2> convert) where T2 : class
        {
            return o == null ? null : convert.Invoke(o);
        }

        public static T DefaultIfNull<T>(this T o) where T : class, new()
        {
            return o ?? (o = new T());
        }
    }
}
