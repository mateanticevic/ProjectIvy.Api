using AnticevicApi.DL.DbContexts;
using AnticevicApi.Model.View.Vendor;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class VendorHandler : Handler
    {
        public VendorHandler(string connectionString, int userId) : base(connectionString, userId)
        {

        }

        public IEnumerable<Vendor> Get(string contains)
        {
            using (var db = new MainContext(ConnectionString))
            {
                var vendors = db.Vendors.OrderBy(x => x.Name)
                                        .AsEnumerable();

                vendors = string.IsNullOrEmpty(contains) ? vendors : vendors.Where(x => x.Name.Contains(contains) || x.ValueId.Contains(contains));

                return vendors.ToList()
                              .Select(x => new Vendor(x));
            }
        }
    }
}
