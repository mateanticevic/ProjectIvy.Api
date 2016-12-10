using System.Collections.Generic;
using View = AnticevicApi.Model.View.Project;

namespace AnticevicApi.BL.Handlers.Project
{
    public interface IProjectHandler : IHandler
    {
        IEnumerable<View.Project> Get();
    }
}
