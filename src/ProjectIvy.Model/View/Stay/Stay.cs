using DatabaseModel = ProjectIvy.Model.Database.Main;

namespace ProjectIvy.Model.View.Stay;

public class Stay
{
    public Stay(DatabaseModel.Travel.Stay x)
    {
        Id = x.Id;
        Date = x.Date;
        CityId = x.CityId;
        CountryId = x.CountryId;
        City = x.City != null ? new City.City(x.City) : null;
        Country = new Country.Country(x.Country);
    }

    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int? CityId { get; set; }

    public int CountryId { get; set; }

    public City.City City { get; set; }

    public Country.Country Country { get; set; }
}