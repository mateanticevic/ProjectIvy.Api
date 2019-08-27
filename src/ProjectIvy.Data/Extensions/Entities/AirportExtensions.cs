using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Binding.Airport;
using ProjectIvy.Model.Database.Main.Transport;
using System.Linq;

namespace ProjectIvy.Data.Extensions.Entities
{
    public static class AirportExtensions
    {
        public static IQueryable<Airport> Where(this IQueryable<Airport> query, AirportGetBinding binding, MainContext context, int? userId = null)
        {
            var airports = context.Airports.Include(x => x.City).AsQueryable();

            if (binding.Visited.HasValue)
            {
                var visitedAirports = context.Flights.WhereUser(userId.Value)
                    .Select(x => new { Destination = x.DestinationAirportId, Origin = x.OriginAirportId })
                    .ToList();

                var visitedAirportIds = visitedAirports.Select(x => x.Destination)
                    .Concat(visitedAirports.Select(x => x.Origin))
                    .ToList();

                airports = binding.Visited.Value ? airports.Where(x => visitedAirportIds.Contains(x.Id)) : airports.Where(x => !visitedAirportIds.Contains(x.Id));
            }

            int? cityId = context.Cities.GetId(binding.CityId);
            airports = cityId.HasValue ? airports.Where(x => x.CityId == cityId) : airports;

            int? countryId = context.Countries.GetId(binding.Countryid);
            airports = countryId.HasValue ? airports.Where(x => x.City.CountryId == countryId) : airports;

            return airports;
        }
    }
}
