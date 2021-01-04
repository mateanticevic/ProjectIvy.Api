using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;
using Views = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Flight
{
    public class FlightHandler : Handler<FlightHandler>, IFlightHandler
    {
        public FlightHandler(IHandlerContext<FlightHandler> context) : base(context)
        {
        }

        public int Count(FlightGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Flights.WhereUser(User)
                                      .Where(binding)
                                      .Count();
            }
        }

        public IEnumerable<KeyValuePair<Views.Airport.Airport, int>> CountByAirport(FlightGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var userAirports = context.Flights.WhereUser(User)
                                                  .Where(binding)
                                                  .Include(x => x.DestinationAirport)
                                                  .Include(x => x.OriginAirport);
                string s = userAirports.Select(x => x.DestinationAirport)
                                   .Concat(userAirports.Select(x => x.OriginAirport))
                                   .GroupBy(x => new
                                   {
                                       x.Iata,
                                       x.Name
                                   })
                                   .OrderByDescending(x => x.Count())
                                   .Select(x => new KeyValuePair<Views.Airport.Airport, int>(new()
                                   {
                                       Iata = x.Key.Iata,
                                       Name = x.Key.Name
                                   }, x.Count())).ToQueryString();

                return userAirports.Select(x => x.DestinationAirport)
                                   .Concat(userAirports.Select(x => x.OriginAirport))
                                   .GroupBy(x => new
                                   {
                                       x.Iata,
                                       x.Name
                                   })
                                   .OrderByDescending(x => x.Count())
                                   .Select(x => new KeyValuePair<Views.Airport.Airport, int>(new()
                                   {
                                       Iata = x.Key.Iata,
                                       Name = x.Key.Name
                                   }, x.Count()))
                                   .ToList();
            }
        }

        public IEnumerable<KeyValuePair<int, int>> CountByYear(FlightGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Flights.WhereUser(User)
                                      .Where(binding)
                                      .GroupBy(x => x.DateOfDeparture.Year)
                                      .OrderByDescending(x => x.Key)
                                      .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                                      .ToList();
            }
        }

        public PagedView<Views.Flight.Flight> Get(FlightGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Flights.WhereUser(User)
                                      .Where(binding)
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
