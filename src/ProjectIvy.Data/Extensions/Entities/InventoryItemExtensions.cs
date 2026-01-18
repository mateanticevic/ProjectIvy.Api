using System.Linq;
using ProjectIvy.Model.Binding.Inventory;
using ProjectIvy.Model.Database.Main.Inventory;

namespace ProjectIvy.Data.Extensions.Entities;

public static class InventoryItemExtensions
{
    public static IOrderedQueryable<InventoryItem> OrderBy(this IQueryable<InventoryItem> query, InventoryItemGetBinding binding)
    {
        return binding.OrderBy switch
        {
            InventoryItemSort.Name or _ => query.OrderBy(binding.OrderAscending, x => x.Name)
        };
    }

    public static IQueryable<InventoryItem> Where(this IQueryable<InventoryItem> query, InventoryItemGetBinding binding)
    {
        var search = binding.Search?.ToLower();

        return query
            .WhereIf(binding.BrandId, x => x.Brand != null && binding.BrandId.Contains(x.Brand.ValueId))
            .WhereIf(!string.IsNullOrWhiteSpace(search), x => x.Name.ToLower().Contains(search) || x.ValueId.ToLower().Contains(search));
    }
}
