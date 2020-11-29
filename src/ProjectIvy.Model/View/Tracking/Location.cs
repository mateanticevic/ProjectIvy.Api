using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Tracking
{
    public class Location
    {
        public Location(DatabaseModel.Tracking.Location l)
        {
            Name = l.Description;
        }

        public string Name { get; set; }
    }
}
