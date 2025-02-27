using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Tracking;

public class KnownLocation
{
    public KnownLocation(DatabaseModel.Tracking.Location l)
    {
        Name = l.Name;
        TypeId = l.LocationType.ValueId;
    }

    public string Name { get; set; }

    public string TypeId { get; set; }
}
