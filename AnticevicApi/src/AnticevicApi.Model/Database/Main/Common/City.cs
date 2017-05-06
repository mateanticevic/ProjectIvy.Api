using AnticevicApi.Model.Database.Main.Finance;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AnticevicApi.Model.Database.Main.Common
{
    [Table(nameof(City), Schema = "Common")]
    public class City : IHasValueId, IHasName
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Vendor> Vendors { get; set; }
    }
}
