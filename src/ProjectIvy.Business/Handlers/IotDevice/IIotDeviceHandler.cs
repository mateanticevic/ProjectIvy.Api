using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectIvy.Model.Binding.IotData;

namespace ProjectIvy.Business.Handlers.IotDevice
{
    public interface IIotDeviceHandler
    {
        Task<IEnumerable<KeyValuePair<DateTime, string>>> GetData(string deviceId, string fieldIdentifier, IotDeviceDataGetBinding binding);

        Task Ping(string deviceId);

        Task PushData(IotDeviceDataBinding b);
    }
}