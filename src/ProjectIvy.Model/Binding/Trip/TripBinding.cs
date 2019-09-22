using System;
using System.Collections.Generic;

namespace ProjectIvy.Model.Binding.Trip
{
    public class TripBinding
    {
        public string Name { get; set; }

        public DateTime TimestampEnd { get; set; }

        public DateTime TimestampStart { get; set; }

        public IEnumerable<string> CityIds { get; set; }
    }
}
