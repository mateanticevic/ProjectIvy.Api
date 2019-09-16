using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

        protected async Task<OkResult> Ok(Task task)
        {
            await task;
            return Ok();
        }
    }
}
