using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectIvy.Business.Handlers.Inventory;
using ProjectIvy.Model.Binding.Inventory;
using ProjectIvy.Model.View;
using View = ProjectIvy.Model.View.Inventory;

namespace ProjectIvy.Api.Controllers.Inventory;

public class InventoryController : BaseController<InventoryController>
{
    private readonly IInventoryHandler _inventoryHandler;

    public InventoryController(ILogger<InventoryController> logger, IInventoryHandler inventoryHandler) : base(logger)
        => _inventoryHandler = inventoryHandler;

    [HttpGet("Item")]
    public async Task<PagedView<View.InventoryItem>> GetItems(InventoryItemGetBinding binding)
        => await _inventoryHandler.GetItems(binding);

    [HttpPost("Item")]
    public async Task<StatusCodeResult> PostItem([FromBody] InventoryItemBinding binding)
    {
        await _inventoryHandler.CreateItem(binding);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPut("Item/{valueId}")]
    public async Task UpdateItem(string valueId, [FromBody] InventoryItemBinding binding)
        => await _inventoryHandler.UpdateItem(valueId, binding);

    [HttpPost("item/{itemValueId}/Expense/{expenseValueId}")]
    public async Task<StatusCodeResult> PostItemExpense(string itemValueId, string expenseValueId)
    {
        await _inventoryHandler.LinkItemToExpense(itemValueId, expenseValueId);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpDelete("item/{itemValueId}/Expense/{expenseValueId}")]
    public async Task<StatusCodeResult> DeleteItemExpense(string itemValueId, string expenseValueId)
    {
        await _inventoryHandler.UnlinkItemFromExpense(itemValueId, expenseValueId);

        return new StatusCodeResult(StatusCodes.Status204NoContent);
    }
}
