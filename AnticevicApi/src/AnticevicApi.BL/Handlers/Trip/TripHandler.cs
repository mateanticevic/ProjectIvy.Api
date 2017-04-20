using AnticevicApi.DL.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using View = AnticevicApi.Model.View;

namespace AnticevicApi.BL.Handlers.Trip
{
    public class TripHandler : Handler<TripHandler>, ITripHandler
    {
        public TripHandler(IHandlerContext<TripHandler> context) : base(context)
        {
        }

        public View.Trip.Trip GetSingle(string valueId)
        {
            using (var context = GetMainContext())
            {
                var trip = context.Trips.WhereUser(User.Id)
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
                    Expenses = expenses.Select(x => new View.Expense.Expense(x))
                };

                return tripView;
            }
        }
    }
}
