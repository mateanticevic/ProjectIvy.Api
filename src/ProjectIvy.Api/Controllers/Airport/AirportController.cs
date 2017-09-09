using ProjectIvy.BL.Handlers.Airport;
using ProjectIvy.Common.Configuration;
using ProjectIvy.Model.Binding.Airport;
using ProjectIvy.Model.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using View = ProjectIvy.Model.View.Airport;

namespace ProjectIvy.Api.Controllers.Airport
{
    [Route("[controller]")]
    public class AirportController : BaseController<AirportController>
    {
        private readonly IAirportHandler _airportHandler;

        public AirportController(IOptions<AppSettings> options, ILogger<AirportController> logger, IAirportHandler airportHandler) : base(logger)
        {
            _airportHandler = airportHandler;
        }

        [HttpGet]
        public PagedView<View.Airport> Get([FromQuery] AirportGetBinding binding)
        {
            return _airportHandler.Get(binding);
        }

        [HttpGet]
        [Route("count")]
        public long GetCount([FromQuery] AirportGetBinding binding)
        {
            return _airportHandler.Count(binding);
        }
    }
}
