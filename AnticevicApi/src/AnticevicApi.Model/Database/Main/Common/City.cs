using AnticevicApi.Model.Database.Main.Finance;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Common
{
    [Table(nameof(City), Schema = "Common")]
    public class City
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Vendor> Vendors { get; set; }
    }
}
