using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Api.Controllers
{
    [Route("[controller]")]
    public abstract class BaseController<TController> : Controller
    {
        protected BaseController(ILogger<TController> logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; }
    }
}
