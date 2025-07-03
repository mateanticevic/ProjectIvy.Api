using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Model.Binding.Trip;

[JsonConverter(typeof(StringEnumConverter))]
public enum TripSort
{
    Date,
    Duration
}
