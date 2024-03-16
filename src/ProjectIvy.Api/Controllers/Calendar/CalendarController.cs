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

        [HttpDelete("{date:datetime}")]
        public async Task<IActionResult> Delete(DateTime date)
        {
            await _vacationHandler.DeleteVacation(date);
            return Ok();
        }

        [HttpPost("{date:datetime}")]
        public async Task<IActionResult> Post(DateTime date)
        {
            await _vacationHandler.CreateVacation(date);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] FilteredPagedBinding binding) => Ok(await _vacationHandler.Get(binding));

        [HttpPatch("{date:datetime}")]
        public async Task<IActionResult> PatchDay(DateTime date, [FromBody] CalendarDayUpdateBinding b)
        {
            await _vacationHandler.UpdateDay(date, b);
            return Ok();
        }
    }
}
