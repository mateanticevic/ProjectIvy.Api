using System.Linq;
using BrandView = ProjectIvy.Model.View.Brand.Brand;
using DatabaseModel = ProjectIvy.Model.Database.Main.Inventory;

namespace ProjectIvy.Model.View.Inventory;

public class InventoryItem
{
    public InventoryItem() { }

    public InventoryItem(DatabaseModel.InventoryItem x)
    {
        var latestOwnership = x.InventoryItemOwnerships?
            .OrderByDescending(o => o.Created)
            .FirstOrDefault();

        Id = x.ValueId;
        Name = x.Name;
        Brand = x.Brand != null ? new (x.Brand) : null;
        Acquired = x.InventoryItemOwnerships?
            .OrderBy(o => o.Created)
            .Select(o => (DateTime?)o.Created)
            .FirstOrDefault();
        Ownership = latestOwnership?.Ownership != null
            ? new Ownership(latestOwnership.Ownership)
            : null;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public BrandView Brand { get; set; }

    public DateTime? Acquired { get; set; }

    public Ownership Ownership { get; set; }
}
