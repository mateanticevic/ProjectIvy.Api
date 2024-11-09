using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Calendar;
using ProjectIvy.Model.View.Calendar;

namespace ProjectIvy.Business.Handlers.Calendar;

public class CalendarHandler : Handler<CalendarHandler>, ICalendarHandler
{
    public CalendarHandler(IHandlerContext<CalendarHandler> context) : base(context)
    {
    }

    public async Task CreateEvent(DateTime date, string name)
    {
        using var context = GetMainContext();

        var @event = new Model.Database.Main.User.Event()
        {
            Date = date,
            Name = name,
            UserId = UserId,
            ValueId = $"${date:YYYY-MM-DD}-${name.ToValueId}"
        };
        context.Events.Add(@event);
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

        bool rangeInFuture = from > DateTime.Now;

        var countriesPerDay = rangeInFuture ? default : await context.Trackings.WhereUser(UserId)
                                                                               .Include(x => x.Country)
                                                                               .Where(x => x.Timestamp >= from && x.Timestamp.Date <= to && x.CountryId != null)
                                                                               .GroupBy(x => x.Timestamp.Date)
                                                                               .Select(x => new { Date = x.Key, Countries = x.Select(c => c.Country).Distinct() })
                                                                               .ToListAsync();

        var citiesPerDay = rangeInFuture ? default : await context.Trackings.WhereUser(UserId)
                                                                            .Include(x => x.City)
                                                                            .ThenInclude(x => x.Country)
                                                                            .Where(x => x.Timestamp >= from && x.Timestamp.Date <= to && x.CityId != null)
                                                                            .GroupBy(x => x.Timestamp.Date)
                                                                            .Select(x => new { Date = x.Key, Cities = x.Select(c => c.City).Distinct() })
                                                                            .ToListAsync();

        var locationsPerDay = rangeInFuture ? default : await context.Trackings.WhereUser(UserId)
                                                                               .Include(x => x.Location)
                                                                               .Where(x => x.Timestamp >= from && x.Timestamp.Date <= to && x.LocationId != null)
                                                                               .GroupBy(x => x.Timestamp.Date)
                                                                               .Select(x => new { Date = x.Key, Locations = x.Select(c => c.Location).Distinct() })
                                                                               .ToListAsync();

        var events = await context.Events.WhereUser(UserId)
                                         .Where(x => x.Date >= from && x.Date <= to)
                                         .ToListAsync();

        foreach (var day in Enumerable.Range(0, (to - from).Days + 1).Select(x => from.AddDays(x)))
        {
            var workDay = await context.WorkDays.WhereUser(UserId)
                                                .FirstOrDefaultAsync(x => x.Date == day);

            var calendarDay = new CalendarDay()
            {
                Cities = citiesPerDay?.SingleOrDefault(x => x.Date == day)?.Cities.Select(x => new Model.View.City.City(x)),
                Countries = countriesPerDay?.SingleOrDefault(x => x.Date == day)?.Countries.Select(x => new Model.View.Country.Country(x)),
                Date = day,
                Events = events.Where(x => x.Date == day).Select(x => new Event(x)),
                IsHoliday = holidays.Contains(day),
                Locations = locationsPerDay?.SingleOrDefault(x => x.Date == day)?.Locations.Select(x => new Model.View.Location.Location(x)).OrderBy(x => x.Name),
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
