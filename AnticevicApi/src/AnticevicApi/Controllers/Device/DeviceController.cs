using AnticevicApi.BL.Handlers.Device;
using AnticevicApi.Model.Binding.Device;
using AnticevicApi.Model.Constants.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnticevicApi.Controllers.Device
{
    [Authorize(Roles = UserRole.User)]
    [Route("[controller]")]
    public class DeviceController : BaseController<DeviceController>
    {
        private readonly IDeviceHandler _deviceHandler;

        public DeviceController(ILogger<DeviceController> logger, IDeviceHandler deviceHandler) : base(logger)
        {
            _deviceHandler = deviceHandler;
        }

        [HttpPut]
        [Route("{deviceId}/browserLog")]
        public StatusCodeResult PutBrowserLog([FromBody] BrowserLogBinding binding, string deviceId)
        {
            binding.DeviceId = deviceId;
            _deviceHandler.CreateBrowserLog(binding);
            return new StatusCodeResult(StatusCodes.Status201Created);
        }
    }
}
