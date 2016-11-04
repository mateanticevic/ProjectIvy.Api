using AnticevicApi.DL.DbContexts;
using System.Linq;

namespace AnticevicApi.DL.Helpers
{
    public class VendorHelper
    {
        public static int? GetId(string valueId)
        {
            using (var db = new MainContext())
            {
                return db.Vendors.SingleOrDefault(x => x.ValueId == valueId)?
                                 .Id;
            }
        }
    }
}
