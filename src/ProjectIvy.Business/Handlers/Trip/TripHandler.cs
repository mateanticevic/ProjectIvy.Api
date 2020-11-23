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
using Database = ProjectIvy.Model.Database.Main;
using View = ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Trip
{
    public class TripHandler : Handler<TripHandler>, ITripHandler
    {
        private readonly ITrackingHandler _trackingHandler;
        private object x;

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
                int expenseId = context.Expenses.WhereUser(User.Id).GetId(expenseValueId).Value;
                int tripId = context.Trips.WhereUser(User.Id).GetId(tripValueId).Value;

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
                int tripId = context.Trips.WhereUser(User.Id).GetId(tripValueId).Value;
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
                trip.UserId = User.Id;
                context.Trips.Add(trip);

                foreach (string cityValueId in binding.CityIds.EmptyIfNull())
                {
                    var city = context.Cities.SingleOrDefault(x => x.ValueId == cityValueId);
                    trip.Cities.Add(city);
                }

                context.SaveChanges();
            }
        }

        public void Delete(string valueId)
        {
            try
            {
                using (var context = GetMainContext())
                {
                    var trip = context.Trips.WhereUser(User)
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
                                   .WhereUser(User.Id)
                                   .Include(x => x.Cities)
                                   .ThenInclude(x => x.Country)
                                   .WhereIf(binding.From.HasValue, x => x.TimestampEnd > binding.From.Value)
                                   .WhereIf(binding.To.HasValue, x => x.TimestampStart < binding.To.Value)
                                   .WhereIf(binding.CityId, x => x.Cities.Select(y => y.ValueId).Any(y => binding.CityId.Contains(y)))
                                   .WhereIf(binding.CountryId, x => x.Cities.Select(y => y.Country.ValueId).Any(y => binding.CountryId.Contains(y)));

                return query.OrderBy(binding)
                            .Select(x => new View.Trip.Trip(x))
                            .ToPagedView(binding);
            }
        }

        public View.Trip.Trip GetSingle(string valueId)
        {
            using (var context = GetMainContext())
            {
                var trip = context.Trips.WhereUser(User.Id)
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

                var userExpenses = context.Expenses.WhereUser(User.Id);

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
                        int targetCurrencyId = context.GetCurrencyId(null, User.Id);
                        string sql = SqlLoader.Load(Constants.GetExpenseSumInDefaultCurrency);

                        var query = new GetExpenseSumQuery()
                        {
                            ExpenseIds = expenseIds,
                            TargetCurrencyId = targetCurrencyId,
                            UserId = User.Id
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
                var trip = context.Trips.WhereUser(User).Include(x => x.Cities).SingleOrDefault(tripValueId);
                var city = context.Cities.SingleOrDefault(x => x.ValueId == cityValueId);
                trip.Cities.Remove(city);
                context.SaveChanges();
            }
        }

        public void RemoveExpense(string tripValueId, string expenseValueId)
        {
            using (var context = GetMainContext())
            {
                int expenseId = context.Expenses.WhereUser(User.Id).GetId(expenseValueId).Value;
                int tripId = context.Trips.WhereUser(User.Id).GetId(tripValueId).Value;

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
                int tripId = context.Trips.WhereUser(User.Id).GetId(tripValueId).Value;

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
