namespace ProjectIvy.Model.Binding.Route;

public class RouteGetBinding : PagedBinding, ISearchable
{
    public string Search { get; set; }
}