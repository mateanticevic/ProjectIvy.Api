using AnticevicApi.Model.Binding.Device;
using AnticevicApi.Model.Binding.Log;
using AnticevicApi.Model.View;
using View = AnticevicApi.Model.View;

namespace AnticevicApi.BL.Handlers.Device
{
    public interface IDeviceHandler
    {
        void CreateBrowserLog(BrowserLogBinding binding);

        PaginatedView<View.Device.BrowserLog> Get(LogBrowserGetBinding binding);
    }
}
