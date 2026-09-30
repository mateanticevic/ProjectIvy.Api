using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Inventory;

[Table(nameof(InventoryItem), Schema = nameof(Inventory))]
public class InventoryItem : UserEntity, IHasName
{
    public Common.Brand Brand { get; set; }

    public int? BrandId { get; set; }

    [Key]
    public long Id { get; set; }

    public ICollection<InventoryItemExpense> InventoryItemExpenses { get; set; }

    public ICollection<InventoryItemOwnership> InventoryItemOwnerships { get; set; }

    public string Name { get; set; }

    public string ValueId { get; set; }
}
