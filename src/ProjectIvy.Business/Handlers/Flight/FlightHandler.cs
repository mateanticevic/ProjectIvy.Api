using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Model.Binding.Flight;
using ProjectIvy.Model.View;
using Views = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Flight;

public class FlightHandler : Handler<FlightHandler>, IFlightHandler
{
    public FlightHandler(IHandlerContext<FlightHandler> context) : base(context)
    {
    }

    public async Task<int> Count(FlightGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Flights.WhereUser(UserId)
                              .Where(binding)
                              .CountAsync();
    }

    public async Task<IEnumerable<KeyValuePair<Views.Airline.Airline, int>>> CountByAirline(FlightGetBinding binding)
    {
        using var context = GetMainContext();

        return await context.Flights.WhereUser(UserId)
                                    .Include(x => x.Airline)
                                    .Select(x => x.Airline)
                                    .GroupBy(x => x)
                                    .OrderByDescending(x => x.Count())
                                    .Select(x => new KeyValuePair<Views.Airline.Airline, int>(new Views.Airline.Airline(x.Key), x.Count()))
                                    .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<Views.Airport.Airport, int>>> CountByAirport(FlightGetBinding binding)
    {
        using var context = GetMainContext();
        var userAirports = context.Flights.WhereUser(UserId)
                                          .Where(binding)
                                          .Include(x => x.DestinationAirport)
                                          .ThenInclude(x => x.Poi)
                                          .Include(x => x.OriginAirport)
                                          .ThenInclude(x => x.Poi);

        return await userAirports.Select(x => x.DestinationAirport)
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
                           .ToListAsync();
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> CountByYear(FlightGetBinding binding)
    {
        using var context = GetMainContext();
        return await context.Flights.WhereUser(UserId)
                              .Where(binding)
                              .GroupBy(x => x.DateOfDepartureLocal.Year)
                              .OrderByDescending(x => x.Key)
                              .Select(x => new KeyValuePair<int, int>(x.Key, x.Count()))
                              .ToListAsync();
    }

    public async Task Create(FlightBinding binding)
    {
        using var context = GetMainContext();
        var entity = await binding.ToEntity(context);
        entity.UserId = UserId;

        await context.Flights.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<PagedView<Views.Flight.Flight>> Get(FlightGetBinding binding)
    {
        using var context = GetMainContext();
        int? destinationAirportId = binding.DestinationId is null ? null : (await context.Airports.SingleOrDefaultAsync(x => x.Iata == binding.DestinationId))?.Id;
        int? originAirportId = binding.OriginId is null ? null : (await context.Airports.SingleOrDefaultAsync(x => x.Iata == binding.OriginId))?.Id;

        return await context.Flights.WhereUser(UserId)
                              .Where(binding)
                              .WhereIf(destinationAirportId, x => x.DestinationAirportId == destinationAirportId)
                              .WhereIf(originAirportId, x => x.OriginAirportId == originAirportId)
                              .Include(x => x.Airline)
                              .Include(x => x.DestinationAirport)
                              .ThenInclude(x => x.Poi)
                              .Include(x => x.OriginAirport)
                              .ThenInclude(x => x.Poi)
                              .OrderByDescending(x => x.DateOfArrivalLocal)
                              .Select(x => new Views.Flight.Flight(x))
                              .ToPagedViewAsync(binding);
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> GetDistanceByYear()
    {
        using var context = GetMainContext();

        return (await context.Flights.WhereUser(UserId)
                              .Include(x => x.Airline)
                              .Include(x => x.DestinationAirport)
                              .ThenInclude(x => x.Poi)
                              .Include(x => x.OriginAirport)
                              .ThenInclude(x => x.Poi)
                              .OrderByDescending(x => x.DateOfArrivalLocal)
                              .Select(x => new Views.Flight.Flight(x))
                              .ToListAsync())
                              .GroupBy(x => x.DepartureLocal.Year)
                              .Select(g => new KeyValuePair<int, int>(g.Key, g.Sum(x => x.DistanceInKm ?? 0)));
    }

    public async Task Update(string valueId, FlightBinding flight)
    {
        using var context = GetMainContext();

        var entity = await context.Flights.WhereUser(UserId).SingleOrDefaultAsync(x => x.ValueId == valueId);
        entity = await flight.ToEntity(context, entity);

        context.Flights.Update(entity);
        await context.SaveChangesAsync();
    }
}
