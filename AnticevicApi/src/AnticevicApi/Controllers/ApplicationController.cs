using AnticevicApi.BL.Handlers.Application;
using AnticevicApi.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AnticevicApi.Controllers
{
    [Route("[controller]")]
    public class ApplicationController : BaseController<AirportController>
    {
        public ApplicationController(IOptions<AppSettings> options, ILogger<AirportController> logger, IApplicationHandler applicationHandler) : base(options, logger)
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
