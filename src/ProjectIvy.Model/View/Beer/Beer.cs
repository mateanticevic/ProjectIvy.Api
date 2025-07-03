using ProjectIvy.Common.Extensions;
using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Beer;

public class Beer
{
    public Beer() { }

    public Beer(DatabaseModel.Beer.Beer b)
    {
        Id = b.ValueId;
        Name = b.Name;
        Abv = b.Abv;
        Brand = b.BeerBrand.ConvertTo(x => new BeerBrand(x));
        Style = b.BeerStyle.ConvertTo(x => new BeerStyle(x));
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public decimal Abv { get; set; }

    public BeerBrand Brand { get; set; }

    public BeerStyle Style { get; set; }
}
