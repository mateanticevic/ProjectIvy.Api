using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Database.Main.Common
{
    [Table(nameof(Country), Schema = "Common")]
    public class Country : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<City> Cities { get; set; }
    }
}
