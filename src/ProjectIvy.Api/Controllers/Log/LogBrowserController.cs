using ProjectIvy.BL.Handlers.Device;
using ProjectIvy.Model.Binding.Log;
using ProjectIvy.Model.Constants.Database;
using ProjectIvy.Model.View.Device;
using ProjectIvy.Model.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProjectIvy.Api.Controllers.Income
{
    [Authorize(Roles = UserRole.User)]
    [Route("log/browser")]
    public class LogBrowserController : BaseController<LogBrowserController>
    {
        private IDeviceHandler _devicehandler;

        public LogBrowserController(ILogger<LogBrowserController> logger, IDeviceHandler devicehandler) : base(logger)
        {
            _devicehandler = devicehandler;
        }
        
        [HttpGet]
        public PagedView<BrowserLog> Get([FromQuery] LogBrowserGetBinding binding)
        {
            return _devicehandler.Get(binding);
        }
    }
}
