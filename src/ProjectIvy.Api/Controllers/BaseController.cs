using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Api.Controllers
{
    public abstract class BaseController<TController> : Controller
    {
        public BaseController(ILogger<TController> logger)
        {
            Logger = logger;
        }

        protected ILogger Logger { get; private set; } 
    }
}
