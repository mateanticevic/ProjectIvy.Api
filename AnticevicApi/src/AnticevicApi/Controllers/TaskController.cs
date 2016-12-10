using AnticevicApi.BL.Handlers.Task;
using AnticevicApi.Config;
using AnticevicApi.Model.View.Task;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class TaskController : BaseController
    {
        public TaskController(IOptions<AppSettings> options, ITaskHandler taskHandler) : base(options)
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
