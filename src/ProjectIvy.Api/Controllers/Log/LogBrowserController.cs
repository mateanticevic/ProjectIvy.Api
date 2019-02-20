using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Device;
using ProjectIvy.Model.Binding.Log;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View.Device;
using ProjectIvy.Model.View;

namespace ProjectIvy.Api.Controllers.Log
{
    [Authorize(Roles = UserRole.User)]
    [Route("Log/Browser")]
    public class LogBrowserController : BaseController<LogBrowserController>
    {
        private readonly IDeviceHandler _devicehandler;

        public LogBrowserController(ILogger<LogBrowserController> logger, IDeviceHandler devicehandler) : base(logger) => _devicehandler = devicehandler;

        [HttpGet]
        public PagedView<BrowserLog> Get([FromQuery] LogBrowserGetBinding binding) => _devicehandler.Get(binding);
    }
}
