using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AnticevicApi.Model.Database.Main.Travel
{
    [Table(nameof(PoiCategory), Schema = nameof(Travel))]
    public class PoiCategory : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Poi> Pois { get; set; }
    }
}
