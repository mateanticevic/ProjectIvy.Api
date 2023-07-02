using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.View;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                return context.Flights.WhereUser(UserId)
                                      .Where(binding)
                                      .Count();
            }
        }

        public async Task Create(FlightBinding binding)
        {
            using (var context = GetMainContext())
            {
                var entity = binding.ToEntity(context);
                entity.UserId = UserId;

                await context.Flights.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public IEnumerable<KeyValuePair<Views.Airport.Airport, int>> CountByAirport(FlightGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var userAirports = context.Flights.WhereUser(UserId)
                                                  .Where(binding)
                                                  .Include(x => x.DestinationAirport)
                                                  .ThenInclude(x => x.Poi)
                                                  .Include(x => x.OriginAirport)
                                                  .ThenInclude(x => x.Poi);

                return userAirports.Select(x => x.DestinationAirport)
                                   .Concat(userAirports.Select(x => x.OriginAirport))
                                   .GroupBy(x => new
                                   {
                                       x.Iata,
                                       x.Name,
                                       x.Poi.Latitude,
                                       x.Poi.Longitude
                                   })
                                   .OrderByDescending(x => x.Count())
                                   .Select(x => new KeyValuePair<Views.Airport.Airport, int>(new()
                                   {
                                       Iata = x.Key.Iata,
                                       Name = x.Key.Name,
                                       Poi = new Views.Poi.Poi()
                                       {
                                           Location = new Model.View.LatLng(x.Key.Latitude, x.Key.Longitude)
                                       }
                                   }, x.Count()))
                                   .ToList();
            }
        }

        public IEnumerable<KeyValuePair<int, int>> CountByYear(FlightGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                return context.Flights.WhereUser(UserId)
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
                int? destinationAirportId = binding.DestinationId is null ? null : context.Airports.SingleOrDefault(x => x.Iata == binding.DestinationId)?.Id;
                int? originAirportId = binding.OriginId is null ? null : context.Airports.SingleOrDefault(x => x.Iata == binding.OriginId)?.Id;

                return context.Flights.WhereUser(UserId)
                                      .Where(binding)
                                      .WhereIf(destinationAirportId, x => x.DestinationAirportId == destinationAirportId)
                                      .WhereIf(originAirportId, x => x.OriginAirportId == originAirportId)
                                      .Include(x => x.Airline)
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
