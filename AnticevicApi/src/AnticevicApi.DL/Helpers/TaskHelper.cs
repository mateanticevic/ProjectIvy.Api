using AnticevicApi.DL.DbContexts;
using System.Linq;

namespace AnticevicApi.DL.Helpers
{
    public class TaskHelper
    {
        public static string LastValueId(int projectId)
        {
            using (var db = new MainContext(""))
            {
                return db.Tasks.Where(x => x.ProjectId == projectId)
                               .OrderByDescending(x => x.ValueId.Count())
                               .ThenByDescending(x => x.ValueId)
                               .FirstOrDefault()
                               .ValueId;
            }
        } 
    }
}
