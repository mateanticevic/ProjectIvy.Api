using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Device;
using ProjectIvy.Model.Binding.Device;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Device
{
    [Authorize(Roles = UserRole.User)]
    public class DeviceController : BaseController<DeviceController>
    {
        private readonly IDeviceHandler _deviceHandler;

        public DeviceController(ILogger<DeviceController> logger, IDeviceHandler deviceHandler) : base(logger) => _deviceHandler = deviceHandler;

        [HttpPut("{deviceId}/browserLog")]
        public StatusCodeResult PutBrowserLog([FromBody] BrowserLogBinding binding, string deviceId)
        {
            binding.DeviceId = deviceId;
            _deviceHandler.CreateBrowserLog(binding);
            return new StatusCodeResult(StatusCodes.Status201Created);
        }
    }
}
