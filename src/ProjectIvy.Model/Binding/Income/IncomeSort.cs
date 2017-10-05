using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ProjectIvy.Model.Binding.Income
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IncomeSort
    {
        Date,
        Amount
    }
}
