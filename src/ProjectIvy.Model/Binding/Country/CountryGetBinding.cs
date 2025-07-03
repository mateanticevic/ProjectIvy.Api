using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.Country;

public class CountryGetBinding : PagedBinding, ISearchable
{
    public string Search { get; set; }
}
