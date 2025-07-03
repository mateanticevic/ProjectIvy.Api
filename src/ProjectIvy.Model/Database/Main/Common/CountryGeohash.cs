using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common;

[Table(nameof(CountryGeohash), Schema = nameof(Common))]
public class CountryGeohash : IHasGeohash
{
    public int CountryId { get; set; }

    public string Geohash { get; set; }

    public Country Country { get; set; }
}
