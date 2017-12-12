using DatabaseModel = ProjectIvy.Model.Database.Main;
using ProjectIvy.Common.Extensions;

namespace ProjectIvy.Model.View.Vendor
{
    public class VendorCount
    {
        public VendorCount(DatabaseModel.Finance.Vendor v, int count)
        {
            Count = count;
            Vendor = v.ConvertTo(x => new Vendor(x));
        }

        public int Count { get; set; }

        public Vendor Vendor { get; set; }
    }
}
