namespace ProjectIvy.Model.View.Country;

public class CountryComparer : IEqualityComparer<Country>
{
    public bool Equals(Country x, Country y) => x.Id == y.Id;

    public int GetHashCode(Country obj) => obj.Id.GetHashCode();
}
