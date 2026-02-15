using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Inventory;
using ProjectIvy.Model.View;
using Database = ProjectIvy.Model.Database.Main.Inventory;
using View = ProjectIvy.Model.View.Inventory;

namespace ProjectIvy.Business.Handlers.Inventory;

public class InventoryHandler : Handler<InventoryHandler>, IInventoryHandler
{
    private const string DefaultOwnershipValueId = "owned";

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

        var itemOwnership = new Database.InventoryItemOwnership
        {
            OwnershipId = context.Ownerships.GetId(binding.OwershipId) ?? context.Ownerships.GetId(DefaultOwnershipValueId).Value,
            Created = DateTime.UtcNow
        };

        await context.InventoryItems.AddAsync(entity);
        await context.InventoryItemOwnerships.AddAsync(itemOwnership);
        await context.SaveChangesAsync();

        return entity.ValueId;
    }

    public async Task<PagedView<View.InventoryItem>> GetItems(InventoryItemGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.InventoryItems
                           .WhereUser(UserId)
                           .Include(x => x.Brand)
                           .Include(x => x.InventoryItemOwnerships)
                           .ThenInclude(x => x.Ownership)
                           .Where(binding);

        return await query.OrderBy(binding)
                          .Select(x => new View.InventoryItem(x))
                          .ToPagedViewAsync(binding);
    }

    public async Task<IEnumerable<View.Ownership>> GetOwnerships()
    {
        using var context = GetMainContext();
        return await context.Ownerships
                            .OrderBy(x => x.Name)
                            .Select(x => new View.Ownership(x))
                            .ToListAsync();
    }

    public async Task LinkItemToExpense(string itemValueId, string expenseValueId)
    {
        using var context = GetMainContext();

        var item = await context.InventoryItems
                                .WhereUser(UserId)
                                .SingleOrDefaultAsync(x => x.ValueId == itemValueId) ?? throw new ResourceNotFoundException();
        var expense = await context.Expenses
                                   .WhereUser(UserId)
                                   .SingleOrDefaultAsync(x => x.ValueId == expenseValueId) ?? throw new ResourceNotFoundException();

        var entity = new Database.InventoryItemExpense
        {
            InventoryItemId = item.Id,
            ExpenseId = expense.Id
        };

        await context.InventoryItemExpenses.AddAsync(entity);
        await context.SaveChangesAsync();

        var hasOwnerships = await context.InventoryItemOwnerships
                                         .AnyAsync(x => x.InventoryItemId == item.Id);

        if (!hasOwnerships)
        {
            var ownershipId = context.Ownerships.GetId(DefaultOwnershipValueId);

            var ownership = new Database.InventoryItemOwnership
            {
                InventoryItemId = item.Id,
                OwnershipId = ownershipId.Value,
                Created = expense.Date
            };

            await context.InventoryItemOwnerships.AddAsync(ownership);
            await context.SaveChangesAsync();
        }
    }

    public async Task UnlinkItemFromExpense(string itemValueId, string expenseValueId)
    {
        using var context = GetMainContext();

        var item = await context.InventoryItems
                                .WhereUser(UserId)
                                .SingleOrDefaultAsync(x => x.ValueId == itemValueId) ?? throw new ResourceNotFoundException();
        var expense = await context.Expenses
                                   .WhereUser(UserId)
                                   .SingleOrDefaultAsync(x => x.ValueId == expenseValueId) ?? throw new ResourceNotFoundException();

        var link = await context.InventoryItemExpenses
                                .SingleOrDefaultAsync(x => x.InventoryItemId == item.Id && x.ExpenseId == expense.Id) ?? throw new ResourceNotFoundException();

        context.InventoryItemExpenses.Remove(link);
        await context.SaveChangesAsync();
    }

    public async Task UpdateItem(string valueId, InventoryItemBinding binding)
    {
        using var context = GetMainContext();

        var item = await context.InventoryItems
                                 .WhereUser(UserId)
                                 .SingleOrDefaultAsync(x => x.ValueId == valueId) ?? throw new ResourceNotFoundException();

        var ownershipId = context.Ownerships.GetId(binding.OwershipId) ?? context.Ownerships.GetId(DefaultOwnershipValueId).Value;
        var lastOwnershipId = await context.InventoryItemOwnerships
                                           .Where(x => x.InventoryItemId == item.Id)
                                           .OrderByDescending(x => x.Created)
                                           .ThenByDescending(x => x.Id)
                                           .Select(x => (int?)x.OwnershipId)
                                           .FirstOrDefaultAsync();

        item.Name = binding.Name;
        item.BrandId = context.Brands.GetId(binding.BrandId);

        context.InventoryItems.Update(item);

        if (!lastOwnershipId.HasValue || lastOwnershipId.Value != ownershipId)
        {
            var itemOwnership = new Database.InventoryItemOwnership
            {
                InventoryItemId = item.Id,
                OwnershipId = ownershipId,
                Created = DateTime.UtcNow
            };

            await context.InventoryItemOwnerships.AddAsync(itemOwnership);
        }

        await context.SaveChangesAsync();
    }
}
