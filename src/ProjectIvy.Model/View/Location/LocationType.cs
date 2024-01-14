namespace ProjectIvy.Model.View.Location;

public class LocationType
{
    public LocationType(Database.Main.Tracking.LocationType lt)
    {
        Id = lt.ValueId;
        Name = lt.Name;
    }

    public string Id { get; set; }

    public string Name { get; set; }
}