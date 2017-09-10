using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Poi;
using System.Collections.Generic;
using View = ProjectIvy.Model.View.Poi;

namespace ProjectIvy.Api.Controllers.Poi
{
    [Route("[controller]")]
    public class CommonController : BaseController<CommonController>
    {
        private readonly IPoiHandler _poiHandler;

        public CommonController(ILogger<CommonController> logger, IPoiHandler poiHandler) : base(logger)
        {
            _poiHandler = poiHandler;
        }

        [HttpGet]
        [Route("poiCategory")]
        public IEnumerable<View.PoiCategory> GetPoiCategories()
        {
            return _poiHandler.GetCategories();
        }
    }
}