using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;

namespace ProjectIvy.Business.Handlers.Vendor
{
    public class VacationHandler : Handler<VacationHandler>, IVacationHandler
    {
        public VacationHandler(IHandlerContext<VacationHandler> context) : base(context)
        {
        }

        public async Task CreateVacation(DateTime date)
        {
            var weekendDays = DayOfWeek.Saturday | DayOfWeek.Sunday;
            if ((date.DayOfWeek & weekendDays) != 0)
                throw new Exception();

            using var context = GetMainContext();

            var vacation = new Model.Database.Main.User.Vacation()
            {
                Date = date,
                UserId = UserId
            };
            context.Vacations.Add(vacation);
            await context.SaveChangesAsync();
        }

        public async Task DeleteVacation(DateTime date)
        {
            using var context = GetMainContext();

            var vacation = await context.Vacations.WhereUser(UserId)
                                                  .FirstOrDefaultAsync(x => x.Date == date);

            if (vacation == null)
                throw new Exception();

            context.Remove(vacation);
            await context.SaveChangesAsync();
        }
    }
}
