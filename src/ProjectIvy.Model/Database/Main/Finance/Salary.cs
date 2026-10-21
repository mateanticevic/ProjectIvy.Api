using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectIvy.Model.Database.Main.Common;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Salary), Schema = nameof(Finance))]
public class Salary : UserEntity, IHasValueId
{
    public Company Company { get; set; }

    public int? CompanyId { get; set; }

    public Currency Currency { get; set; }

    public int CurrencyId { get; set; }

    [Column(TypeName = "datetime2(0)")]
    public DateTime Date { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal? Deductions { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Gross { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal GrossTotal { get; set; }

    [Key]
    public int Id { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Medical { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Net { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal NetTotal { get; set; }

    [Column(TypeName = "varchar(1000)")]
    [MaxLength(1000)]
    public string Note { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Tax { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal TravelExpenses { get; set; }

    public string ValueId { get; set; }
}
