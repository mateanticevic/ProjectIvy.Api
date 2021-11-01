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

        [HttpGet("{geohashId}")]
        public async Task<IActionResult> GetGeohash(string geohashId)
        {
            var geohash = await _geohashHandler.GetGeohash(geohashId);

            return geohash is null ? NotFound() : Ok(geohash);
        }
    }
}
