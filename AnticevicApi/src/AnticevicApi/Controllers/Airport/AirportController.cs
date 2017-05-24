using AnticevicApi.BL.Handlers.Airport;
using AnticevicApi.Common.Configuration;
using AnticevicApi.Model.Binding.Airport;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using View = AnticevicApi.Model.View.Airport;

namespace AnticevicApi.Controllers.Airport
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
        public PaginatedView<View.Airport> Get([FromQuery] AirportGetBinding binding)
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
