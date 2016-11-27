using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Model.Constants.Database
{
    public class TaskStatuses
    {
        public static KeyValuePair<int, string> New = new KeyValuePair<int, string>(1, "new");
        public static KeyValuePair<int, string> InProgress = new KeyValuePair<int, string>(2, "in-progress");
        public static KeyValuePair<int, string> Done = new KeyValuePair<int, string>(3, "done");
        public static KeyValuePair<int, string> Discarded = new KeyValuePair<int, string>(4, "discarded");

        private static IEnumerable<KeyValuePair<int, string>> All()
        {
            return new List<KeyValuePair<int, string>>()
            {
                New,
                InProgress,
                Done,
                Discarded
            };
        }

        public static int GetId(string valueId)
        {
            return All().SingleOrDefault(x => x.Value == valueId).Key;
        }
    }
}
