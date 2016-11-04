using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ApplicationController : BaseController
    {
        [HttpGet]
        [Route("{valueId}/settings")]
        public Dictionary<string, object> GetSettings(string valueId)
        {
            return ApplicationHandler.GetSettings(valueId);
        }
    }
}
