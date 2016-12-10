using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.Project;

namespace AnticevicApi.BL.Handlers.Project
{
    public class ProjectHandler : Handler, IProjectHandler
    {
        public IEnumerable<View.Project> Get()
        {
            using (var db = new MainContext(ConnectionString))
            {
                return db.Projects.WhereUser(UserId)
                                  .OrderBy(x => x.Name)
                                  .ToList()
                                  .Select(x => new View.Project(x));
            }
        }
    }
}
