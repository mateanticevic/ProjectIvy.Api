using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Finance;

[Table(nameof(ExpenseTransaction), Schema = nameof(Finance))]
public class ExpenseTransaction
{
    public Expense Expense { get; set; }

    public int ExpenseId { get; set; }

    public Transaction Transaction { get; set; }

    public int TransactionId { get; set; }
}
