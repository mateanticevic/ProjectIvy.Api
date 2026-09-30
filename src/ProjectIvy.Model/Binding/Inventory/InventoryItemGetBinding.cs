namespace ProjectIvy.Model.Binding.Inventory;

public class InventoryItemGetBinding : FilteredPagedBinding
{
    public IEnumerable<string> BrandId { get; set; }

    public InventoryItemSort OrderBy { get; set; } = InventoryItemSort.Name;

    public string Search { get; set; }
}
