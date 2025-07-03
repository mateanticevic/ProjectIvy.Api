using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectIvy.Model.Database.Main.Travel;

[Table(nameof(CountryListCountry), Schema = nameof(Travel))]
public class CountryListCountry
{
    public int CountryListId { get; set; }

    public int CountryId { get; set; }

    public Common.Country Country { get; set; }

    public CountryList CountryList { get; set; }
}
