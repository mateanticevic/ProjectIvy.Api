using System.Threading.Tasks;
using ProjectIvy.Model.Binding.Inventory;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Inventory;

namespace ProjectIvy.Business.Handlers.Inventory;

public interface IInventoryHandler
{
    Task<string> CreateItem(InventoryItemBinding binding);

    Task<PagedView<View.InventoryItem>> GetItems(InventoryItemGetBinding binding);

    Task<IEnumerable<View.Ownership>> GetOwnerships();

    Task LinkItemToExpense(string itemValueId, string expenseValueId);

    Task UnlinkItemFromExpense(string itemValueId, string expenseValueId);

    Task UpdateItem(string valueId, InventoryItemBinding binding);
}
