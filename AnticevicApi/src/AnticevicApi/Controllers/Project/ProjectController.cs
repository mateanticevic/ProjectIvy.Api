﻿using AnticevicApi.BL.Handlers.Project;
using AnticevicApi.BL.Handlers.Task;
using AnticevicApi.Model.Binding.Task;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using ViewProject = AnticevicApi.Model.View.Project;
using ViewTask = AnticevicApi.Model.View.Task;

namespace AnticevicApi.Controllers.Project
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