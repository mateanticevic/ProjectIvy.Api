using AnticevicApi.BL.Handlers.Project;
using AnticevicApi.BL.Handlers.Task;
using AnticevicApi.Common.Configuration;
using AnticevicApi.Model.Binding.Task;
using AnticevicApi.Model.View.Project;
using AnticevicApi.Model.View.Task;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ProjectController : BaseController<ProjectController>
    {
        public ProjectController(IOptions<AppSettings> options, ILogger<ProjectController> logger, IProjectHandler projectHandler,
                                                                                                   ITaskHandler taskHandler) : base(options, logger)
        {
            ProjectHandler = projectHandler;
            TaskHandler = taskHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<Project> Get()
        {
            return ProjectHandler.Get();
        }

        [HttpGet]
        [Route("{valueId}/tasks")]
        public IEnumerable<Task> GetTasks(string valueId)
        {
            return TaskHandler.Get(valueId);
        }

        [HttpGet]
        [Route("{valueId}/task/{taskValueId}")]
        public Task GetTask(string valueId, string taskValueId)
        {
            return TaskHandler.Get(valueId, taskValueId);
        }

        [HttpPut]
        [Route("{valueId}/task")]
        public string PutTask([FromBody] TaskBinding binding, string valueId)
        {
            binding.ProjectId = valueId;
            return TaskHandler.Create(binding);
        }

        #endregion
    }
}
