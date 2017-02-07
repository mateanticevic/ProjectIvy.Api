using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Model.Constants.Database
{
    public class TaskPriorities
    {
        public static KeyValuePair<int, string> Normal { get; } = new KeyValuePair<int, string>(1, "normal");

        public static KeyValuePair<int, string> Minor { get; } = new KeyValuePair<int, string>(2, "minor");

        public static KeyValuePair<int, string> Important { get; } = new KeyValuePair<int, string>(3, "important");

        public static KeyValuePair<int, string> Urgent { get; } = new KeyValuePair<int, string>(4, "urgent");

        public static int GetId(string valueId)
        {
            return All().SingleOrDefault(x => x.Value == valueId).Key;
        }

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
    }
}
