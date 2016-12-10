using AnticevicApi.DL.DbContexts;
using AnticevicApi.DL.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using View = AnticevicApi.Model.View.Airport;

namespace AnticevicApi.BL.Handlers.Airport
{
    public class AirportHandler : Handler, IAirportHandler
    {
        public AirportHandler()
        {

        }

        public AirportHandler(string connectionString, int userId) : base(connectionString, userId)
        {
        }

        public IEnumerable<View.Airport> Get(bool onlyVisited = false)
        {
            using (var db = new MainContext(ConnectionString))
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

                return visitedAirports.Select(x => new View.Airport(x));
            }
        }
    }
}
