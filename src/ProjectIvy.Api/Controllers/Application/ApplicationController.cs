using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.BL.Handlers.Application;
using System.Collections.Generic;

namespace ProjectIvy.Api.Controllers.Application
{
    [Route("[controller]")]
    public class ApplicationController : BaseController<ApplicationController>
    {
        private readonly IApplicationHandler _applicationHandler;

        public ApplicationController(ILogger<ApplicationController> logger, IApplicationHandler applicationHandler) : base(logger)
        {
            _applicationHandler = applicationHandler;
        }

        [HttpGet("{valueId}/Settings")]
        public Dictionary<string, object> GetSettings(string valueId)
        {
            return _applicationHandler.GetSettings(valueId);
        }
    }
}
