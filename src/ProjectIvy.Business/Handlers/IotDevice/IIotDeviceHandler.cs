﻿using System.Threading.Tasks;
using ProjectIvy.Model.Binding.IotData;

namespace ProjectIvy.Business.Handlers.IotDevice
{
    public interface IIotDeviceHandler
    {
        Task Ping(string deviceId);

        Task PushData(IotDeviceDataBinding b);
    }
}