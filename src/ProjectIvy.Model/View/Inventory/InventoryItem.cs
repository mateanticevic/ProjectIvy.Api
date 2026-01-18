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
        Brand = x.Brand != null ? new BrandView(x.Brand) : null;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public BrandView Brand { get; set; }
}
