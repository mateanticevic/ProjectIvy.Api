using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Vendor;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Calendar;

namespace ProjectIvy.Api.Controllers.Route
{
    public class CalendarController : BaseController<RouteController>
    {
        private readonly ICalendarHandler _vacationHandler;

        public CalendarController(ILogger<RouteController> logger, ICalendarHandler vacationHandler) : base(logger)
        {
            _vacationHandler = vacationHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DateTime from, [FromQuery] DateTime to) => Ok(await _vacationHandler.Get(from, to));

        [HttpPatch("{date:datetime}")]
        public async Task<IActionResult> PatchDay(DateTime date, [FromBody] CalendarDayUpdateBinding b)
        {
            await _vacationHandler.UpdateDay(date, b);
            return Ok();
        }
    }
}
