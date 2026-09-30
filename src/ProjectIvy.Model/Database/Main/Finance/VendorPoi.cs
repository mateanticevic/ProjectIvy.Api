using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(VendorPoi), Schema = nameof(Finance))]
public class VendorPoi
{
    public Travel.Poi Poi { get; set; }

    public int PoiId { get; set; }

    public Vendor Vendor { get; set; }

    public int VendorId { get; set; }
}
