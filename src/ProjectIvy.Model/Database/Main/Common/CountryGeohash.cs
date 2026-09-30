using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common;

[Table(nameof(CountryGeohash), Schema = nameof(Common))]
public class CountryGeohash : IHasGeohash
{
    public Country Country { get; set; }

    public int CountryId { get; set; }

    public string Geohash { get; set; }
}
