using ProjectIvy.Model.Binding.Route;

namespace ProjectIvy.Model.Binding.City
{
    public class CityGetBinding : PagedBinding, ISearchable
    {
        public string CountryId { get; set; }

        public string Search { get; set; }
    }
}
