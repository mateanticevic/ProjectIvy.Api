using System;

namespace AnticevicApi.Model.Binding.Device
{
    public class BrowserLogBinding
    {
        public bool IsSecured { get; set; }
        public DateTime End { get; set; }
        public DateTime Start { get; set; }
        public string DeviceId { get; set; }
        public string Domain { get; set; }
    }
}
