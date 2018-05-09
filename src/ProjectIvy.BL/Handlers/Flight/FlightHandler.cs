using Microsoft.EntityFrameworkCore;
using ProjectIvy.DL.Extensions;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;
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
                                      .GroupBy(x => x)
                                      .Select(x => new CountBy<Views.Airport.Airport>(new Model.View.Airport.Airport(x.Key), x.Count()))
                                      .OrderByDescending(x => x.Count)
                                      .ToList();

            }
        }
    }
}
