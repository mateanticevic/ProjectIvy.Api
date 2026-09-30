using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Tracking;

[Table(nameof(LocationGeohash), Schema = nameof(Tracking))]
public class LocationGeohash : IHasGeohash
{
    public string Geohash { get; set; }

    [Key]
    public long Id { get; set; }

    public Location Location { get; set; }

    public int LocationId { get; set; }
}
