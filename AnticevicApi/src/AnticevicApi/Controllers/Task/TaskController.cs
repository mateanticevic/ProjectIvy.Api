using AnticevicApi.BL.Handlers.Task;
using AnticevicApi.Model.Binding.Task;
using AnticevicApi.Model.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using View = AnticevicApi.Model.View.Task;

namespace AnticevicApi.Controllers.Task
{
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
        public IEnumerable<View.Task> Get(TaskGetBinding binding)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(Get), binding);

            return _taskHandler.Get(binding);
        }

        #endregion
    }
}
