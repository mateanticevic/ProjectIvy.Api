using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.ToDo;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.ToDo
{
    [Authorize(Roles = UserRole.User)]
    public class ToDoController : BaseController<ToDoController>
    {
        private readonly IToDoHandler _toDoHandler;

        public ToDoController(ILogger<ToDoController> logger, IToDoHandler toDoHandler) : base(logger)
        {
            _toDoHandler = toDoHandler;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] ToDoGetBinding binding) => Ok(_toDoHandler.GetPaged(binding));

        [HttpPost]
        public IActionResult Post([FromBody] string name) => Ok(_toDoHandler.Create(name));

        [HttpPost("{id}/Done")]
        public IActionResult PostDone(string id)
        {
            _toDoHandler.SetDone(id);
            return Ok();
        }
    }
}
