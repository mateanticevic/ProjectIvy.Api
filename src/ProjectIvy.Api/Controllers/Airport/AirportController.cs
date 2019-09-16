using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Airport;
using ProjectIvy.Model.Binding.Airport;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Airport;

namespace ProjectIvy.Api.Controllers.Airport
{
    public class AirportController : BaseController<AirportController>
    {
        private readonly IAirportHandler _airportHandler;

        public AirportController(ILogger<AirportController> logger, IAirportHandler airportHandler) : base(logger)
        {
            _airportHandler = airportHandler;
        }

        [HttpGet]
        public PagedView<View.Airport> Get([FromQuery] AirportGetBinding binding) => _airportHandler.Get(binding);

        [HttpGet("Count")]
        public long GetCount([FromQuery] AirportGetBinding binding) => _airportHandler.Count(binding);
    }
}
