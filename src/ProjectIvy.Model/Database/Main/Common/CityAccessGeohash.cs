using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common;

[Table(nameof(CityAccessGeohash), Schema = nameof(Common))]
public class CityAccessGeohash
{
    public City City { get; set; }

    public int CityId { get; set; }

    public string Geohash { get; set; }
}
