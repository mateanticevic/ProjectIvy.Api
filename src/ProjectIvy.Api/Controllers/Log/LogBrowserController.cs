using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Device;
using ProjectIvy.Model.Binding.Log;
using ProjectIvy.Model.View;
using ProjectIvy.Model.View.Device;

namespace ProjectIvy.Api.Controllers.Log
{
    [Route("Log/Browser")]
    public class LogBrowserController : BaseController<LogBrowserController>
    {
        private readonly IDeviceHandler _devicehandler;

        public LogBrowserController(ILogger<LogBrowserController> logger, IDeviceHandler devicehandler) : base(logger) => _devicehandler = devicehandler;

        [HttpGet]
        public PagedView<BrowserLog> Get([FromQuery] LogBrowserGetBinding binding) => _devicehandler.Get(binding);
    }
}
