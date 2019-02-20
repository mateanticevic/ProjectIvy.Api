using System.Linq;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Vendor;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Vendor;

namespace ProjectIvy.BL.Handlers.Vendor
{
    public class VendorHandler : Handler<VendorHandler>, IVendorHandler
    {
        public VendorHandler(IHandlerContext<VendorHandler> context) : base(context)
        {
        }

        public View.Vendor Get(string id)
        {
            using (var context = GetMainContext())
            {
                return context.Vendors.SingleOrDefault(x => x.ValueId == id).ConvertTo(x => new View.Vendor(x));
            }
        }

        public PagedView<View.Vendor> Get(VendorGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Vendors.WhereIf(binding?.Search != null,
                        x => x.ValueId.ToLowerInvariant().Contains(binding.Search.ToLowerInvariant()) ||
                             x.Name.ToLowerInvariant().Contains(binding.Search.ToLowerInvariant()))
                              .Select(x => new View.Vendor(x))
                              .OrderBy(x => x.Name)
                              .ToPagedView(binding);
            }
        }
    }
}
