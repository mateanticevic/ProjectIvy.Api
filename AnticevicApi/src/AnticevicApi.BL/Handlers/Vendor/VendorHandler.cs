using AnticevicApi.DL.DbContexts;
using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.Vendor;

namespace AnticevicApi.BL.Handlers.Vendor
{
    public class VendorHandler : Handler, IVendorHandler
    {
        public IEnumerable<View.Vendor> Get(string contains)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var vendors = db.Vendors.OrderBy(x => x.Name)
                                        .AsEnumerable();

                vendors = string.IsNullOrEmpty(contains) ? vendors : vendors.Where(x => x.Name.Contains(contains) || x.ValueId.Contains(contains));

                return vendors.ToList()
                              .Select(x => new View.Vendor(x));
            }
        }
    }
}
