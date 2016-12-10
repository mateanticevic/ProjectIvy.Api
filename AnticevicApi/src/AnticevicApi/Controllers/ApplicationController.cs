using AnticevicApi.BL.Handlers.Application;
using AnticevicApi.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ApplicationController : BaseController
    {
        public ApplicationController(IOptions<AppSettings> options, IApplicationHandler applicationHandler) : base(options)
        {
            ApplicationHandler = applicationHandler;
        }

        [HttpGet]
        [Route("{valueId}/settings")]
        public Dictionary<string, object> GetSettings(string valueId)
        {
            return ApplicationHandler.GetSettings(valueId);
        }
    }
}
