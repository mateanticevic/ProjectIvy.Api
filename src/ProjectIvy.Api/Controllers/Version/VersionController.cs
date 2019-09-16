using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Api.Controllers.Version
{
    public class VersionController : BaseController<VersionController>
    {
        public VersionController(ILogger<VersionController> logger) : base(logger)
        {
        }

        [HttpGet]
        public string Get() => Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;
    }
}
