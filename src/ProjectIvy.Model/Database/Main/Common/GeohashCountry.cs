using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common
{
    [Table(nameof(GeohashCountry), Schema = nameof(Common))]
    public class GeohashCountry
	{
        public int CountryId { get; set; }

        public string Geohash { get; set; }

        public Country Country { get; set; }
    }
}

