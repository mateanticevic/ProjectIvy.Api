using AnticevicApi.BL.Handlers.Device;
using AnticevicApi.Model.Binding.Log;
using AnticevicApi.Model.Constants.Database;
using AnticevicApi.Model.View.Device;
using AnticevicApi.Model.View;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnticevicApi.Controllers.Income
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
        public PaginatedView<BrowserLog> Get([FromQuery] LogBrowserGetBinding binding)
        {
            return _devicehandler.Get(binding);
        }
    }
}
