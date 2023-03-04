using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Geohash;
using ProjectIvy.Model.Binding.Geohash;

namespace ProjectIvy.Api.Controllers.Device
{
    public class GeohashController : BaseController<GeohashController>
    {
        private readonly IGeohashHandler _geohashHandler;

        public GeohashController(ILogger<GeohashController> logger, IGeohashHandler geohashHandler) : base(logger) => _geohashHandler = geohashHandler;

        [HttpGet]
        public async Task<IActionResult> Get(GeohashGetBinding binding) => Ok(await _geohashHandler.GetGeohashes(binding));

        [HttpGet("{geohash}")]
        public async Task<IActionResult> GetGeohash(string geohash)
        {
            var view = await _geohashHandler.GetGeohash(geohash);

            return view is null ? NotFound() : Ok(view);
        }

        [HttpGet("{geohash}/Days")]
        public async Task<IActionResult> GetDays(string geohash) => Ok(await _geohashHandler.GetDays(geohash));

        [HttpGet("{fromGeohash}/To/{toGeohash}")]
        public async Task<IActionResult> GetGeohashToGeohash(string fromGeohash, string toGeohash, [FromQuery] RouteTimeSort orderBy = RouteTimeSort.Date) => Ok(await _geohashHandler.FromGeohashToGeohash(fromGeohash, toGeohash, orderBy));

        [HttpGet("Root/Children")]
        public async Task<IActionResult> GetRootChildren() => Ok(await _geohashHandler.GetChildren(null));

        [HttpGet("{geohash}/Children")]
        public async Task<IActionResult> GetChildren(string geohash) => Ok(await _geohashHandler.GetChildren(geohash));

        [HttpGet("{geohash}/City")]
        public async Task<IActionResult> GetCity(string geohash) => Ok(await _geohashHandler.GetCity(geohash));

        [HttpGet("{geohash}/Country")]
        public async Task<IActionResult> GetCountry(string geohash) => Ok(await _geohashHandler.GetCountry(geohash));
    }
}
