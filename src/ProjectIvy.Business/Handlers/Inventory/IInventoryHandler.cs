using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Inventory;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Inventory;

namespace ProjectIvy.Business.Handlers.Inventory;

public interface IInventoryHandler
{
    Task<string> CreateItem(InventoryItemBinding binding);

    Task<PagedView<View.InventoryItem>> GetItems(InventoryItemGetBinding binding);

    Task UpdateItem(string valueId, InventoryItemBinding binding);
}
