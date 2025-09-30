using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Model.Binding.Stay;

[JsonConverter(typeof(StringEnumConverter))]
public enum StaySort
{
    Date
}