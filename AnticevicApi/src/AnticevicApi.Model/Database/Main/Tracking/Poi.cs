using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnticevicApi.Model.Database.Main.Tracking
{
    [Table("Poi", Schema = "Tracking")]
    public class Poi : IHasValueId
    {
        [Key]
        public int Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int PoiCategoryId { get; set; }
        public int PoiListId { get; set; }
        public string Name { get; set; }
        public string ValueId { get; set; }

        public PoiCategory PoiCategory { get; set; }
        public PoiList PoiList { get; set; }
    }
}
