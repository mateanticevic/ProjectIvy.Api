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

namespace ProjectIvy.Business.Handlers.Trip
{
    public class TripHandler : Handler<TripHandler>, ITripHandler
    {
        private readonly ITrackingHandler _trackingHandler;

        public TripHandler(IHandlerContext<TripHandler> context, ITrackingHandler trackingHandler) : base(context)
        {
            _trackingHandler = trackingHandler;
        }

        public void AddCity(string tripValueId, string cityValueId)
        {
            using (var context = GetMainContext())
            {
                var city = context.Cities.SingleOrDefault(x => x.ValueId == cityValueId);
                context.Trips.Include(x => x.Cities).SingleOrDefault(x => x.ValueId == tripValueId).Cities.Add(city);
                context.SaveChanges();
            }
        }

        public void AddExpense(string tripValueId, string expenseValueId)
        {
            using (var context = GetMainContext())
            {
                int expenseId = context.Expenses.WhereUser(UserId).GetId(expenseValueId).Value;
                int tripId = context.Trips.WhereUser(UserId).GetId(tripValueId).Value;

                var excludedExpense = context.TripExpensesExcluded.SingleOrDefault(x => x.TripId == tripId && x.ExpenseId == expenseId);
                if (excludedExpense != null)
                {
                    context.TripExpensesExcluded.Remove(excludedExpense);
                    context.SaveChanges();

                    return;
                }

                var tripExpenseIncluded = new TripExpenseInclude()
                {
                    ExpenseId = expenseId,
                    TripId = tripId
                };

                context.TripExpensesIncluded.Add(tripExpenseIncluded);
                context.SaveChanges();
            }
        }

        public void AddPoi(string tripValueId, string poiValueId)
        {
            using (var context = GetMainContext())
            {
                int tripId = context.Trips.WhereUser(UserId).GetId(tripValueId).Value;
                int poiId = context.Pois.GetId(poiValueId).Value;

                var tripPoi = new TripPoi()
                {
                    PoiId = poiId,
                    TripId = tripId
                };

                context.TripPois.Add(tripPoi);
                context.SaveChanges();
            }
        }

        public void Create(TripBinding binding)
        {
            using (var context = GetMainContext())
            {
                var trip = binding.ToEntity();
                trip.UserId = UserId;
                context.Trips.Add(trip);

                foreach (string cityValueId in binding.CityIds.EmptyIfNull())
                {
                    var city = context.Cities.SingleOrDefault(x => x.ValueId == cityValueId);
                    trip.Cities.Add(city);
                }

                context.SaveChanges();
            }
        }

        public async Task<IEnumerable<KeyValuePair<int, int>>> DaysByYear(TripGetBinding binding)
        {
            using(var context = GetMainContext())
            {
                var query = context.Trips.WhereUser(UserId)
                                         .Where(binding)
                                         .Where(x => x.TimestampEnd < DateTime.Now);

                var left = await query.Where(x => x.TimestampEnd.Year != x.TimestampStart.Year)
                                      .Select(x => new Tuple<DateTime, DateTime>(x.TimestampStart, new DateTime(x.TimestampStart.Year, 12, 31, 23, 59, 59)))
                                      .ToListAsync();
                                    
                var right = await query.Where(x => x.TimestampEnd.Year != x.TimestampStart.Year)
                                       .Select(x => new Tuple<DateTime, DateTime>(new DateTime(x.TimestampEnd.Year, 1, 1, 0, 0, 0), x.TimestampEnd))
                                       .ToListAsync();

                var center = await query.Where(x => x.TimestampEnd.Year == x.TimestampStart.Year)
                                        .Select(x => new Tuple<DateTime, DateTime>(x.TimestampStart, x.TimestampEnd))
                                        .ToListAsync();

                return left.Concat(right)
                           .Concat(center)
                           .GroupBy(x => x.Item1.Year)
                           .Select(x => new KeyValuePair<int, int>(x.Key, x.Sum(y => y.Item2.Date.Subtract(y.Item1.Date).Days + (y.Item2.Hour > 6 ? 1 : 0))))
                           .OrderBy(x => x.Key);
            }
        }

        public void Delete(string valueId)
        {
            try
            {
                using (var context = GetMainContext())
                {
                    var trip = context.Trips.WhereUser(UserId)
                                            .SingleOrDefault(x => x.ValueId == valueId);

                    context.Trips.Remove(trip);
                    context.SaveChanges();
                }
            }
            catch
            {
                throw new ResourceNotFoundException();
            }
        }

        public PagedView<View.Trip.Trip> Get(TripGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var query = context.Trips
                                   .WhereUser(UserId)
                                   .Include(x => x.Cities)
                                   .ThenInclude(x => x.Country)
                                   .Where(binding);

                return query.OrderBy(binding)
                            .Select(x => new View.Trip.Trip(x))
                            .ToPagedView(binding);
            }
        }

        public View.Trip.Trip GetSingle(string valueId)
        {
            using (var context = GetMainContext())
            {
                var trip = context.Trips.WhereUser(UserId)
                                        .Include(x => x.Cities)
                                        .ThenInclude(x => x.Country)
                                        .Include(x => x.Files)
                                        .Include($"{nameof(Database.Travel.Trip.Pois)}.{nameof(TripPoi.Poi)}")
                                        .SingleOrDefault(x => x.ValueId == valueId);

                var excludedExpenseIds = context.TripExpensesExcluded.Where(x => x.TripId == trip.Id)
                                                                     .Select(x => x.ExpenseId)
                                                                     .ToList();

                var includedExpenseIds = context.TripExpensesIncluded.Where(x => x.TripId == trip.Id)
                                                                     .Select(x => x.ExpenseId)
                                                                     .ToList();

                var userExpenses = context.Expenses.WhereUser(UserId);

                var expensesWithExcluded = userExpenses.Where(x => trip.TimestampStart.Date <= x.Date && trip.TimestampEnd.Date >= x.Date)
                                                       .Where(x => !excludedExpenseIds.Contains(x.Id));

                var expensesIncluded = userExpenses.Where(x => includedExpenseIds.Contains(x.Id));

                var expenseIds = expensesWithExcluded.Union(expensesIncluded)
                                                     .OrderBy(x => x.Date)
                                                     .Select(x => x.Id)
                                                     .ToList();

                var expenses = context.Expenses.Where(x => expenseIds.Contains(x.Id))
                                               .IncludeAll()
                                               .OrderBy(x => x.Date)
                                               .ToList();

                decimal totalSpent = 0;

                if (expenseIds.Any())
                {
                    using (var db = GetSqlConnection())
                    {
                        int targetCurrencyId = context.GetCurrencyId(null, UserId);
                        string sql = SqlLoader.Load(SqlScripts.GetExpenseSumInDefaultCurrency);

                        var query = new GetExpenseSumQuery()
                        {
                            ExpenseIds = expenseIds,
                            TargetCurrencyId = targetCurrencyId,
                            UserId = UserId
                        };

                        totalSpent = db.ExecuteScalar<decimal>(sql, query);
                    }
                }

                var tripView = new View.Trip.Trip(trip)
                {
                    Expenses = expenses.Select(x => new View.Expense.Expense(x)),
                    Distance = _trackingHandler.GetDistance(new Model.Binding.FilteredBinding(trip.TimestampStart, trip.TimestampEnd)),
                    TotalSpent = totalSpent
                };

                return tripView;
            }
        }

        public void RemoveCity(string tripValueId, string cityValueId)
        {
            using (var context = GetMainContext())
            {
                var trip = context.Trips.WhereUser(UserId).Include(x => x.Cities).SingleOrDefault(tripValueId);
                var city = context.Cities.SingleOrDefault(x => x.ValueId == cityValueId);
                trip.Cities.Remove(city);
                context.SaveChanges();
            }
        }

        public void RemoveExpense(string tripValueId, string expenseValueId)
        {
            using (var context = GetMainContext())
            {
                int expenseId = context.Expenses.WhereUser(UserId).GetId(expenseValueId).Value;
                int tripId = context.Trips.WhereUser(UserId).GetId(tripValueId).Value;

                var includedExpense = context.TripExpensesIncluded.SingleOrDefault(x => x.TripId == tripId && x.ExpenseId == expenseId);
                if (includedExpense != null)
                {
                    context.TripExpensesIncluded.Remove(includedExpense);
                    context.SaveChanges();

                    return;
                }

                var tripExpenseExcluded = new TripExpenseExclude()
                {
                    ExpenseId = expenseId,
                    TripId = tripId
                };

                context.TripExpensesExcluded.Add(tripExpenseExcluded);
                context.SaveChanges();
            }
        }

        public void RemovePoi(string tripValueId, string poiValueId)
        {
            using (var context = GetMainContext())
            {
                int poiId = context.Pois.GetId(poiValueId).Value;
                int tripId = context.Trips.WhereUser(UserId).GetId(tripValueId).Value;

                var tripPoi = context.TripPois.SingleOrDefault(x => x.PoiId == poiId && x.TripId == tripId);

                if (tripPoi != null)
                {
                    context.TripPois.Remove(tripPoi);
                    context.SaveChanges();
                }
                else
                {
                    throw new ResourceNotFoundException();
                }
            }
        }
    }
}
