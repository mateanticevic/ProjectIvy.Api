using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            if(collection == null)
            {
                return new List<T>();
            }

            return collection;
        }
    }
}
