using AnticevicApi.BL.Handlers.Task;
using AnticevicApi.Model.Binding.Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnticevicApi.Controllers.Project
{
    [Route("project/{projectId}/task")]
    public class ProjectTaskController : BaseController<ProjectTaskController>
    {
        private readonly ITaskHandler _taskHandler;

        public ProjectTaskController(ILogger<ProjectTaskController> logger, ITaskHandler taskHandler) : base(logger)
        {
            _taskHandler = taskHandler;
        }

        [HttpPost]
        [Route("{taskId}/change")]
        public StatusCodeResult PostTaskChange([FromBody] TaskChangeBinding binding, string projectId, string taskId)
        {
            try
            {
                binding.ProjectId = projectId;
                binding.TaskId = taskId;

                _taskHandler.CreateChange(binding);
                return new StatusCodeResult(StatusCodes.Status201Created);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
