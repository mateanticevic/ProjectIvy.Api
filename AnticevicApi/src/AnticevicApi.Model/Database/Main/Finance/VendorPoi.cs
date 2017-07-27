using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Finance
{
    [Table(nameof(VendorPoi), Schema = nameof(Finance))]
    public class VendorPoi
    {
        public int VendorId { get; set; }

        public int PoiId { get; set; }

        public Vendor Vendor { get; set; }

        public Travel.Poi Poi { get; set; }
    }
}
