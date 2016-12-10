using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Finance
{
    [Table("Vendor", Schema = "Finance")]
    public class Vendor : IHasValueId
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Expense> Expenses { get; set; }
    }
}
