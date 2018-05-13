using Microsoft.EntityFrameworkCore;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;
using ProjectIvy.Model.Binding;
using Entities = ProjectIvy.Model.Database.Main;
using Views = ProjectIvy.Model.View;

namespace ProjectIvy.BL.Handlers.Flight
{
    public class FlightHandler : Handler<FlightHandler>, IFlightHandler
    {
        public FlightHandler(IHandlerContext<FlightHandler> context) : base(context)
        {
        }

        public int Count()
        {
            using (var context = GetMainContext())
            {
                return context.Flights.WhereUser(User)
                                      .Count();
            }
        }

        public IEnumerable<CountBy<Views.Airport.Airport>> CountByAirport()
        {
            using (var context = GetMainContext())
            {
                return context.Flights.WhereUser(User)
                                      .Include(x => x.DestinationAirport)
                                      .Include(x => x.OriginAirport)
                                      .Select(x => new List<Entities.Transport.Airport>(){ x.DestinationAirport, x.OriginAirport })
                                      .SelectMany(x => x)
                                      .Include(x => x.Poi)
                                      .GroupBy(x => x)
                                      .Select(x => new CountBy<Views.Airport.Airport>(new Model.View.Airport.Airport(x.Key), x.Count()))
                                      .OrderByDescending(x => x.Count)
                                      .ToList();

            }
        }

        public PagedView<Views.Flight.Flight> Get(FilteredPagedBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Flights.WhereUser(User)
                                      .Include(x => x.DestinationAirport)
                                      .ThenInclude(x => x.Poi)
                                      .Include(x => x.OriginAirport)
                                      .ThenInclude(x => x.Poi)
                                      .OrderByDescending(x => x.DateOfArrival)
                                      .Select(x => new Views.Flight.Flight(x))
                                      .ToPagedView(binding);
            }
        }
    }
}
