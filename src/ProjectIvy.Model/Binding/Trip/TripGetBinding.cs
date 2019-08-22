namespace ProjectIvy.Model.Binding.Trip
{
    public class TripGetBinding : FilteredPagedBinding
    {
        public string CityId { get; set; }

        public string CountryId { get; set; }

        public TripSort OrderBy { get; set; }
    }
}
