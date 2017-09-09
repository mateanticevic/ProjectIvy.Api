using ProjectIvy.Model.Binding.Common;

namespace ProjectIvy.Model.Binding.Trip
{
    public class TripGetBinding : FilteredPagedBinding
    {
        public string CityId { get; set; }
    }
}
