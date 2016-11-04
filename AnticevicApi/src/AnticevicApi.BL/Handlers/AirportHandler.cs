using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.View.Airport;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace AnticevicApi.BL.Handlers
{
    public class AirportHandler : Handler
    {
        public AirportHandler()
        {

        }

        public AirportHandler(int userId)
        {
            UserId = userId;
        }

        public IEnumerable<Airport> Get(bool onlyVisited = false)
        {
            using (var db = new MainContext())
            {
                var destinationAirports = db.Flights.WhereUser(UserId)
                                                    .Include(x => x.DestinationAirport)
                                                    .Select(x => x.DestinationAirport)
                                                    .Distinct()
                                                    .ToList();

                var originAirports = db.Flights.WhereUser(UserId)
                                               .Include(x => x.OriginAirport)
                                               .Select(x => x.OriginAirport)
                                               .Distinct()
                                               .ToList();

                var visitedAirports = destinationAirports.Union(originAirports).Distinct();

                return visitedAirports.Select(x => new Airport(x));
            }
        }
    }
}
