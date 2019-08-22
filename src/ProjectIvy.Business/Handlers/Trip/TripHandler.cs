using Dapper;
using Database = ProjectIvy.Model.Database.Main;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Business.Exceptions;
using ProjectIvy.Business.Handlers.Tracking;
using ProjectIvy.Business.MapExtensions;
using ProjectIvy.Data.Databases.Main.Queries;
using ProjectIvy.Data.Extensions.Entities;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Data.Sql;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.Database.Main.Travel;
using ProjectIvy.Model.View;
using System.Linq;
using ProjectIvy.Data.Sql.Main.Scripts;
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
                int tripId = context.Trips.WhereUser(User.Id).GetId(tripValueId).Value;
                int cityId = context.Cities.GetId(cityValueId).Value;

                var tripCity = new TripCity()
                {
                    CityId = cityId,
                    TripId = tripId
                };

                context.TripCities.Add(tripCity);
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
                var query = context.Trips.WhereUser(User.Id)
                                         .Include($"{nameof(Database.Travel.Trip.Cities)}.{nameof(TripCity.City)}.{nameof(Database.Common.City.Country)}")
                                         .WhereIf(binding.From.HasValue, x => x.TimestampEnd > binding.From.Value)
                                         .WhereIf(binding.To.HasValue, x => x.TimestampStart < binding.To.Value)
                                         .WhereIf(!string.IsNullOrWhiteSpace(binding.CountryId), x => x.Cities.Select(y => y.City.Country).Any(y => y.ValueId == binding.CountryId));

                int? cityId = context.Cities.GetId(binding.CityId);

                query = cityId.HasValue ? query.Where(x => x.Cities.Select(y => y.CityId).Contains(cityId.Value)) : query;

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
                                        .Include($"{nameof(Database.Travel.Trip.Cities)}.{nameof(TripCity.City)}")
                                        .Include($"{nameof(Database.Travel.Trip.Cities)}.{nameof(TripCity.City)}.{nameof(Database.Common.City.Country)}")
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
                int cityId = context.Cities.GetId(cityValueId).Value;
                int tripId = context.Trips.WhereUser(User.Id).GetId(tripValueId).Value;

                var tripCity = context.TripCities.SingleOrDefault(x => x.CityId == cityId && x.TripId == tripId);

                if (tripCity != null)
                {
                    context.TripCities.Remove(tripCity);
                    context.SaveChanges();
                }
                else
                {
                    throw new ResourceNotFoundException();
                }
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
