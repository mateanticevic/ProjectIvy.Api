namespace ProjectIvy.Model.View.Calendar;

public class CalendarDay
{
    public bool IsHoliday { get; set; }

    public DateTime Date { get; set; }

    public WorkDayType WorkDayType { get; set; }
}
