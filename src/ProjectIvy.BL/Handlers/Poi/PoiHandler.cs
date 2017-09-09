using ProjectIvy.BL.MapExtensions;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Poi;
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

        public IEnumerable<Model.View.Poi.Poi> Get(PoiGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                int? vendorId = context.Vendors.GetId(binding.VendorId);

                var pois = context.VendorPois.WhereIf(vendorId.HasValue, x => x.VendorId == vendorId.Value)
                                             .Select(x => x.Poi)
                                             .ToList();

                return pois.Select(x => new Model.View.Poi.Poi(x));
            }
        }
    }
}
