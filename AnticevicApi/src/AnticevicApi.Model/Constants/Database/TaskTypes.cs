using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Model.Constants.Database
{
    public static class TaskTypes
    {
        public static KeyValuePair<int, string> Coding { get; } = new KeyValuePair<int, string>(1, "coding");

        public static KeyValuePair<int, string> Buy { get; } = new KeyValuePair<int, string>(2, "buy");

        public static KeyValuePair<int, string> Write { get; } = new KeyValuePair<int, string>(3, "write");

        public static KeyValuePair<int, string> HouseChours { get; } = new KeyValuePair<int, string>(4, "house-chours");

        public static KeyValuePair<int, string> HouseRepair { get; } = new KeyValuePair<int, string>(5, "house-repair");

        public static KeyValuePair<int, string> Arrange { get; } = new KeyValuePair<int, string>(6, "arrange");

        public static KeyValuePair<int, string> CarRepair { get; } = new KeyValuePair<int, string>(7, "car-repair");

        public static KeyValuePair<int, string> Research { get; } = new KeyValuePair<int, string>(8, "research");

        public static int GetId(string valueId)
        {
            return All().SingleOrDefault(x => x.Value == valueId).Key;
        }

        private static IEnumerable<KeyValuePair<int, string>> All()
        {
            return new List<KeyValuePair<int, string>>()
            {
                Coding,
                Buy,
                Write,
                HouseChours,
                HouseRepair,
                Arrange,
                CarRepair,
                Research
            };
        }
    }
}
