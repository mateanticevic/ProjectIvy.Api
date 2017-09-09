using ProjectIvy.BL.Handlers.Poi;
using ProjectIvy.Model.Binding.Poi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Api.Controllers.Poi
{
    [Route("[controller]")]
    public class PoiController : BaseController<PoiController>
    {
        private readonly IPoiHandler _poiHandler;

        public PoiController(ILogger<PoiController> logger, IPoiHandler poiHandler) : base(logger)
        {
            _poiHandler = poiHandler;
        }

        [Route("{poiId}")]
        [HttpPost]
        public StatusCodeResult Post([FromBody] PoiBinding binding, string poiId)
        {
            binding.Id = poiId;
            _poiHandler.Create(binding);

            return new StatusCodeResult(StatusCodes.Status201Created);
        }
    }
}