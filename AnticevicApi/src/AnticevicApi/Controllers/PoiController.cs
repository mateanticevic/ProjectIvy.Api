using AnticevicApi.BL.Handlers.Poi;
using AnticevicApi.Config;
using AnticevicApi.Model.Constants;
using AnticevicApi.Model.View.Poi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class PoiController : BaseController<PoiController>
    {
        public PoiController(IOptions<AppSettings> options, ILogger<PoiController> logger, IPoiHandler poiHandler) : base(options, logger)
        {
            PoiHandler = poiHandler;
        }

        #region Get

        [HttpGet]
        [Route("categories")]
        public IEnumerable<PoiCategory> GetCategories()
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetCategories));

            return PoiHandler.GetCategories();
        }

        [HttpGet]
        [Route("lists")]
        public IEnumerable<PoiList> GetLists()
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetLists));

            return PoiHandler.GetLists();
        }

        [HttpGet]
        [Route("list/{listValueId}/pois")]
        public IEnumerable<Poi> GetPois(string listValueId)
        {
            Logger.LogInformation((int)LogEvent.ActionCalled, nameof(GetPois), listValueId);

            return PoiHandler.GetByList(listValueId);
        }

        #endregion
    }
}
