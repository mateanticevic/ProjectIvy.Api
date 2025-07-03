using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table("IncomeType", Schema = "Finance")]
public class IncomeType : IHasValueId
{
    [Key]
    public int Id { get; set; }

    public string ValueId { get; set; }

    public string Name { get; set; }
}
