using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common
{
    [Table(nameof(CityGeohash), Schema = nameof(Common))]
    public class CityGeohash : IHasGeohash
	{
        public int CityId { get; set; }

        public string Geohash { get; set; }

        public City City { get; set; }
    }
}

