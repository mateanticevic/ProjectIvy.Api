using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.City;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Model.Binding.City;
using ProjectIvy.Model.Binding.Geohash;
using System.Threading.Tasks;
using View = ProjectIvy.Model.View.City;

namespace ProjectIvy.Api.Controllers.City
{
    public class CityController : BaseController<CityController>
    {
        private readonly ICityHandler _cityHandler;
        private readonly IGeohashHandler _geohashHandler;

        public CityController(ILogger<CityController> logger, ICityHandler cityHandler, IGeohashHandler geohashHandler) : base(logger)
        {
            _cityHandler = cityHandler;
            _geohashHandler = geohashHandler;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CityGetBinding binding) => Ok(await _cityHandler.Get(binding));

        [HttpGet("{cityId}/Geohash")]
        public async Task<IActionResult> GetGeohashes(string cityId) => Ok(await _geohashHandler.GetCityGeohashes(cityId));

        [HttpGet("{fromCityId}/To/{toCityId}/Route")]
        public async Task<IActionResult> GetRoutes(string fromCityId, string toCityId, [FromQuery] RouteTimeSort orderBy = RouteTimeSort.Date)
            => Ok(await _cityHandler.GetRoutes(fromCityId, toCityId, orderBy));

        [HttpGet("Visited")]
        public IEnumerable<View.City> GetVisited() => _cityHandler.GetVisited();

        [HttpPost("Visited/{cityId}")]
        public async Task<IActionResult> PostVisited(string cityId)
        {
            await _cityHandler.AddVisitedCity(cityId);
            return Ok();
        }
    }
}