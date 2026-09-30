using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Poi;
using ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Poi;

public class PoiHandler : Handler<PoiHandler>, IPoiHandler
{
    public PoiHandler(IHandlerContext<PoiHandler> context) : base(context)
    {
    }

    public async Task Create(PoiBinding binding)
    {
        using var context = GetMainContext();
        var entity = await binding.ToEntity(context);

        await context.Pois.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<PagedView<Model.View.Poi.Poi>> Get(PoiGetBinding binding)
    {
        using var context = GetMainContext();
        int? categoryId = await context.PoiCategories.GetIdAsync(binding.CategoryId);
        int? vendorId = await context.Vendors.GetIdAsync(binding.VendorId);

        var pois = context.Pois.Include(x => x.PoiCategory)
                               .WhereIf(categoryId.HasValue, x => x.PoiCategoryId == categoryId)
                               .WhereIf(!string.IsNullOrWhiteSpace(binding.Name), x => x.Name.Contains(binding.Name))
                               .WhereIf(vendorId.HasValue, x => x.VendorPois.Any(y => y.VendorId == vendorId && x.Id == y.PoiId))
                               .WhereIf(binding.Search, x => x.Name.ToLower().Contains(binding.Search.ToLower()))
                               .InsideRectangle(binding.X, binding.Y);

        var result = new PagedView<Model.View.Poi.Poi>();
        result.Count = await pois.CountAsync();
        result.Items = (await pois.OrderByDescending(x => x.Id)
                           .Page(binding)
                           .ToListAsync())
                           .Select(x => new Model.View.Poi.Poi(x));

        return result;
    }

    public async Task<IEnumerable<Model.View.Poi.PoiCategory>> GetCategories()
    {
        using var context = GetMainContext();
        return (await context.PoiCategories.OrderBy(x => x.Name)
                                    .ToListAsync())
                                    .Select(x => new Model.View.Poi.PoiCategory(x));
    }
}
