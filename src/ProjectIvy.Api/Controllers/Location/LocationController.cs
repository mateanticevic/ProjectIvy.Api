using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Api.Controllers.Flight;
using ProjectIvy.Business.Handlers.Flight;
using ProjectIvy.Business.Handlers.Location;

namespace ProjectIvy.Api.Controllers.Location
{
	public class LocationController : BaseController<LocationController>
    {
        private readonly ILocationHandler _locationHandler;

        public LocationController(ILogger<LocationController> logger, ILocationHandler locationHandler) : base(logger)
        {
            _locationHandler = locationHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _locationHandler.Get());

        [HttpGet("{locationId}/Days")]
        public async Task<IActionResult> GetDays(string locationId) => Ok(await _locationHandler.GetDays(locationId));

        [HttpPost("{locationId}/Geohashes")]
        public async Task PostGeohashes(string locationId, [FromBody] IEnumerable<string> geohashes) => await _locationHandler.SetGeohashes(locationId, geohashes);
    }
}

