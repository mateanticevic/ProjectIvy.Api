using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Calendar;
using ProjectIvy.Model.Binding.Calendar;
using ProjectIvy.Model.View.Calendar;

namespace ProjectIvy.Api.Controllers.Calendar;

public class CalendarController : BaseController<CalendarController>
{
    private readonly ICalendarHandler _vacationHandler;

    public CalendarController(ILogger<CalendarController> logger, ICalendarHandler vacationHandler) : base(logger)
    {
        _vacationHandler = vacationHandler;
    }

    [HttpGet("Days")]
    public async Task<CalendarSection> GetDays([FromQuery] DateTime from, [FromQuery] DateTime to) => await _vacationHandler.Get(from, to);

    [HttpPatch("{date:datetime}")]
    public async Task<IActionResult> PatchDay(DateTime date, [FromBody] CalendarDayUpdateBinding b)
    {
        await _vacationHandler.UpdateDay(date, b);
        return Ok();
    }

    [HttpPost("{date:datetime}/Events")]
    public async Task<IActionResult> PostEvent(DateTime date, [FromBody] string name)
    {
        await _vacationHandler.CreateEvent(date, name);
        return Ok();
    }
}
