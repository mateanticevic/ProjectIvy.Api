using System.ComponentModel.DataAnnotations;

namespace ProjectIvy.Model.Binding.Expense;

public class ExpenseSplitBinding : ExpenseSplitPartBinding
{
    [Required, MinLength(1)]
    public List<ExpenseSplitPartBinding> Expenses { get; set; }
}
