using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Project;
using ProjectIvy.Business.Handlers.Task;
using ProjectIvy.Model.Binding.Task;
using ProjectIvy.Model.Constants.Database;
using System.Collections.Generic;
using ViewProject = ProjectIvy.Model.View.Project;
using ViewTask = ProjectIvy.Model.View.Task;

namespace ProjectIvy.Api.Controllers.Project
{
    [Authorize(Roles = UserRole.User)]
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
        public IEnumerable<ViewProject.Project> Get() => _projectHandler.Get();

        [HttpGet("{valueId}/Tasks")]
        public IEnumerable<ViewTask.Task> GetTasks(string valueId) => _taskHandler.Get(valueId);

        [HttpGet("{valueId}/Task/{taskValueId}")]
        public ViewTask.Task GetTask(string valueId, string taskValueId) => _taskHandler.Get(valueId, taskValueId);

        #endregion

        #region Put

        [HttpPut]
        [Route("{valueId}/Task")]
        public string PutTask([FromBody] TaskBinding binding, string valueId)
        {
            binding.ProjectId = valueId;
            return _taskHandler.Create(binding);
        }

        #endregion
    }
}
