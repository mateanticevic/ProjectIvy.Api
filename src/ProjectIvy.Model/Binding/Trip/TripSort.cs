using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ProjectIvy.Model.Binding.Trip
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TripSort
    {
        Date,
        Duration
    }
}
