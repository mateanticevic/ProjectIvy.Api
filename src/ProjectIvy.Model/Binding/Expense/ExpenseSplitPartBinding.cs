using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Binding.Expense;

public class ExpenseSplitPartBinding
{
    [Required]
    public decimal? Amount { get; set; }

    public string ExpenseTypeId { get; set; }

    public string Comment { get; set; }
}
