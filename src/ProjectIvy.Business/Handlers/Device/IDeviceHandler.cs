using ProjectIvy.Model.Binding.Device;
using ProjectIvy.Model.Binding.Log;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Device
{
    public interface IDeviceHandler
    {
        void CreateBrowserLog(BrowserLogBinding binding);

        PagedView<View.Device.BrowserLog> Get(LogBrowserGetBinding binding);
    }
}
