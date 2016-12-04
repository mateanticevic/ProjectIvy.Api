using AnticevicApi.Model.View.Poi;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class PoiController : BaseController
    {
        [HttpGet]
        [Route("categories")]
        public IEnumerable<PoiCategory> GetCategories()
        {
            return PoiHandler.GetCategories();
        }

        [HttpGet]
        [Route("lists")]
        public IEnumerable<PoiList> GetLists()
        {
            return PoiHandler.GetLists();
        }
    }
}
