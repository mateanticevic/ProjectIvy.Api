using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Business.Handlers.Location;
using ProjectIvy.Model.Binding.Geohash;

namespace ProjectIvy.Api.Controllers.Location
{
    public class LocationController : BaseController<LocationController>
    {
        private readonly IGeohashHandler _geohashHandler;
        private readonly ILocationHandler _locationHandler;

        public LocationController(ILogger<LocationController> logger,
                                  IGeohashHandler geohashHandler,
                                  ILocationHandler locationHandler) : base(logger)
        {
            _geohashHandler = geohashHandler;
            _locationHandler = locationHandler;
        }

        [HttpGet]
        public async Task<IEnumerable<Model.View.Location.Location>> Get() => await _locationHandler.Get();

        [HttpGet("{locationId}/Days")]
        public async Task<IActionResult> GetDays(string locationId) => Ok(await _locationHandler.GetDays(locationId));

        [HttpPost("{locationId}/Geohashes")]
        public async Task PostGeohashes(string locationId, [FromBody] IEnumerable<string> geohashes) => await _locationHandler.SetGeohashes(locationId, geohashes);

        [HttpPost("{locationId}/Geohashes/{geohash}")]
        public async Task PostLocationGeohash(string locationId, string geohash)
            => await _geohashHandler.AddGeohashToLocation(locationId, geohash);

        [HttpDelete("{locationId}/Geohashes/{geohash}")]
        public async Task DeleteLocationGeohash(string locationId, string geohash)
            => await _geohashHandler.RemoveGeohashFromLocation(locationId, geohash);

        [HttpGet("{fromLocationId}/To/{toLocationId}")]
        public async Task GetRoutes(string fromLocationId, string toLocationId, [FromQuery] RouteTimeSort orderBy)
            => Ok(await _locationHandler.FromLocationToLocation(fromLocationId, toLocationId, orderBy));
    }
}

