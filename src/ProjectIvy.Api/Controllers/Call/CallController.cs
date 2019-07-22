using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Call;

namespace ProjectIvy.Api.Controllers.Call
{
    [Route("[controller]")]
    public class CallController : BaseController<CallController>
    {
        private readonly ICallHandler _callHandler;

        public CallController(ILogger<CallController> logger, ICallHandler callHandler) : base(logger)
        {
            _callHandler = callHandler;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_callHandler.Get());
    }
}
