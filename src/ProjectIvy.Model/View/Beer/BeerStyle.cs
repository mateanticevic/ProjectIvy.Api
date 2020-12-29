using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Beer
{
    public class BeerStyle
    {
        public BeerStyle() { }

        public BeerStyle(DatabaseModel.Beer.BeerStyle x)
        {
            Id = x.ValueId;
            Name = x.Name;
        }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
