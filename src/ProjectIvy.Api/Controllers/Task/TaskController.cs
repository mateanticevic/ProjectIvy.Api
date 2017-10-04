using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Task;
using ProjectIvy.Model.Binding.Task;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.Constants;
using System.Collections.Generic;
using ViewModel = ProjectIvy.Model.View.Task;

namespace ProjectIvy.Api.Controllers.Task
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class TaskController : BaseController<TaskController>
    {
        private readonly ITaskHandler _taskHandler;

        public TaskController(ILogger<TaskController> logger, ITaskHandler taskHandler) : base(logger)
        {
            _taskHandler = taskHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<ViewModel.Task> Get(TaskGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), binding);

            return _taskHandler.Get(binding);
        }

        [HttpGet]
        [Route(nameof(ViewModel.Priority))]
        public IEnumerable<ViewModel.Priority> GetPriorities()
        {
            return _taskHandler.GetPriorities();
        }

        [HttpGet]
        [Route(nameof(ViewModel.Status))]
        public IEnumerable<ViewModel.Status> GetStatuses()
        {
            return _taskHandler.GetStatuses();
        }

        [HttpGet]
        [Route(nameof(ViewModel.Type))]
        public IEnumerable<ViewModel.Type> GetTypes()
        {
            return _taskHandler.GetTypes();
        }

        #endregion
    }
}
