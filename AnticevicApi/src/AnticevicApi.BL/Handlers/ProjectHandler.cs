using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.View.Project;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class ProjectHandler : Handler
    {
        public ProjectHandler(int userId) : base(userId)
        {

        }

        public IEnumerable<Project> Get()
        {
            using (var db = new MainContext())
            {
                return db.Projects.WhereUser(UserId)
                                  .OrderBy(x => x.Name)
                                  .ToList()
                                  .Select(x => new Project(x));
            }
        }
    }
}
