using System;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.IotData;
using ProjectIvy.Model.Database.Main.Iot;

namespace ProjectIvy.Business.MapExtensions
{
    public static class IotDeviceExtensions
    {
        public static DeviceData ToEntity(this IotDeviceDataBinding binding, MainContext context, DeviceData d = null)
        {
            var entity = d ?? new DeviceData();

            entity.Created = binding.Timestamp ?? DateTime.Now;
            entity.DeviceId = context.IotDevices.GetId(binding.DeviceId).Value;
            entity.FieldIdentifier = binding.FieldIdentifier;
            entity.Value = binding.Value;

            return entity;
        }
    }
}
