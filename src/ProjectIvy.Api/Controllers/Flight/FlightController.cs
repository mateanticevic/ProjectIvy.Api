using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Flight;
using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.Flight
{
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

        [HttpGet]
        public PagedView<Model.View.Flight.Flight> Get([FromQuery] FlightGetBinding binding) => _flightHandler.Get(binding);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FlightBinding binding)
        {
            await _flightHandler.Create(binding);
            return Ok();
        }
    }
}