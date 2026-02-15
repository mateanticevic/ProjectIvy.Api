using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Inventory;
using ProjectIvy.Model.View;
using Database = ProjectIvy.Model.Database.Main.Inventory;
using View = ProjectIvy.Model.View.Inventory;

namespace ProjectIvy.Business.Handlers.Inventory;

public class InventoryHandler : Handler<InventoryHandler>, IInventoryHandler
{
    public InventoryHandler(IHandlerContext<InventoryHandler> context) : base(context)
    {
    }

    public async Task<string> CreateItem(InventoryItemBinding binding)
    {
        using var context = GetMainContext();

        var entity = new Database.InventoryItem
        {
            Name = binding.Name,
            ValueId = binding.Name.ToValueId(),
            BrandId = context.Brands.GetId(binding.BrandId),
            UserId = UserId
        };

        await context.InventoryItems.AddAsync(entity);
        await context.SaveChangesAsync();

        return entity.ValueId;
    }

    public async Task<PagedView<View.InventoryItem>> GetItems(InventoryItemGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.InventoryItems
                           .WhereUser(UserId)
                           .Include(x => x.Brand)
                           .Where(binding);

        return await query.OrderBy(binding)
                          .Select(x => new View.InventoryItem(x))
                          .ToPagedViewAsync(binding);
    }

    public async Task LinkItemToExpense(string itemValueId, string expenseValueId)
    {
        using var context = GetMainContext();

        var item = await context.InventoryItems
                                .WhereUser(UserId)
                                .SingleOrDefaultAsync(x => x.ValueId == itemValueId);

        if (item == null)
            throw new Exceptions.ResourceNotFoundException();

        var expense = await context.Expenses
                                   .WhereUser(UserId)
                                   .SingleOrDefaultAsync(x => x.ValueId == expenseValueId);

        if (expense == null)
            throw new Exceptions.ResourceNotFoundException();

        var entity = new Database.InventoryItemExpense
        {
            InventoryItemId = item.Id,
            ExpenseId = expense.Id
        };

        await context.InventoryItemExpenses.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateItem(string valueId, InventoryItemBinding binding)
    {
        using var context = GetMainContext();

        var item = await context.InventoryItems
                                 .WhereUser(UserId)
                                 .SingleOrDefaultAsync(x => x.ValueId == valueId);

        if (item == null)
            throw new Exceptions.ResourceNotFoundException();

        item.Name = binding.Name;
        item.BrandId = context.Brands.GetId(binding.BrandId);

        context.InventoryItems.Update(item);
        await context.SaveChangesAsync();
    }
}
