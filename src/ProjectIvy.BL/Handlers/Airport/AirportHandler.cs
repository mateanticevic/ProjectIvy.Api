using ProjectIvy.DL.Extensions.Entities;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.Binding.Airport;
using ProjectIvy.Model.View;
using System.Linq;
using View = ProjectIvy.Model.View.Airport;

namespace ProjectIvy.BL.Handlers.Airport
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
                                       .Select(x => new View.Airport(x))
                                       .ToPagedView(binding);
            }
        }
    }
}
