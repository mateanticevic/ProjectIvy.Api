using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Tracking
{
    [Table("PoiCategory", Schema = "Tracking")]
    public class PoiCategory : IHasValueId
    {
        [Key]
        public int Id { get; set; }
        public string ValueId { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
