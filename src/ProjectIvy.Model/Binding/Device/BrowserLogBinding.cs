using System;

namespace ProjectIvy.Model.Binding.Device;

public class BrowserLogBinding
{
    public string DeviceId { get; set; }

    public string Domain { get; set; }

    public DateTime End { get; set; }

    public bool IsSecured { get; set; }

    public DateTime Start { get; set; }
}
