namespace ProjectIvy.Model.View.Calendar;

public class IcsCalendarEvent
{
    public string Summary { get; set; }

    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    public string Description { get; set; }

    public string Location { get; set; }

    public string Uid { get; set; }
}
