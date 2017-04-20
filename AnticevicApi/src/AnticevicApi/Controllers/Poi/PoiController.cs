using AnticevicApi.BL.Handlers.Poi;
using AnticevicApi.Model.Constants;
using View = AnticevicApi.Model.View.Poi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AnticevicApi.Controllers.Poi
{
    [Route("[controller]")]
    public class PoiController : BaseController<PoiController>
    {
        private readonly IPoiHandler _poiHandler;

        public PoiController(ILogger<PoiController> logger, IPoiHandler poiHandler) : base(logger)
        {
            _poiHandler = poiHandler;
        }

        #region Get


        #endregion
    }
}