using ProjectIvy.BL.Handlers.Project;
using ProjectIvy.BL.Handlers.Task;
using ProjectIvy.Model.Binding.Task;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using ViewProject = ProjectIvy.Model.View.Project;
using ViewTask = ProjectIvy.Model.View.Task;

namespace ProjectIvy.Api.Controllers.Project
{
    [Route("[controller]")]
    public class ProjectController : BaseController<ProjectController>
    {
        private readonly IProjectHandler _projectHandler;
        private readonly ITaskHandler _taskHandler;

        public ProjectController(ILogger<ProjectController> logger, IProjectHandler projectHandler, ITaskHandler taskHandler) : base(logger)
        {
            _projectHandler = projectHandler;
            _taskHandler = taskHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<ViewProject.Project> Get()
        {
            return _projectHandler.Get();
        }

        [HttpGet]
        [Route("{valueId}/tasks")]
        public IEnumerable<ViewTask.Task> GetTasks(string valueId)
        {
            return _taskHandler.Get(valueId);
        }

        [HttpGet]
        [Route("{valueId}/task/{taskValueId}")]
        public ViewTask.Task GetTask(string valueId, string taskValueId)
        {
            return _taskHandler.Get(valueId, taskValueId);
        }

        #endregion

        #region Put

        [HttpPut]
        [Route("{valueId}/task")]
        public string PutTask([FromBody] TaskBinding binding, string valueId)
        {
            binding.ProjectId = valueId;
            return _taskHandler.Create(binding);
        }

        #endregion
    }
}
