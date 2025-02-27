namespace ProjectIvy.Model.View.City;

public class CityComparer : IEqualityComparer<City>
{
    public bool Equals(City x, City y) => x.Id == y.Id;

    public int GetHashCode(City obj) => obj.Id.GetHashCode();
}
