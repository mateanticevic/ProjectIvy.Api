namespace ProjectIvy.Model.View.Calendar;

public class TimelineItem
{
    public City.City City { get; set; }

    public Location.Location Location { get; set; }

    public DateTime? EnterTime { get; set; }

    public DateTime? ExitTime { get; set; }
}
