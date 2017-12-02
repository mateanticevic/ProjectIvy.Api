using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Poi;
using ProjectIvy.Model.Binding.Poi;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Poi;

namespace ProjectIvy.Api.Controllers.Poi
{
    [Route("[controller]")]
    public class PoiController : BaseController<PoiController>
    {
        private readonly IPoiHandler _poiHandler;

        public PoiController(ILogger<PoiController> logger, IPoiHandler poiHandler) : base(logger) => _poiHandler = poiHandler;

        [HttpGet]
        public PagedView<View.Poi> Get([FromQuery] PoiGetBinding binding) => _poiHandler.Get(binding);

        [HttpPost("{poiId}")]
        public StatusCodeResult Post([FromBody] PoiBinding binding, string poiId)
        {
            binding.Id = poiId;
            _poiHandler.Create(binding);

            return new StatusCodeResult(StatusCodes.Status201Created);
        }
    }
}