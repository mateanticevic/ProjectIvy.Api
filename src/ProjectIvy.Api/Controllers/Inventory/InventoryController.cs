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

    [HttpGet("item")]
    public async Task<PagedView<View.InventoryItem>> GetItems(InventoryItemGetBinding binding)
        => await _inventoryHandler.GetItems(binding);

    [HttpPost("item")]
    public async Task<StatusCodeResult> CreateItem([FromBody] InventoryItemBinding binding)
    {
        await _inventoryHandler.CreateItem(binding);

        return new StatusCodeResult(StatusCodes.Status201Created);
    }

    [HttpPut("item/{valueId}")]
    public async Task UpdateItem(string valueId, [FromBody] InventoryItemBinding binding)
        => await _inventoryHandler.UpdateItem(valueId, binding);
}
