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

        [HttpGet]
        [Route("categories")]
        public IEnumerable<View.PoiCategory> GetCategories()
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetCategories));

            return _poiHandler.GetCategories();
        }

        [HttpGet]
        [Route("lists")]
        public IEnumerable<View.PoiList> GetLists()
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetLists));

            return _poiHandler.GetLists();
        }

        [HttpGet]
        [Route("list/{listValueId}/pois")]
        public IEnumerable<View.Poi> GetPois(string listValueId)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetPois), listValueId);

            return _poiHandler.GetByList(listValueId);
        }

        #endregion
    }
}
