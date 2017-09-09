using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Api.Controllers.Version
{
    [Route("[controller]")]
    public class VersionController : BaseController<VersionController>
    {
        public VersionController(ILogger<VersionController> logger) : base(logger)
        {
        }

        [HttpGet]
        public string Get()
        {
            return Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;
        }
    }
}
