using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Inventory;

[Table(nameof(InventoryItemExpense), Schema = nameof(Inventory))]
public class InventoryItemExpense
{
    public long InventoryItemId { get; set; }

    public int ExpenseId { get; set; }

    public InventoryItem InventoryItem { get; set; }

    public Finance.Expense Expense { get; set; }
}
