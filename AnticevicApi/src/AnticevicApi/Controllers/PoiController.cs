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
        public IEnumerable<PoiCategory> Get()
        {
            return PoiHandler.GetCategories();
        }
    }
}
