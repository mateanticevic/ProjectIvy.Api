using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Inventory;

[Table(nameof(InventoryItemOwnership), Schema = nameof(Inventory))]
public class InventoryItemOwnership
{
    [Key]
    public long Id { get; set; }

    public long InventoryItemId { get; set; }

    public int OwnershipId { get; set; }

    public DateTime Created { get; set; }

    public InventoryItem InventoryItem { get; set; }

    public Ownership Ownership { get; set; }
}
