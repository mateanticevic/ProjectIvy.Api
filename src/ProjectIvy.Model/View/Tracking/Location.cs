using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Tracking
{
    public class Location
    {
        public Location(DatabaseModel.Tracking.Location l)
        {
            Name = l.Description;
            TypeId = l.LocationType.ValueId;
        }

        public string Name { get; set; }

        public string TypeId { get; set; }
    }
}
