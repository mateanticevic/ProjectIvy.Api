using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Task;
using ProjectIvy.Model.Binding.Task;
using ProjectIvy.Model.Constants.Database;
using System.Collections.Generic;
using ViewModel = ProjectIvy.Model.View.Task;

namespace ProjectIvy.Api.Controllers.Task
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class TaskController : BaseController<TaskController>
    {
        private readonly ITaskHandler _taskHandler;

        public TaskController(ILogger<TaskController> logger, ITaskHandler taskHandler) : base(logger) => _taskHandler = taskHandler;

        [HttpGet]
        public IEnumerable<ViewModel.Task> Get(TaskGetBinding binding) => _taskHandler.Get(binding);

        [HttpGet(nameof(ViewModel.Priority))]
        public IEnumerable<ViewModel.Priority> GetPriorities() => _taskHandler.GetPriorities();

        [HttpGet(nameof(ViewModel.Status))]
        public IEnumerable<ViewModel.Status> GetStatuses() => _taskHandler.GetStatuses();

        [HttpGet(nameof(ViewModel.Type))]
        public IEnumerable<ViewModel.Type> GetTypes() => _taskHandler.GetTypes();
    }
}
