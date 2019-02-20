using ProjectIvy.Data.Extensions;
using ProjectIvy.Model.Binding.Trip;
using ProjectIvy.Model.Database.Main.Travel;

namespace ProjectIvy.BL.MapExtensions
{
    public static class TripExtensions
    {
        public static Trip ToEntity(this TripBinding binding, Trip trip = null)
        {
            trip = trip ?? new Trip();

            trip.ValueId = binding.Name.ToValueId();
            trip.Name = binding.Name;
            trip.TimestampEnd = binding.TimestampEnd;
            trip.TimestampStart = binding.TimestampStart;

            return trip;
        }
    }
}
