using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.DbContexts;
using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.Database.Main.Transport;
using ProjectIvy.Data.Extensions;

namespace ProjectIvy.Business.MapExtensions;

public static class FlightExtensions
{
    public static async Task<Flight> ToEntity(this FlightBinding b, MainContext context, Flight entity = null)
    {
        if (entity is null)
            entity = new Flight();

        entity.ValueId = $"{b.OriginId}{b.DestinationId}{b.DepartureLocal:yyyyMMdd}";
        entity.AirlineId = (await context.Airlines.GetIdAsync(b.AirlineId)).Value;
        entity.DateOfArrival = b.Arrival;
        entity.DateOfArrivalLocal = b.ArrivalLocal;
        entity.DateOfDeparture = b.Departure;
        entity.DateOfDepartureLocal = b.DepartureLocal;
        entity.DestinationAirportId = (await context.Airports.SingleAsync(x => x.Iata == b.DestinationId)).Id;
        entity.FlightNumber = b.Number;
        entity.OriginAirportId = (await context.Airports.SingleAsync(x => x.Iata == b.OriginId)).Id;

        return entity;
    }
}
