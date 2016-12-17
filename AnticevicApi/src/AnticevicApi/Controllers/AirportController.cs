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
        public AirportController(IOptions<AppSettings> options, ILogger<AirportController> logger, IAirportHandler airportHandler) : base(options, logger)
        {
            AirportHandler = airportHandler;
        }

        [HttpGet]
        public IEnumerable<Airport> Get([FromQuery] bool onlyVisited)
        {
            return AirportHandler.Get(onlyVisited);
        }

        [HttpGet]
        [Route("count")]
        public int GetCount([FromQuery] bool onlyVisited)
        {
            return AirportHandler.Get(onlyVisited).Count();
        }
    }
}
