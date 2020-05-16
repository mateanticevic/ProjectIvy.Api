using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Beer
{
    public class Beer
    {
        public Beer(DatabaseModel.Beer.Beer b)
        {
            Id = b.ValueId;
            Name = b.Name;
            Abv = b.Abv;
            Style = b.BeerStyle.ConvertTo(x => new BeerStyle(x));
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Abv { get; set; }

        public BeerStyle Style { get; set; }
    }
}
