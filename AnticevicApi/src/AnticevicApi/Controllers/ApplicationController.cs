using AnticevicApi.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ApplicationController : BaseController
    {
        public ApplicationController(IOptions<AppSettings> options) : base(options)
        {

        }

        [HttpGet]
        [Route("{valueId}/settings")]
        public Dictionary<string, object> GetSettings(string valueId)
        {
            return ApplicationHandler.GetSettings(valueId);
        }
    }
}
