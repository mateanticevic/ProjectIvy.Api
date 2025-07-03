using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Model.Constants.Database;

[JsonConverter(typeof(StringEnumConverter))]
public enum BeerServing
{
    OnTap = 1,
    Bottle = 2,
    Can = 3,
    Plastic = 4
}
