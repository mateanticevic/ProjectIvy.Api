using AnticevicApi.BL.MapExtensions;
using AnticevicApi.Model.Binding.Poi;

namespace AnticevicApi.BL.Handlers.Poi
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
    }
}
