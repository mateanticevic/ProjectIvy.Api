using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ProjectIvy.Model.Binding.Beer
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum BeerSort
    {
        Abv,
        Name
    }
}
