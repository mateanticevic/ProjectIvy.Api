using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(IncomeTransaction), Schema = nameof(Finance))]
public class IncomeTransaction
{
    public Income Income { get; set; }

    public int? IncomeId { get; set; }

    public Transaction Transaction { get; set; }

    public int? TransactionId { get; set; }
}
