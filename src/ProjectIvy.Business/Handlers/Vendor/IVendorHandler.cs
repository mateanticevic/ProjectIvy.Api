using ProjectIvy.Model.Binding.Vendor;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Vendor;

namespace ProjectIvy.Business.Handlers.Vendor
{
    public interface IVendorHandler : IHandler
    {
        View.Vendor Get(string id);

        PagedView<View.Vendor> Get(VendorGetBinding binding);
    }
}
