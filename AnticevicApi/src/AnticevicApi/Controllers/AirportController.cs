using AnticevicApi.BL.Handlers.Airport;
using AnticevicApi.Common.Configuration;
using AnticevicApi.Model.View.Airport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Controllers
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
        public IEnumerable<Airport> Get([FromQuery] bool onlyVisited)
        {
            return _airportHandler.Get(onlyVisited);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] bool onlyVisited)
        {
            return _airportHandler.Get(onlyVisited).Count();
        }
    }
}
