using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Flight;
using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Flight
{
    [Authorize(Roles = UserRole.User)]
    public class FlightController : BaseController<FlightController>
    {
        private readonly IFlightHandler _flightHandler;

        public FlightController(ILogger<FlightController> logger, IFlightHandler flightHandler) : base(logger)
        {
            _flightHandler = flightHandler;
        }

        [HttpGet("Count")]
        public IActionResult GetCount(FlightGetBinding binding) => Ok(_flightHandler.Count(binding));

        [HttpGet("Count/ByAirport")]
        public IActionResult GetCountByAirport(FlightGetBinding binding) => Ok(_flightHandler.CountByAirport(binding));

        [HttpGet("Count/ByYear")]
        public IActionResult GetCountByYear(FlightGetBinding binding) => Ok(_flightHandler.CountByYear(binding));

        [HttpGet("")]
        public IActionResult Get([FromQuery] FlightGetBinding binding) => Ok(_flightHandler.Get(binding));
    }
}