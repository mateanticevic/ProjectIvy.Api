using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.ToDo;
using ProjectIvy.Model.Binding.ToDo;

namespace ProjectIvy.Api.Controllers.ToDo;

[Authorize(ApiScopes.BasicUser)]
public class ToDoController : BaseController<ToDoController>
{
    private readonly IToDoHandler _toDoHandler;

    public ToDoController(ILogger<ToDoController> logger, IToDoHandler toDoHandler) : base(logger)
    {
        _toDoHandler = toDoHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ToDoGetBinding binding) => Ok(await _toDoHandler.Get(binding));
}
