using System.Collections.Generic;
using View = ProjectIvy.Model.View.Project;

namespace ProjectIvy.BL.Handlers.Project
{
    public interface IProjectHandler : IHandler
    {
        IEnumerable<View.Project> Get();
    }
}
