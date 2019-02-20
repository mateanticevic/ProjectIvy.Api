using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Task;
using ProjectIvy.Model.Binding.Task;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Project
{
    [Authorize(Roles = UserRole.User)]
    [Route("project/{projectId}/Task")]
    public class ProjectTaskController : BaseController<ProjectTaskController>
    {
        private readonly ITaskHandler _taskHandler;

        public ProjectTaskController(ILogger<ProjectTaskController> logger, ITaskHandler taskHandler) : base(logger) => _taskHandler = taskHandler;

        [HttpPost("{taskId}/Change")]
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

        [HttpDelete("{taskId}")]
        public StatusCodeResult DeleteTask(string projectId, string taskId)
        {
            _taskHandler.Delete(projectId, taskId);

            return new StatusCodeResult(StatusCodes.Status200OK);
        }
    }
}
