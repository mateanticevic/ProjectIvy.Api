using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Airline
{
    public class Airline
    {
        public Airline(DatabaseModel.Transport.Airline a)
        {
            Id = a.ValueId;
            Name = a.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
