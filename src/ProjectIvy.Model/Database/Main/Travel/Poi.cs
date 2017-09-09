using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Database.Main.Travel
{
    [Table(nameof(Poi), Schema = nameof(Travel))]
    public class Poi : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public int PoiCategoryId { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public PoiCategory PoiCategory { get; set; }
    }
}
