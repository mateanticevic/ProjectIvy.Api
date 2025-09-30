using Dapper;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Common.Extensions;
using ProjectIvy.Data.Databases.Main.Queries;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Data.Sql;
using ProjectIvy.Data.Sql.Main.Scripts;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.Database.Main.Travel;
using ProjectIvy.Model.View;
using System.Linq;
using System.Threading.Tasks;
using Database = ProjectIvy.Model.Database.Main;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Trip;

public class TripHandler : Handler<TripHandler>, ITripHandler
{
    private readonly ITrackingHandler _trackingHandler;

    public TripHandler(IHandlerContext<TripHandler> context, ITrackingHandler trackingHandler) : base(context)
    {
        _trackingHandler = trackingHandler;
    }

    public async Task AddCity(string tripValueId, string cityValueId)
    {
        using var context = GetMainContext();
        int cityId = context.Cities.GetId(cityValueId).Value;
        int tripId = context.Trips.WhereUser(UserId).GetId(tripValueId).Value;
        var cityVisited = new CityVisited()
        {
            CityId = cityId,
            TripId = tripId,
            UserId = UserId
        };
        context.CitiesVisited.Add(cityVisited);
        await context.SaveChangesAsync();
    }

    public async Task AddExpense(string tripValueId, string expenseValueId)
    {
        using var context = GetMainContext();
        int expenseId = context.Expenses.WhereUser(UserId).GetId(expenseValueId).Value;
        int tripId = context.Trips.WhereUser(UserId).GetId(tripValueId).Value;

        var excludedExpense = context.TripExpensesExcluded.SingleOrDefault(x => x.TripId == tripId && x.ExpenseId == expenseId);
        if (excludedExpense != null)
        {
            context.TripExpensesExcluded.Remove(excludedExpense);
            await context.SaveChangesAsync();

            return;
        }

        var tripExpenseIncluded = new TripExpenseInclude()
        {
            ExpenseId = expenseId,
            TripId = tripId
        };

        await context.TripExpensesIncluded.AddAsync(tripExpenseIncluded);
        await context.SaveChangesAsync();
    }

    public async Task AddPoi(string tripValueId, string poiValueId)
    {
        using var context = GetMainContext();
        int tripId = context.Trips.WhereUser(UserId).GetId(tripValueId).Value;
        int poiId = context.Pois.GetId(poiValueId).Value;

        var tripPoi = new TripPoi()
        {
            PoiId = poiId,
            TripId = tripId
        };

        await context.TripPois.AddAsync(tripPoi);
        await context.SaveChangesAsync();
    }

    public async Task Create(TripBinding binding)
    {
        using var context = GetMainContext();
        var trip = binding.ToEntity();
        trip.UserId = UserId;
        await context.Trips.AddAsync(trip);

        foreach (string cityValueId in binding.CityIds.EmptyIfNull())
        {
            var cityId = context.Cities.GetId(cityValueId).Value;
            var cityVisited = new CityVisited()
            {
                CityId = cityId,
                Trip = trip,
                UserId = UserId
            };
            await context.CitiesVisited.AddAsync(cityVisited);
        }

        context.SaveChanges();
    }

    public async Task<IEnumerable<KeyValuePair<int, int>>> DaysByYear(TripGetBinding binding)
    {
        using (var context = GetMainContext())
        {
            var query = context.Trips.WhereUser(UserId)
                                     .Where(binding)
                                     .Where(x => x.TimestampEnd < DateTime.Now);

            var left = await query.Where(x => x.TimestampEnd.Year != x.TimestampStart.Year)
                                  .Select(x => new Tuple<DateTime, DateTime>(x.TimestampStart, new DateTime(x.TimestampStart.Year, 12, 31, 23, 59, 59)))
                                  .ToListAsync();

            var right = await query.Where(x => x.TimestampEnd.Year != x.TimestampStart.Year)
                                   .Select(x => new Tuple<DateTime, DateTime>(new DateTime(x.TimestampEnd.Year, 1, 1, 0, 0, 0), x.TimestampEnd))
                                   .ToListAsync();

            var center = await query.Where(x => x.TimestampEnd.Year == x.TimestampStart.Year)
                                    .Select(x => new Tuple<DateTime, DateTime>(x.TimestampStart, x.TimestampEnd))
                                    .ToListAsync();

            return left.Concat(right)
                       .Concat(center)
                       .GroupBy(x => x.Item1.Year)
                       .Select(x => new KeyValuePair<int, int>(x.Key, x.Sum(y => y.Item2.Date.Subtract(y.Item1.Date).Days + (y.Item2.Hour > 6 ? 1 : 0))))
                       .OrderBy(x => x.Key);
        }
    }

    public async Task Delete(string valueId)
    {
        try
        {
            using var context = GetMainContext();
            var trip = await context.Trips.WhereUser(UserId)
                                    .SingleOrDefaultAsync(x => x.ValueId == valueId);

            context.Trips.Remove(trip);
            await context.SaveChangesAsync();
        }
        catch
        {
            throw new ResourceNotFoundException();
        }
    }

    public async Task<PagedView<View.Trip.Trip>> Get(TripGetBinding binding)
    {
        using var context = GetMainContext();
        var query = context.Trips
                           .WhereUser(UserId)
                           .Include(x => x.Cities)
                           .ThenInclude(x => x.Country)
                           .Where(binding);

        return await query.OrderBy(binding)
                    .Select(x => new View.Trip.Trip(x))
                    .ToPagedViewAsync(binding);
    }

    public async Task<View.Trip.Trip> GetSingle(string valueId)
    {
        using var context = GetMainContext();
        var trip = await context.Trips.WhereUser(UserId)
                                      .Include(x => x.Cities)
                                      .ThenInclude(x => x.Country)
                                      .Include(x => x.Files)
                                      .Include($"{nameof(Database.Travel.Trip.Pois)}.{nameof(TripPoi.Poi)}")
                                      .SingleOrDefaultAsync(x => x.ValueId == valueId);

        var excludedExpenseIds = await context.TripExpensesExcluded.Where(x => x.TripId == trip.Id)
                                                             .Select(x => x.ExpenseId)
                                                             .ToListAsync();

        var includedExpenseIds = await context.TripExpensesIncluded.Where(x => x.TripId == trip.Id)
                                                             .Select(x => x.ExpenseId)
                                                             .ToListAsync();

        var userExpenses = context.Expenses.WhereUser(UserId);

        var expensesWithExcluded = userExpenses.Where(x => trip.TimestampStart.Date <= x.Date && trip.TimestampEnd.Date >= x.Date)
                                               .Where(x => !excludedExpenseIds.Contains(x.Id));

        var expensesIncluded = userExpenses.Where(x => includedExpenseIds.Contains(x.Id));

        var expenseIds = await expensesWithExcluded.Union(expensesIncluded)
                                             .OrderBy(x => x.Date)
                                             .Select(x => x.Id)
                                             .ToListAsync();

        var expenses = await context.Expenses.Where(x => expenseIds.Contains(x.Id))
                                       .IncludeAll()
                                       .OrderBy(x => x.Date)
                                       .ToListAsync();

        decimal totalSpent = 0;

        if (expenseIds.Any())
        {
            using var db = GetSqlConnection();
            int targetCurrencyId = context.GetCurrencyId(null, UserId);
            string sql = SqlLoader.Load(SqlScripts.GetExpenseSumInDefaultCurrency);

            var query = new GetExpenseSumQuery()
            {
                ExpenseIds = expenseIds,
                TargetCurrencyId = targetCurrencyId,
                UserId = UserId
            };

            totalSpent = await db.ExecuteScalarAsync<decimal>(sql, query);
        }

        var stays = await context.Stays.WhereUser(UserId)
                                       .Where(x => x.Date >= trip.TimestampStart.Date && x.Date <= trip.TimestampEnd.Date)
                                       .Include(x => x.City)
                                       .Include(x => x.Country)
                                       .OrderBy(x => x.Date)
                                       .ToListAsync();

        var tripView = new View.Trip.Trip(trip)
        {
            Expenses = expenses.Select(x => new View.Expense.Expense(x)),
            Distance = _trackingHandler.GetDistance(new Model.Binding.FilteredBinding(trip.TimestampStart, trip.TimestampEnd)),
            Stays = stays.Select(x => new View.Stay.Stay(x)),
            TotalSpent = totalSpent
        };

        return tripView;
    }

    public async Task RemoveCity(string tripValueId, string cityValueId)
    {
        using var context = GetMainContext();
        var trip = await context.Trips.WhereUser(UserId).Include(x => x.Cities).SingleOrDefaultAsync(x => x.ValueId == tripValueId);
        var city = await context.Cities.SingleOrDefaultAsync(x => x.ValueId == cityValueId);
        trip.Cities.Remove(city);
        await context.SaveChangesAsync();
    }

    public async Task RemoveExpense(string tripValueId, string expenseValueId)
    {
        using var context = GetMainContext();
        int expenseId = (await context.Expenses.WhereUser(UserId).GetIdAsync(expenseValueId)).Value;
        int tripId = (await context.Trips.WhereUser(UserId).GetIdAsync(tripValueId)).Value;

        var includedExpense = await context.TripExpensesIncluded.SingleOrDefaultAsync(x => x.TripId == tripId && x.ExpenseId == expenseId);
        if (includedExpense != null)
        {
            context.TripExpensesIncluded.Remove(includedExpense);
            await context.SaveChangesAsync();

            return;
        }

        var tripExpenseExcluded = new TripExpenseExclude()
        {
            ExpenseId = expenseId,
            TripId = tripId
        };

        await context.TripExpensesExcluded.AddAsync(tripExpenseExcluded);
        await context.SaveChangesAsync();
    }

    public async Task RemovePoi(string tripValueId, string poiValueId)
    {
        using var context = GetMainContext();
        int poiId = (await context.Pois.GetIdAsync(poiValueId)).Value;
        int tripId = (await context.Trips.WhereUser(UserId).GetIdAsync(tripValueId)).Value;

        var tripPoi = context.TripPois.SingleOrDefault(x => x.PoiId == poiId && x.TripId == tripId);

        if (tripPoi != null)
        {
            context.TripPois.Remove(tripPoi);
            await context.SaveChangesAsync();
        }
        else
        {
            throw new ResourceNotFoundException();
        }
    }
}
