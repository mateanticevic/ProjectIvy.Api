using Microsoft.EntityFrameworkCore;
using ProjectIvy.BL.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Poi;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIvy.BL.Handlers.Poi
{
    public class PoiHandler : Handler<PoiHandler>, IPoiHandler
    {
        public PoiHandler(IHandlerContext<PoiHandler> context) : base(context)
        {
        }

        public void Create(PoiBinding binding)
        {
            using (var context = GetMainContext())
            {
                var entity = binding.ToEntity(context);

                context.Pois.Add(entity);
                context.SaveChanges();
            }
        }

        public PagedView<Model.View.Poi.Poi> Get(PoiGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? categoryId = context.PoiCategories.GetId(binding.CategoryId);
                int? vendorId = context.Vendors.GetId(binding.VendorId);

                var pois = context.Pois.Include(x => x.PoiCategory)
                                       .WhereIf(categoryId.HasValue, x => x.PoiCategoryId == categoryId)
                                       .WhereIf(!string.IsNullOrWhiteSpace(binding.Name), x => x.Name.Contains(binding.Name))
                                       .WhereIf(vendorId.HasValue, x => x.VendorPois.Any(y => y.VendorId == vendorId && x.Id == y.PoiId))
                                       .InsideRectangle(binding.X, binding.Y);

                var result = new PagedView<Model.View.Poi.Poi>();
                result.Count = pois.Count();
                result.Items = pois.OrderByDescending(x => x.Id)
                                   .Page(binding)
                                   .ToList()
                                   .Select(x => new Model.View.Poi.Poi(x));

                return result;
            }
        }

        public IEnumerable<Model.View.Poi.PoiCategory> GetCategories()
        {
            using (var context = GetMainContext())
            {
                return context.PoiCategories.OrderBy(x => x.Name)
                                            .ToList()
                                            .Select(x => new Model.View.Poi.PoiCategory(x));
            }
        }
    }
}
