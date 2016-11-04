using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnticevicApi.Model.Database.Main.Finance
{
    [Table("IncomeSource", Schema = "Finance")]
    public class IncomeSource : UserEntity
    {
        [Key]
        public int Id { get; set; }

        public string ValueId { get; set; }

        public string Name { get; set; }

        public ICollection<Income> Incomes { get; set; }
    }
}
