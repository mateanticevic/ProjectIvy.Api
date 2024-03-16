using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding;
using ProjectIvy.Model.Binding.Calendar;
using ProjectIvy.Model.View;

namespace ProjectIvy.Business.Handlers.Vendor
{
    public class CalendarHandler : Handler<CalendarHandler>, ICalendarHandler
    {
        public CalendarHandler(IHandlerContext<CalendarHandler> context) : base(context)
        {
        }

        public async Task CreateVacation(DateTime date)
        {
            var weekendDays = DayOfWeek.Saturday | DayOfWeek.Sunday;
            if ((date.DayOfWeek & weekendDays) != 0)
                throw new Exception();

            using var context = GetMainContext();

            var vacation = new Model.Database.Main.User.WorkDay()
            {
                Date = date,
                UserId = UserId
            };
            context.WorkDays.Add(vacation);
            await context.SaveChangesAsync();
        }

        public async Task DeleteVacation(DateTime date)
        {
            using var context = GetMainContext();

            var vacation = await context.WorkDays.WhereUser(UserId)
                                                  .FirstOrDefaultAsync(x => x.Date == date);

            if (vacation == null)
                throw new Exception();

            context.Remove(vacation);
            await context.SaveChangesAsync();
        }

        public async Task<PagedView<DateTime>> Get(FilteredPagedBinding binding)
        {
            using var context = GetMainContext();

            return await context.WorkDays.WhereUser(UserId)
                                          .WhereIf(binding.From.HasValue, x => x.Date >= binding.From)
                                          .WhereIf(binding.To.HasValue, x => x.Date <= binding.To)
                                          .OrderByDescending(x => x.Date)
                                          .Select(x => x.Date)
                                          .ToPagedViewAsync(binding);
        }

        public async Task UpdateDay(DateTime day, CalendarDayUpdateBinding b)
        {
            using var context = GetMainContext();

            var workDay = await context.WorkDays.WhereUser(UserId)
                                            .FirstOrDefaultAsync(x => x.Date == day);

            workDay ??= new Model.Database.Main.User.WorkDay()
            {
                Date = day,
                UserId = UserId
            };

            if (!string.IsNullOrEmpty(b.WorkDayTypeId))
            {
                int? workDayTypeId = context.WorkDayTypes.SingleOrDefault(x => x.ValueId == b.WorkDayTypeId)?.Id;

                if (workDayTypeId == null)
                    throw new Exception();

                workDay.WorkDayTypeId = workDayTypeId.Value;
            }

            if (workDay.Id == 0)
                context.WorkDays.Add(workDay);
            else
                context.WorkDays.Update(workDay);

                try
                {

            await context.SaveChangesAsync();
                }
                catch(Exception e)
                {

                }
        }
    }
}
