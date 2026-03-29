using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Constants;
using ProjectIvy.Business.Handlers.ToDo;
using ProjectIvy.Model.Binding.ToDo;
using ProjectIvy.Model.View;

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
    public async Task<PagedView<Model.View.ToDo.ToDo>> Get([FromQuery] ToDoGetBinding binding) => await _toDoHandler.Get(binding);

    [HttpGet("Count/ByTag")]
    public async Task<IEnumerable<KeyValuePair<Model.View.Tag.Tag, int>>> GetCountByTag([FromQuery] ToDoGetBinding binding)
        => await _toDoHandler.GetCountByTag(binding);

    [HttpGet("Count/ByTrip")]
    public async Task<IEnumerable<KeyValuePair<Model.View.Trip.Trip, int>>> GetCountByTrip([FromQuery] ToDoGetBinding binding)
        => await _toDoHandler.GetCountByTrip(binding);

    [HttpGet("Sum/ByCurrency")]
    public async Task<IEnumerable<KeyValuePair<Model.View.Currency.Currency, decimal>>> GetSumByCurrency([FromQuery] ToDoGetBinding binding)
        => await _toDoHandler.SumByCurrency(binding);

    [HttpPost]
    public async Task<StatusCodeResult> Post([FromBody] ToDoBinding binding)
    {
        await _toDoHandler.Create(binding);
        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPut("{id}")]
    public async Task Put(string id, [FromBody] ToDoBinding binding) => await _toDoHandler.Update(id, binding);

    [HttpPost("{id}/tag/{tagId}")]
    public async Task<StatusCodeResult> PostTag(string id, string tagId)
    {
        await _toDoHandler.LinkTag(id, tagId);
        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpDelete("{id}/tag/{tagId}")]
    public async Task<StatusCodeResult> DeleteTag(string id, string tagId)
    {
        await _toDoHandler.UnlinkTag(id, tagId);
        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}
