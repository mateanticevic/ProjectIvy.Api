using System.Linq;
using BrandView = ProjectIvy.Model.View.Brand.Brand;
using DatabaseModel = ProjectIvy.Model.Database.Main.Inventory;

namespace ProjectIvy.Model.View.Inventory;

public class InventoryItem
{
    public InventoryItem() { }

    public InventoryItem(DatabaseModel.InventoryItem x)
    {
        Id = x.ValueId;
        Name = x.Name;
        Brand = x.Brand != null ? new (x.Brand) : null;
        Ownership = x.InventoryItemOwnerships?
            .OrderByDescending(o => o.Created)
            .FirstOrDefault()?.Ownership != null
            ? new Ownership(x.InventoryItemOwnerships.OrderByDescending(o => o.Created).First().Ownership)
            : null;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public BrandView Brand { get; set; }

    public Ownership Ownership { get; set; }
}
