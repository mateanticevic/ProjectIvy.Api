namespace ProjectIvy.Model.View.Calendar;

public class CalendarDay
{
    public IEnumerable<City.City> Cities { get; set; }

    public IEnumerable<City.CityVisited> CityVisits { get; set; }

    public IEnumerable<Country.Country> Countries { get; set; }

    public IEnumerable<Event> Events { get; set; }

    public IEnumerable<IcsCalendarEvent> ExternalEvents { get; set; }

    public bool IsHoliday { get; set; }

    public IEnumerable<Location.LocationVisited> Locations { get; set; }

    public IEnumerable<TimelineItem> Timeline { get; set; }

    public DateTime Date { get; set; }

    public WorkDayTypeOld WorkDayType { get; set; }
}
