﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.IotDevice;
using ProjectIvy.Model.Binding.IotData;
using ProjectIvy.Model.Constants.Database;

namespace ProjectIvy.Api.Controllers.Device
{
    [Authorize(Roles = UserRole.User)]
    public class IotDeviceController : BaseController<IotDeviceController>
    {
        private readonly IIotDeviceHandler _iotDeviceHandler;

        public IotDeviceController(ILogger<IotDeviceController> logger, IIotDeviceHandler iotDeviceHandler) : base(logger) => _iotDeviceHandler = iotDeviceHandler;

        [HttpPost("{deviceId}/Ping")]
        public async Task PutLastPing(string deviceId) => await _iotDeviceHandler.Ping(deviceId);

        [HttpPost("{deviceId}/Data/{fieldIdentifier}")]
        public async Task PostData([FromBody] IotDeviceDataBinding b, string deviceId, string fieldIdentifier)
        {
            b.DeviceId = deviceId;
            b.FieldIdentifier = fieldIdentifier;

            await _iotDeviceHandler.PushData(b);
        }

        [HttpPost("{deviceId}/Data/{fieldIdentifier}/{value}")]
        public async Task PostDataValue(string deviceId, string fieldIdentifier, string value)
        {
            var b = new IotDeviceDataBinding()
            {
                DeviceId = deviceId,
                FieldIdentifier = fieldIdentifier,
                Value = value
            };

            await _iotDeviceHandler.PushData(b);
        }

        [HttpGet("{deviceId}/Data/{fieldIdentifier}")]
        public async Task<IActionResult> GetDeviceData(string deviceId, string fieldIdentifier, [FromQuery] IotDeviceDataGetBinding binding)
            => Ok(await _iotDeviceHandler.GetData(deviceId, fieldIdentifier, binding));
    }
}