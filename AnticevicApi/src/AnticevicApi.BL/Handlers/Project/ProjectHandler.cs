using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.Project;

namespace AnticevicApi.BL.Handlers.Project
{
    public class ProjectHandler : Handler<ProjectHandler>, IProjectHandler
    {
        public ProjectHandler(IHandlerContext<ProjectHandler> context) : base(context)
        {

        }

        public IEnumerable<View.Project> Get()
        {
            using (var db = GetMainContext())
            {
                return db.Projects.WhereUser(User.Id)
                                  .OrderBy(x => x.Name)
                                  .ToList()
                                  .Select(x => new View.Project(x));
            }
        }
    }
}
