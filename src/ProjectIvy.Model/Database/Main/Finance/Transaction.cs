using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Transaction), Schema = nameof(Finance))]
public class Transaction
{
    [Key]
    public int Id { get; set; }

    public int AccountId { get; set; }

    public decimal Amount { get; set; }

    public decimal? Balance { get; set; }

    public string Description { get; set; }

    public string Type { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Completed { get; set; }

    public Account Account { get; set; }
}
