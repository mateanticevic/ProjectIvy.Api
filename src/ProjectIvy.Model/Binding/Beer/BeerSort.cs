using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Model.Binding.Beer;

[JsonConverter(typeof(StringEnumConverter))]
public enum BeerSort
{
    Abv,
    Name
}
