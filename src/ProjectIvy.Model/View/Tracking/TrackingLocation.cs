using ProjectIvy.Model.View.Country;

namespace ProjectIvy.Model.View.Tracking
{
    public class TrackingLocation
    {
        public City.City City { get; set; }

        public Country.Country Country { get; set; }

        public Location Location { get; set; }

        public Tracking Tracking { get; set; }
    }
}
