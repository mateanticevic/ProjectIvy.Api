using AnticevicApi.BL.Handlers.Airport;
using AnticevicApi.Common.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<View.Airport> Get([FromQuery] bool onlyVisited)
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
