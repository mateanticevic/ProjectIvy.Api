using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProjectIvy.Model.Binding.Income;

[JsonConverter(typeof(StringEnumConverter))]
public enum IncomeSort
{
    Date,
    Amount
}
