using AnticevicApi.BL.Handlers;
using AnticevicApi.Model.Database.Main.Security;
using AnticevicApi.Model.View.Airport;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class AirportController : BaseController
    {
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
