using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Calendar;
using ProjectIvy.Model.View.Calendar;

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

        public async Task<CalendarSection> Get(DateTime from, DateTime to)
        {
            using var context = GetMainContext();

            var calendarSection = new CalendarSection();
            var calendarDays = new List<CalendarDay>();

            var holidays = await context.Holidays.Where(x => x.Date >= from && x.Date <= to)
                                                 .Select(x => x.Date)
                                                 .ToListAsync();

            var workDays = await context.WorkDays.WhereUser(UserId)
                                                 .Include(x => x.WorkDayType)
                                                 .Where(x => x.Date >= from && x.Date <= to)
                                                 .Select(x => new { x.Date, x.WorkDayType.ValueId, x.WorkDayType.Name })
                                                 .ToListAsync();

            var countriesPerDay = await context.Trackings.WhereUser(UserId)
                                                         .Include(x => x.Country)
                                                         .Where(x => x.Timestamp >= from && x.Timestamp <= to && x.CountryId != null)
                                                         .GroupBy(x => x.Timestamp.Date)
                                                         .Select(x => new { Date = x.Key, Countries = x.Select(c => c.Country).Distinct() })
                                                         .ToListAsync();

            foreach (var day in Enumerable.Range(0, (to - from).Days + 1).Select(x => from.AddDays(x)))
            {
                var workDay = await context.WorkDays.WhereUser(UserId)
                                                    .FirstOrDefaultAsync(x => x.Date == day);

                var calendarDay = new CalendarDay()
                {
                    Date = day,
                    IsHoliday = holidays.Contains(day),
                    Countries = countriesPerDay.SingleOrDefault(x => x.Date == day)?.Countries.Select(x => new Model.View.Country.Country(x))
                };

                if (workDays.Any(x => x.Date == day))
                {
                    var workDayType = workDays.First(x => x.Date == day);
                    calendarDay.WorkDayType = new WorkDayType()
                    {
                        Id = workDayType.ValueId,
                        Name = workDayType.Name
                    };
                }

                calendarDays.Add(calendarDay);
            }
            calendarSection.Days = calendarDays.OrderByDescending(x => x.Date);

            return calendarSection;
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
            catch (Exception e)
            {

            }
        }
    }
}
