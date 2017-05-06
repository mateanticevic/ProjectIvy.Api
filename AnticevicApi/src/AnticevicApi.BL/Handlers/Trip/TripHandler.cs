using AnticevicApi.BL.Exceptions;
using AnticevicApi.BL.Handlers.Tracking;
using AnticevicApi.BL.MapExtensions;
using AnticevicApi.DL.Extensions;
using AnticevicApi.Model.Binding.Trip;
using AnticevicApi.Model.Database.Main.Travel;
using AnticevicApi.Model.View;
using Database = AnticevicApi.Model.Database.Main;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using View = AnticevicApi.Model.View;

namespace AnticevicApi.BL.Handlers.Trip
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

        public PaginatedView<View.Trip.Trip> Get(TripGetBinding binding)
        {
            using (var context = GetMainContext())
            {
                var query = context.Trips.WhereUser(User.Id)
                                         .Include($"{nameof(Database.Travel.Trip.Cities)}.{nameof(TripCity.City)}")
                                         .AsQueryable();

                query = binding.From.HasValue ? query.Where(x => x.TimestampEnd > binding.From.Value) : query;
                query = binding.To.HasValue ? query.Where(x => x.TimestampStart < binding.To.Value) : query;

                int? cityId = context.Cities.GetId(binding.CityId);

                query = cityId.HasValue ? query.Where(x => x.Cities.Select(y => y.CityId).Contains(cityId.Value)) : query;

                int count = query.Count();

                var items = query.OrderByDescending(x => x.TimestampStart)
                                 .Page(binding)
                                 .ToList()
                                 .Select(x => new View.Trip.Trip(x));

                return new PaginatedView<View.Trip.Trip>(items, count);
            }
        }

        public View.Trip.Trip GetSingle(string valueId)
        {
            using (var context = GetMainContext())
            {
                var trip = context.Trips.WhereUser(User.Id)
                                        .Include($"{nameof(Database.Travel.Trip.Cities)}.{nameof(TripCity.City)}")
                                        .SingleOrDefault(x => x.ValueId == valueId);

                var excludedExpenseIds = context.TripExpensesExcluded.Where(x => x.TripId == trip.Id)
                                                                     .Select(x => x.ExpenseId)
                                                                     .ToList();

                var includedExpenseIds = context.TripExpensesIncluded.Where(x => x.TripId == trip.Id)
                                                                     .Select(x => x.ExpenseId)
                                                                     .ToList();

                var userExpenses = context.Expenses.WhereUser(User.Id)
                                                   .Include(x => x.Currency)
                                                   .Include(x => x.ExpenseType)
                                                   .Include(x => x.Vendor);

                var expensesWithExcluded = userExpenses.Where(x => trip.TimestampStart.Date <= x.Date && trip.TimestampEnd.Date >= x.Date)
                                                       .Where(x => !excludedExpenseIds.Contains(x.Id));

                var expensesIncluded = userExpenses.Where(x => includedExpenseIds.Contains(x.Id));

                var expenses = expensesWithExcluded.Union(expensesIncluded)
                                                   .OrderBy(x => x.Date)
                                                   .ToList();

                var tripView = new View.Trip.Trip(trip)
                {
                    Expenses = expenses.Select(x => new View.Expense.Expense(x)),
                    Distance = _trackingHandler.GetDistance(new Model.Binding.Common.FilteredBinding(trip.TimestampStart, trip.TimestampEnd))
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
    }
}
