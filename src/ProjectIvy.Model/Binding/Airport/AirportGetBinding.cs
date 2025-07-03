namespace ProjectIvy.Model.Binding.Airport;

public class AirportGetBinding : PagedBinding
{
    public bool? Visited { get; set; }

    public string CityId { get; set; }

    public string Countryid { get; set; }

    public string Search { get; set; }
}
