using System.Collections.Generic;

namespace ProjectIvy.Model.Binding.Trip
{
    public class TripGetBinding : FilteredPagedBinding
    {
        public IEnumerable<string> CityId { get; set; }

        public IEnumerable<string> CountryId { get; set; }

        public TripSort OrderBy { get; set; }

        public string Search { get; set; }

        public bool? IsDomestic { get; set; }
    }
}
