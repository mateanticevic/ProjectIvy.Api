using ProjectIvy.BL.Handlers.Task;
using ProjectIvy.Model.Binding.Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Api.Controllers.Project
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

        [HttpDelete]
        [Route("{taskId}")]
        public StatusCodeResult DeleteTask(string projectId, string taskId)
        {
            _taskHandler.Delete(projectId, taskId);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}
