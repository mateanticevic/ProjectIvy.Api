using System;
namespace ProjectIvy.Model.Binding.IotData
{
    public class IotDeviceDataBinding
    {
        public string DeviceId { get; set; }

        public string FieldIdentifier { get; set; }

        public string Value { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
