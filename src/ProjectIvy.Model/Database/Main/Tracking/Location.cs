using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Tracking;

[Table(nameof(Location), Schema = nameof(Tracking))]
public class Location : UserEntity, IHasValueId, IHasName
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public int LocationTypeId { get; set; }

    public bool CanContainOtherLocations { get; set; }

    public LocationType LocationType { get; set; }

    public ICollection<LocationGeohash> Geohashes { get; set; }
}
