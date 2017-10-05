using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace ProjectIvy.Model.Binding.Expense
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExpenseSort
    {
        Date,
        Amount
    }
}
