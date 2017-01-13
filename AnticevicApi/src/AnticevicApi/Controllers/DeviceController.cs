using AnticevicApi.BL.Handlers.Device;
using AnticevicApi.Model.Binding.Device;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AnticevicApi.Controllers
{
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
        public bool PutBrowserLog([FromBody] BrowserLogBinding binding, string deviceId)
        {
            binding.DeviceId = deviceId;

            return _deviceHandler.CreateBrowserLog(binding);
        }
    }
}
