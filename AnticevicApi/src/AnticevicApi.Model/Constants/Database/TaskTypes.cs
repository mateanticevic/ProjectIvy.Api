using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Model.Constants.Database
{
    public static class TaskTypes
    {
        public static KeyValuePair<int, string> Coding = new KeyValuePair<int, string>(1, "coding");
        public static KeyValuePair<int, string> Buy = new KeyValuePair<int, string>(2, "buy");
        public static KeyValuePair<int, string> Write = new KeyValuePair<int, string>(3, "write");
        public static KeyValuePair<int, string> HouseChours = new KeyValuePair<int, string>(4, "house-chours");
        public static KeyValuePair<int, string> HouseRepair = new KeyValuePair<int, string>(5, "house-repair");
        public static KeyValuePair<int, string> Arrange = new KeyValuePair<int, string>(6, "arrange");
        public static KeyValuePair<int, string> CarRepair = new KeyValuePair<int, string>(7, "car-repair");
        public static KeyValuePair<int, string> Research = new KeyValuePair<int, string>(8, "research");

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

        public static int GetId(string valueId)
        {
            return All().SingleOrDefault(x => x.Value == valueId).Key;
        }
    }
}
