using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common
{
    [Table(nameof(GeohashCity), Schema = nameof(Common))]
    public class GeohashCity
	{
        public int CityId { get; set; }

        public string Geohash { get; set; }

        public City City { get; set; }
    }
}

