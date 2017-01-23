using AnticevicApi.Model.Binding.Device;

namespace AnticevicApi.BL.Handlers.Device
{
    public interface IDeviceHandler
    {
        void CreateBrowserLog(BrowserLogBinding binding);
    }
}
