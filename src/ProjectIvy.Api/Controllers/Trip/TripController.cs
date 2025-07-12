using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Trip;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Trip;

namespace ProjectIvy.Api.Controllers.Trip;

public class TripController : BaseController<TripController>
{
    private readonly ITripHandler _tripHandler;

    public TripController(ILogger<TripController> logger, ITripHandler tripHandler) : base(logger) => _tripHandler = tripHandler;

    [HttpDelete("{tripId}")]
    public async Task<StatusCodeResult> Delete(string tripId)
    {
        await _tripHandler.Delete(tripId);

        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpDelete("{tripId}/City/{cityId}")]
    public async Task<StatusCodeResult> DeleteCity(string tripId, string cityId)
    {
        await _tripHandler.RemoveCity(tripId, cityId);

        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpDelete("{tripId}/Expense/{expenseId}")]
    public async Task<StatusCodeResult> DeleteExpense(string tripId, string expenseId)
    {
        await _tripHandler.RemoveExpense(tripId, expenseId);

        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpDelete("{tripId}/Poi/{poiId}")]
    public async Task<StatusCodeResult> DeletePoi(string tripId, string poiId)
    {
        await _tripHandler.RemovePoi(tripId, poiId);

        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }

    [HttpGet]
    public async Task<PagedView<View.Trip>> Get(TripGetBinding binding) => await _tripHandler.Get(binding);

    [HttpGet("{tripId}")]
    public async Task<View.Trip> Get(string tripId)
        => await _tripHandler.GetSingle(tripId);

    [HttpGet("Days/ByYear")]
    public async Task<IActionResult> GetDaysByYear([FromQuery] TripGetBinding binding) => Ok(await _tripHandler.DaysByYear(binding));

    [HttpPost("{tripId}/Poi/{poiId}")]
    public async Task<StatusCodeResult> PostPoi(string tripId, string poiId)
    {
        await _tripHandler.AddPoi(tripId, poiId);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPost("{tripId}/City/{cityId}")]
    public async Task<StatusCodeResult> PostCity(string tripId, string cityId)
    {
        await _tripHandler.AddCity(tripId, cityId);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPost("{tripId}/Expense/{expenseId}")]
    public async Task<StatusCodeResult> PostExpense(string tripId, string expenseId)
    {
        await _tripHandler.AddExpense(tripId, expenseId);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPost]
    public async Task<StatusCodeResult> Post([FromBody] TripBinding binding)
    {
        await _tripHandler.Create(binding);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }
}