using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Airport;
using ProjectIvy.Model.View;
using System.Linq;
using View = ProjectIvy.Model.View.Airport;

namespace ProjectIvy.Business.Handlers.Airport
{
    public class AirportHandler : Handler<AirportHandler>, IAirportHandler
    {
        public AirportHandler(IHandlerContext<AirportHandler> context) : base(context)
        {
        }

        public long Count(AirportGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Airports.Where(binding, context, User.Id)
                                       .LongCount();
            }
        }

        public PagedView<View.Airport> Get(AirportGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Airports.Where(binding, context, User.Id)
                                       .WhereIf(binding.Search, x => x.Iata == binding.Search.ToUpper() || x.Name.ToLower().Contains(binding.Search.ToLower()))
                                       .Include(x => x.Poi)
                                       .ThenInclude(x => x.PoiCategory)
                                       .OrderByDescending(x => x.Iata == binding.Search)
                                       .Select(x => new View.Airport(x))
                                       .ToPagedView(binding);
            }
        }
    }
}
