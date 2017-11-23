using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.Common.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection) => collection ?? new List<T>();
    }
}
