namespace ProjectIvy.Model.View.Location;

public class LocationVisited : Location
{
    public LocationVisited(Database.Main.Tracking.Location location, DateTime enterTime, DateTime? exitTime)
        : base(location)
    {
        EnterTime = enterTime;
        ExitTime = exitTime;
    }

    public DateTime EnterTime { get; set; }

    public DateTime? ExitTime { get; set; }
}
