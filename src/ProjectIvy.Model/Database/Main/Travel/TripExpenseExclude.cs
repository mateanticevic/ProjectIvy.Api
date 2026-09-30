using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(TripExpenseExclude), Schema = nameof(Travel))]
public class TripExpenseExclude
{
    public Finance.Expense Expense { get; set; }

    public int ExpenseId { get; set; }

    public Trip Trip { get; set; }

    public int TripId { get; set; }
}
