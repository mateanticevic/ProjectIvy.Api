using AnticevicApi.Extensions.BuiltInTypes;
using AnticevicApi.Model.Binding.Trip;
using AnticevicApi.Model.Database.Main.Travel;

namespace AnticevicApi.BL.MapExtensions
{
    public static class TripExtensions
    {
        public static Trip ToEntity(this TripBinding binding, Trip trip = null)
        {
            trip = trip == null ? new Trip() : trip;

            trip.ValueId = binding.Name.NameToValueId();
            trip.Name = binding.Name;
            trip.TimestampEnd = binding.TimestampEnd;
            trip.TimestampStart = binding.TimestampStart;

            return trip;
        }
    }
}
