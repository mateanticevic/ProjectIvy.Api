using AnticevicApi.DL.DbContexts;
using System.Linq;

namespace AnticevicApi.DL.Helpers
{
    public class ProjectHelper
    {
        public static int GetId(string valueId)
        {
            using (var db = new MainContext(""))
            {
                return db.Projects.SingleOrDefault(x => x.ValueId == valueId)
                                  .Id;
            }
        }
    }
}
