using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Common
{
    [Table(nameof(CountryPolygon), Schema = nameof(Common))]
    public class CountryPolygon
    {
        public int CountryId { get; set; }

        public int GroupId { get; set; }

        public int Index { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public Country Country { get; set; }
    }
}
