using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Flight;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Flight
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class FlightController : BaseController<FlightController>
    {
        private readonly IFlightHandler _flightHandler;

        public FlightController(ILogger<FlightController> logger, IFlightHandler flightHandler) : base(logger)
        {
            _flightHandler = flightHandler;
        }

        [HttpGet("Count")]
        public IActionResult GetCount() => Ok(_flightHandler.Count());

        [HttpGet("Count/ByAirport")]
        public IActionResult GetCountByAirport() => Ok(_flightHandler.CountByAirport());

        [HttpGet("")]
        public IActionResult Get([FromQuery] FilteredPagedBinding binding) => Ok(_flightHandler.Get(binding));
    }
}