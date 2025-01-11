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
    public StatusCodeResult Delete(string tripId)
    {
        _tripHandler.Delete(tripId);

        return new StatusCodeResult(StatusCodes.Status200OK);
    }

    [HttpDelete("{tripId}/City/{cityId}")]
    public StatusCodeResult DeleteCity(string tripId, string cityId)
    {
        _tripHandler.RemoveCity(tripId, cityId);

        return new StatusCodeResult(StatusCodes.Status200OK);
    }

    [HttpDelete("{tripId}/Expense/{expenseId}")]
    public StatusCodeResult DeleteExpense(string tripId, string expenseId)
    {
        _tripHandler.RemoveExpense(tripId, expenseId);

        return new StatusCodeResult(StatusCodes.Status200OK);
    }

    [HttpDelete("{tripId}/Poi/{poiId}")]
    public StatusCodeResult DeletePoi(string tripId, string poiId)
    {
        _tripHandler.RemovePoi(tripId, poiId);

        return new StatusCodeResult(StatusCodes.Status200OK);
    }

    [HttpGet]
    public PagedView<View.Trip> Get(TripGetBinding binding) => _tripHandler.Get(binding);

    [HttpGet("{tripId}")]
    public View.Trip Get(string tripId) => _tripHandler.GetSingle(tripId);

    [HttpGet("Days/ByYear")]
    public async Task<IActionResult> GetDaysByYear([FromQuery] TripGetBinding binding) => Ok(await _tripHandler.DaysByYear(binding));

    [HttpPost("{tripId}/Poi/{poiId}")]
    public StatusCodeResult PostPoi(string tripId, string poiId)
    {
        _tripHandler.AddPoi(tripId, poiId);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPost("{tripId}/City/{cityId}")]
    public StatusCodeResult PostCity(string tripId, string cityId)
    {
        _tripHandler.AddCity(tripId, cityId);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPost("{tripId}/Expense/{expenseId}")]
    public StatusCodeResult PostExpense(string tripId, string expenseId)
    {
        _tripHandler.AddExpense(tripId, expenseId);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPost("")]
    public StatusCodeResult Post([FromBody] TripBinding binding, string tripId)
    {
        _tripHandler.Create(binding);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }
}