using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Airport;
using AnticevicApi.Model.View;
using Database = AnticevicApi.Model.Database.Main;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using View = AnticevicApi.Model.View.Airport;

namespace AnticevicApi.BL.Handlers.Airport
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
                return Filter(binding, context).LongCount();
            }
        }

        public PagedView<View.Airport> Get(AirportGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var airports = Filter(binding, context);

                long count = airports.Count();

                var items = airports.Page(binding)
                                    .ToList()
                                    .Select(x => new View.Airport(x))
                                    .ToList();

                return new PagedView<View.Airport>()
                {
                    Count = count,
                    Items = items
                };
            }
        }

        private IQueryable<Database.Transport.Airport> Filter(AirportGetBinding binding, MainContext context)
        {
            var airports = context.Airports.Include(x => x.City).AsQueryable();

            if (binding.Visited.HasValue)
            {
                var visitedAirports = context.Flights.WhereUser(User.Id)
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
