using AnticevicApi.Model.View.Project;
using AnticevicApi.Model.View.Task;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ProjectController : BaseController
    {
        [HttpGet]
        public IEnumerable<Project> Get()
        {
            return ProjectHandler.Get();
        }

        [HttpGet]
        [Route("{valueId}/tasks")]
        public IEnumerable<Task> GetTasks(string valueId)
        {
            return TaskHandler.GetTasks(valueId);
        }

        [HttpGet]
        [Route("{valueId}/task/{taskValueId}")]
        public Task GetTask(string valueId, string taskValueId)
        {
            return TaskHandler.Get(valueId, taskValueId);
        }
    }
}
