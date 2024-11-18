using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Vendor;
using ProjectIvy.Model.View;
using System.Linq;
using View = ProjectIvy.Model.View.Vendor;

namespace ProjectIvy.Business.Handlers.Vendor
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
            string searchPattern = $"%{binding.Search}%";

            using (var context = GetMainContext())
            {
                return context.Vendors.WhereIf(binding?.Search != null,
                        x => EF.Functions.Like(x.ValueId, searchPattern) ||
                             EF.Functions.Like(x.Name, searchPattern))
                              .OrderByDescending(x => x.ValueId == binding.Search)
                              .ThenBy(x => x.Name)
                              .Select(x => new View.Vendor(x))
                              .ToPagedView(binding);
            }
        }
    }
}
