namespace ProjectIvy.Model.View.Calendar;

public class CalendarDay
{
    public IEnumerable<City.City> Cities { get; set; }

    public IEnumerable<Country.Country> Countries { get; set; }

    public IEnumerable<Event> Events { get; set; }

    public bool IsHoliday { get; set; }

    public DateTime Date { get; set; }

    public WorkDayType WorkDayType { get; set; }
}
