using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.City
{
    public class City
    {
        public City(DatabaseModel.Common.City x)
        {
            Name = x.Name;
            Id = x.ValueId;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
