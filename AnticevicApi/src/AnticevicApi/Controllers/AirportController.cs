using AnticevicApi.BL.Handlers.Airport;
using AnticevicApi.Config;
using AnticevicApi.Model.View.Airport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class AirportController : BaseController
    {
        public AirportController(IOptions<AppSettings> options, IAirportHandler airportHandler) : base(options)
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
