using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Inventory;

[Table(nameof(InventoryItemOwnership), Schema = nameof(Inventory))]
public class InventoryItemOwnership
{
    public DateTime Created { get; set; }

    [Key]
    public long Id { get; set; }

    public InventoryItem InventoryItem { get; set; }

    public long InventoryItemId { get; set; }

    public Ownership Ownership { get; set; }

    public int OwnershipId { get; set; }
}
