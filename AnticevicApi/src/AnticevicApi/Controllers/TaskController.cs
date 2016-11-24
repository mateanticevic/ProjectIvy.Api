using AnticevicApi.Model.View.Task;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class TaskController : BaseController
    {
        [HttpGet]
        public IEnumerable<Task> Get(string status, string priority, string type)
        {
            return TaskHandler.GetBy(status, priority, type);
        }
    }
}
