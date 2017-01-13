using AnticevicApi.Model.Binding.Device;

namespace AnticevicApi.BL.Handlers.Device
{
    public interface IDeviceHandler
    {
        bool CreateBrowserLog(BrowserLogBinding binding);
    }
}
