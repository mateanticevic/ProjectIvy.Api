using System;

namespace ProjectIvy.Extensions.BuiltInTypes
{
    public static class ObjectExtensions
    {
        public static T2 ConvertTo<T1, T2>(this T1 o, Func<T1, T2> convert) where T2 : class
        {
            if (o == null)
            {
                return null;
            }
            else
            {
                return convert.Invoke(o);
            }
        }

        public static T DefaultIfNull<T>(this T o) where T : class, new()
        {
            if (o == null)
            {
                o = new T();
            }

            return o;
        }
    }
}
