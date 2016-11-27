using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Model.Constants.Database
{
    public class TaskPriorities
    {
        public static KeyValuePair<int, string> Normal = new KeyValuePair<int, string>(1, "normal");
        public static KeyValuePair<int, string> Minor = new KeyValuePair<int, string>(2, "minor");
        public static KeyValuePair<int, string> Important = new KeyValuePair<int, string>(3, "important");
        public static KeyValuePair<int, string> Urgent = new KeyValuePair<int, string>(4, "urgent");

        private static IEnumerable<KeyValuePair<int, string>> All()
        {
            return new List<KeyValuePair<int, string>>()
            {
                Normal,
                Minor,
                Important,
                Urgent
            };
        }

        public static int GetId(string valueId)
        {
            return All().SingleOrDefault(x => x.Value == valueId).Key;
        }
    }
}
