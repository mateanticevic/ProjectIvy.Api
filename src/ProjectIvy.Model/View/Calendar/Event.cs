namespace ProjectIvy.Model.View.Calendar;

public class Event
{
    public Event(Database.Main.User.Event e)
    {
        Id = e.ValueId;
        Name = e.Name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}
