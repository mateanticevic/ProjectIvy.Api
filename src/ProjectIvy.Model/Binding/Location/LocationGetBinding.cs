using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.Location;

public class LocationGetBinding : PagedBinding, ISearchable
{
    public string Search { get; set; }
}