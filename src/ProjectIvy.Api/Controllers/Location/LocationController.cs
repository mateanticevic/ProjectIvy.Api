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

        [HttpGet("{locationId}/Days")]
        public async Task<IActionResult> GetDays(string locationId) => Ok(await _locationHandler.GetDays(locationId));
    }
}

