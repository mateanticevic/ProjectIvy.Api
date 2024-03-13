using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Vendor;

namespace ProjectIvy.Api.Controllers.Route
{
    public class VacationController : BaseController<RouteController>
    {
        private readonly IVacationHandler _vacationHandler;

        public VacationController(ILogger<RouteController> logger, IVacationHandler vacationHandler) : base(logger)
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
    }
}
