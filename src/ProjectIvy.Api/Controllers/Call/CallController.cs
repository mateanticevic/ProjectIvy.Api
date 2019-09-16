using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Call;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Call;

namespace ProjectIvy.Api.Controllers.Call
{
    public class CallController : BaseController<CallController>
    {
        private readonly ICallHandler _callHandler;

        public CallController(ILogger<CallController> logger, ICallHandler callHandler) : base(logger)
        {
            _callHandler = callHandler;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] FilteredPagedBinding binding) => Ok(_callHandler.Get(binding));

        [HttpPost]
        public IActionResult Post([FromBody] CallBinding binding) => Ok(_callHandler.Create(binding));
    }
}
