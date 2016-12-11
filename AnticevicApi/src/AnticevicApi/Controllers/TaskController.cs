using AnticevicApi.BL.Handlers.Task;
using AnticevicApi.Config;
using AnticevicApi.Model.View.Task;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class TaskController : BaseController<TaskController>
    {
        public TaskController(IOptions<AppSettings> options, ILogger<TaskController> logger, ITaskHandler taskHandler) : base(options, logger)
        {
            TaskHandler = taskHandler;
        }

        #region Get

        [HttpGet]
        public IEnumerable<Task> Get(string status, string priority, string type)
        {
            return TaskHandler.Get(status, priority, type);
        }

        #endregion
    }
}
