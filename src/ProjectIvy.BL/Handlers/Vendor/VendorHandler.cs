using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using View = ProjectIvy.Model.View.Vendor;

namespace ProjectIvy.BL.Handlers.Vendor
{
    public class VendorHandler : Handler<VendorHandler>, IVendorHandler
    {
        public VendorHandler(IHandlerContext<VendorHandler> context) : base(context)
        {
        }

        public IEnumerable<View.Vendor> Get(string contains)
        {
            using (var db = GetMainContext())
            {
                var vendors = db.Vendors.Include(x => x.City)
                                        .OrderBy(x => x.Name)
                                        .AsEnumerable();

                vendors = string.IsNullOrEmpty(contains) ? vendors : vendors.Where(x => x.Name.Contains(contains) || x.ValueId.Contains(contains));

                return vendors.ToList()
                              .Select(x => new View.Vendor(x));
            }
        }
    }
}
