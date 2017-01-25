using AnticevicApi.BL.Handlers.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AnticevicApi.Controllers.Application
{
    [Route("[controller]")]
    public class ApplicationController : BaseController<ApplicationController>
    {
        private readonly IApplicationHandler _applicationHandler;

        public ApplicationController(ILogger<ApplicationController> logger, IApplicationHandler applicationHandler) : base(logger)
        {
            _applicationHandler = applicationHandler;
        }

        [HttpGet]
        [Route("{valueId}/settings")]
        public Dictionary<string, object> GetSettings(string valueId)
        {
            return _applicationHandler.GetSettings(valueId);
        }
    }
}
