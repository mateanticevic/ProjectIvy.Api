using System;
namespace ProjectIvy.Model.Binding.Ride
{
    public class RideBinding
    {
        public string DestinationCityId { get; set; }

        public string DestinationPoiId { get; set; }

        public string OriginCityId { get; set; }

        public string OriginPoiId { get; set; }

        public DateTime Arrival { get; set; }

        public DateTime Departure { get; set; }

        public string TypeId { get; set; }
    }
}
