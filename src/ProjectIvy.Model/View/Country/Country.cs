using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Country;

public class Country
{
    public Country() { }

    public Country(DatabaseModel.Common.Country x)
    {
        Name = x.Name;
        Id = x.ValueId;
        Population = x.Population;
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public long? Population { get; set; }
}

public class CountryComparer : IEqualityComparer<Country>
{
    public bool Equals(Country x, Country y) => x.Id == y.Id;

    public int GetHashCode(Country obj) => obj.Id.GetHashCode();
}
