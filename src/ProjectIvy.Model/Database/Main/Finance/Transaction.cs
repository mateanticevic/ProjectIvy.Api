using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(Transaction), Schema = nameof(Finance))]
public class Transaction
{
    public Account Account { get; set; }

    public int AccountId { get; set; }

    public decimal Amount { get; set; }

    public decimal? Balance { get; set; }

    public DateTime? Completed { get; set; }

    public DateTime Created { get; set; }

    public string Description { get; set; }

    [Key]
    public int Id { get; set; }

    public string Type { get; set; }
}
